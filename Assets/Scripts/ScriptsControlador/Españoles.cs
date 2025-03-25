using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Españoles : Enemigo
{
    private NavMeshAgent agente;
    public Animator animaciones;
    public float daño = 2;
    

    public override void PosAwake()
    {
        agente = GetComponent<NavMeshAgent>();

    }

    private void Awake()
    {
        base.Awake();
    }

    public override void EstadoIdle()
    {
        base.EstadoIdle();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 0);
        if (animaciones != null) animaciones.SetBool("Atacando", false);
        agente.SetDestination(transform.position);
    }

    public override void EstadoSeguir()
    {
        base.EstadoSeguir();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 1);
        if (animaciones != null) animaciones.SetBool("Atacando", false);
        agente.SetDestination(target.position);
        
    }

    public override void EstadoAtaque()
    {
        base.EstadoAtaque();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 0);
        if (animaciones != null) animaciones.SetBool("Atacando", true);
        agente.SetDestination(transform.position);
        transform.LookAt(target, Vector3.up);
    }

    public override void EstadoMuerto()
    {
        base.EstadoMuerto();
        if (animaciones != null) animaciones.SetBool("Vivo", false);
        agente.enabled = false;
    }

    [ContextMenu("Matar")]

    public void Matar()
    {
        CambiarDeEstado(Estados.Muerto);
    }

    public void Atacar()
    {
        Personaje.personajeLocal.vida.CausasDaño(daño);
    }

}
