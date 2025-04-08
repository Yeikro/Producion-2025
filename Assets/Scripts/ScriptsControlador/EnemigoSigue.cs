using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

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
   

    PhotonView PV;


    public override void PosAwake()
    {
        agente = GetComponent<NavMeshAgent>();
        distanciaCheckPoint2 = distanciaCheckPoint * distanciaCheckPoint;
    }

    private void Awake()
    {
        base.Awake();
        PV = GetComponent<PhotonView>();
    }

    public override void EstadoIdle()
    {
        if (!PV.IsMine) return;
        base.EstadoIdle();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 1);
        if (animaciones != null) animaciones.SetBool("Atacando", false);
        if (animaciones != null) animaciones.SetBool("Vivo", true);

        agente.SetDestination(checkPoint[indice].position);
        if ((checkPoint[indice].position - transform.position).sqrMagnitude < distanciaCheckPoint2)
        {
            indice = Random.Range(0, checkPoint.Length);
        }

    }

    public override void EstadoSeguir()
    {
        if (!PV.IsMine) return;
        base.EstadoSeguir();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 1);
        if (animaciones != null) animaciones.SetBool("Atacando", false);
        if (target != null) agente.SetDestination(target.position);

    }

    public override void EstadoAtaque()
    {
        if (!PV.IsMine) return;
        base.EstadoAtaque();
        if (animaciones != null) animaciones.SetFloat("Velocidad", 0);
        if (animaciones != null) animaciones.SetBool("Atacando", true);
        agente.SetDestination(transform.position);
        transform.LookAt(target, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public override void EstadoMuerto()
    {
        if (!PV.IsMine) return;
        base.EstadoMuerto();
        if (animaciones != null) animaciones.SetBool("Vivo", false);
        //agente.enabled = true;
    }

    [ContextMenu("Matar")]

    public void Matar()
    {
        if (!PV.IsMine) return;
        if (vivo)
        {
            CambiarDeEstado(Estados.Muerto);
            Invoke("Respawn", 5f);
        }
    }

    public void Atacar()
    {
        if (!PV.IsMine) return;

        PhotonView targetPV = target.GetComponent<PhotonView>();

        if (targetPV != null)
        {
            targetPV.RPC("CausarDañoRPC", targetPV.Owner, daño);
        }
    }

    public void Respawn()
    {
        if (!PV.IsMine) return;
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

