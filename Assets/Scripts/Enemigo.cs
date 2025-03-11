using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Estados estado;
    public float distanciaSeguir;
    public float distanciaAtacar;
    public float distanciaEscapar;

    public void LateUpdate()
    {
        CheckEstados();
    }

    public void CheckEstados()
    {
        switch (estado) 
        {
            case Estados.Idle:
                EstadoIdle();
                break;
            case Estados.Seguir:
                EstadoSeguir();
                break;
            case Estados.Atacar:
                EstadoAtaque();
                break;
            case Estados.Muerto:
                
                break;
            default:
                break;
        }
    }
    public virtual void EstadoIdle()
    {

    }
    public virtual void EstadoAtaque()
    {

    }
    public virtual void EstadoSeguir()
    {

    }
}





public enum Estados
{
    Idle = 0,
    Seguir = 1,
    Atacar = 2,
    Muerto = 3
}