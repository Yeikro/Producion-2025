using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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

    public float vidaMaxima = 100f;
    public float vida;

    public void Awake()
    {
        StartCoroutine(CalcularDistancia());
        PosAwake();
        //vida = vidaMaxima;
    }
    public virtual void PosAwake()
    {

    }

     private void Start()
    {
        if (autoseleccionarTarget)
            target = Personaje.singleton.transform;
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
        if (distancia > distanciaAtacar + 0.4f)
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
            if (target != null)
            {
                yield return new WaitForSeconds(0.5f);
                distancia = Vector3.Distance(transform.position, target.position);
            }
            
        }
    }

    //public void RecibirDaño(float cantidad)
    //{
    //    if (!vivo) return;

    //    vida -= cantidad;
    //    Debug.Log($"El enemigo recibió {cantidad} de daño. Vida restante: {vida}");

    //    if (vida <= 0)
    //    {
    //        CambiarDeEstado(Estados.Muerto);
    //    }
    //}
}

public enum Estados
{
    Idle = 0,
    Seguir = 1,
    Atacar = 2,
    Muerto = 3
}
