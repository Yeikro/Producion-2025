using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemigo : MonoBehaviour, IPunObservable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    public Estados estado;
    public float distanciaSeguir;
    public float distanciaAtacar;
    public float distanciaEscapar;

    public bool autoseleccionarTarget;
    public Transform target;
    public float distancia;

    public bool vivo = true;

    public float vidaMaxima = 10f;
    public float vida;
    public UnityEvent eventoMorir;

    PhotonView PV;

    public void Awake()
    {
        StartCoroutine(CalcularDistancia());
        PosAwake();
        vida = vidaMaxima;
        PV = GetComponent<PhotonView>();
        if (PV != null && !PV.IsMine)
        {
            this.enabled = false;
        }
    }
    public virtual void PosAwake()
    {

    }

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        if (autoseleccionarTarget)
            CalcularTarget();
    }

    public void CalcularTarget()
    {
        float d = 100000;
        target = ControlObjetivos.singleton.objetivos[0];
        for (int i = 0; i < ControlObjetivos.singleton.objetivos.Count; i++)
        {
            float d2 = (transform.position - ControlObjetivos.singleton.objetivos[i].position).sqrMagnitude;
            if (d2 < d)
            {
                d = d2;
                target = ControlObjetivos.singleton.objetivos[i];
            }
        }
    }
    public void LateUpdate()
    {
        CheckEstados();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaAtacar);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaSeguir);
        Handles.color = Color.green;
        Handles.DrawWireDisc(transform.position, Vector3.up, distanciaEscapar);
    }
#endif

    public void CheckEstados()
    {
        switch (estado)
        {
            case Estados.Idle:
                EstadoIdle();
                break;
            case Estados.Seguir:
                transform.LookAt(target, Vector3.up);
                EstadoSeguir();
                break;
            case Estados.Atacar:
                EstadoAtaque();
                break;
            case Estados.Muerto:
                EstadoMuerto();
                break;
            default:
                break;
        }
    }

    public void CambiarDeEstado(Estados e)
    {
        switch (e)
        {
            case Estados.Idle:
                break;
            case Estados.Seguir:
                break;
            case Estados.Atacar:
                break;
            case Estados.Muerto:
                vivo = false;

                break;
            default:
                break;
        }
        estado = e;
    }

    public virtual void EstadoIdle()
    {
        if (distancia < distanciaSeguir)
        {
            CambiarDeEstado(Estados.Seguir);
        }
    }

    public virtual void EstadoAtaque()
    {
        if (distancia > distanciaAtacar + 0.5f)
        {
            CambiarDeEstado(Estados.Seguir);
        }
    }

    public virtual void EstadoSeguir()
    {
        if (distancia < distanciaAtacar)
        {
            CambiarDeEstado(Estados.Atacar);
        }
        else if (distancia > distanciaEscapar)
        {
            CambiarDeEstado(Estados.Idle);
        }
    }

    public virtual void EstadoMuerto()
    {
        vivo = false;
        //Debug.Log("El enemigo ha muerto.");
        //gameObject.SetActive(false);
    }

    IEnumerator CalcularDistancia()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            CalcularTarget();
            if (target != null)
            {
                distancia = Vector3.Distance(transform.position, target.position);
            }

        }
    }

    public void RecibirDa�o(float cantidad)
    {
        if (!vivo) return;

        vida -= cantidad;
        healthbarImage.fillAmount = vida / vidaMaxima;
        Debug.Log($"El enemigo recibi� {cantidad} de da�o. Vida restante: {vida}");

        if (vida <= 0 && PV.IsMine)
        {
            eventoMorir.Invoke();
            CambiarDeEstado(Estados.Muerto);
        }
    }
    [ContextMenu("CausarDa�o")]
    public void Da�ar()
    {
        RecibirDa�o(5);
    }

    [PunRPC]
    public void RPC_RecibirDa�o(float cantidad)
    {
        RecibirDa�o(cantidad); // Solo lo ejecuta el due�o del enemigo
    }

    public void SolicitarDa�o(float cantidad)
    {
        if (!PV.IsMine)
        {
            // Si NO soy el due�o del enemigo, le pido al due�o que aplique el da�o
            PV.RPC("RPC_RecibirDa�o", PV.Owner, cantidad);
        }
        else
        {
            // Si soy el due�o, lo aplico directamente
            RecibirDa�o(cantidad);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Soy el due�o, env�o mi valor de vida
            stream.SendNext(vida);
        }
        else
        {
            // Soy un observador, recibo el valor de vida
            vida = (float)stream.ReceiveNext();
            healthbarImage.fillAmount = vida / vidaMaxima;
        }
    }
}

public enum Estados
{
    Idle = 0,
    Seguir = 1,
    Atacar = 2,
    Muerto = 3
}
