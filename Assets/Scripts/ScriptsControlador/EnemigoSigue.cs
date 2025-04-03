using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemigoSigue : Enemigo
{
    private NavMeshAgent agente;
    public Animator animaciones;
    public Transform[] checkPoint;
    private int indice;
    public float distanciaCheckPoint;
    private float distanciaCheckPoint2;
    public float daño = 2;


    public override void PosAwake()
    {
        agente = GetComponent<NavMeshAgent>();
        distanciaCheckPoint2 = distanciaCheckPoint * distanciaCheckPoint;
    }

    private void Awake()
    {
        base.Awake();
    }

    public override void EstadoIdle()
    {
        base.EstadoIdle();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 1);
        if (animaciones != null) animaciones.SetBool("Atacando", false);
        if (animaciones != null) animaciones.SetBool("Vivo", true);

        agente.SetDestination(checkPoint[indice].position);
        if ((checkPoint[indice].position - transform.position).sqrMagnitude < distanciaCheckPoint2)
        {
            indice = Random.Range(0,checkPoint.Length);
        }
        
    }

    public override void EstadoSeguir()
    {
        base.EstadoSeguir();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 1);
        if (animaciones != null) animaciones.SetBool("Atacando", false);
        if (target != null) agente.SetDestination(target.position);

    }

    public override void EstadoAtaque()
    {
        base.EstadoAtaque();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 0);
        if (animaciones != null) animaciones.SetBool("Atacando", true);
        agente.SetDestination(transform.position);
        transform.LookAt(target, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public override void EstadoMuerto()
    {
        base.EstadoMuerto();
        if (animaciones != null) animaciones.SetBool("Vivo", false);
        //agente.enabled = true;
    }

    [ContextMenu("Matar")]

    public void Matar()
    {
        if (vivo)
        {
            CambiarDeEstado(Estados.Muerto);
            Invoke("Respawn", 5f);
        }
    }

    public void Atacar()
    {
        //Personaje.personajeLocal.vida.CausasDaño(daño);
        Vida v = target.GetComponent<Vida>();
        if (v!=null)
        {
           v.CausasDaño(daño);
           
        }
    }

    public void Respawn()
    {

        // Obtiene una posición aleatoria de respawn
        Transform puntoRespawn = PuntosRespown.singleton.GetPosEnemigo();
        transform.position = puntoRespawn.position;

        CambiarDeEstado(Estados.Idle);
        agente.enabled = true;
        if (animaciones != null) animaciones.SetBool("Vivo", true);
        vida = vidaMaxima;
        vivo = true;
        Debug.Log("¡Has reaparecido!");
    }

}

