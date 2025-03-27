using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemigo : MonoBehaviour
{
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

    PhotonView PV;

    public void Awake()
    {
        StartCoroutine(CalcularDistancia());
        PosAwake();
        vida = vidaMaxima;
        PV = GetComponent<PhotonView>();
        if (PV != null && ! PV.IsMine)
        {
            this.enabled = false;
        }
    }
    public virtual void PosAwake()
    {

    }

     private void Start()
    {
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
            if (d2<d)
            {
                d2 = d;
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
        gameObject.SetActive(false);
    }

    IEnumerator CalcularDistancia()
    {
        while (vivo)
        {
            yield return new WaitForSeconds(0.5f);
            CalcularTarget();
            if (target != null)
            {
                distancia = Vector3.Distance(transform.position, target.position);
            }
            
        }
    }

    public void RecibirDaño(float cantidad)
    {
        if (!vivo) return;

        vida -= cantidad;
        Debug.Log($"El enemigo recibió {cantidad} de daño. Vida restante: {vida}");

       if (vida <= 0)
        {
            CambiarDeEstado(Estados.Muerto);
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
