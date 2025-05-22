using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaPilar : MonoBehaviour
{
    public Vida vida;
    public GameObject canvasVidaMundo;
    public GameObject indicador;
    public ParticleSystem zonaParticula;

    private int jugadoresDentro = 0;
    private int enemigosDentro = 0;

    private void Start()
    {
        if (vida == null)
            vida = GetComponentInParent<Vida>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jugador")) jugadoresDentro++;
        if (other.CompareTag("Enemigo")) enemigosDentro++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Jugador")) jugadoresDentro--;
        if (other.CompareTag("Enemigo")) enemigosDentro--;
    }

    private void Update()
    {
        if (!vida.GetComponent<PhotonView>().IsMine) return; // Solo el dueño controla la lógica

        if (vida.estaMuerto || vida.vidaActual >= vida.vidaInicial) return;

        float diferencia = jugadoresDentro - enemigosDentro;

        if (diferencia == 0)
        {
            // Igual número → no se cura ni se daña y sale de la lista de objetivos
            if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
            {
                ControlObjetivos.singleton.objetivos.Remove(transform.parent);
            }
        }
        else
        {
            if (vida.vidaActual <= 0f)
            {
                // Pilar muerto → marcar como tal, quitar de objetivos y desactivar zona
                vida.estaMuerto = true;
                canvasVidaMundo.SetActive(false);
                indicador.SetActive(false);
                zonaParticula.Stop();

                Debug.Log("pilar caido");

                PilarScoreManager.instance.RegistrarPilarMuerto();

                if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
                {
                    ControlObjetivos.singleton.objetivos.Remove(transform.parent);
                }

                gameObject.SetActive(false); //Desactiva la zona de curación
                return;
            }

            // Sigue siendo un objetivo
            if (!ControlObjetivos.singleton.objetivos.Contains(transform.parent))
            {
                ControlObjetivos.singleton.objetivos.Add(transform.parent);
            }

            float cambio = Time.deltaTime * Mathf.Abs(diferencia) * 2f;

            if (diferencia > 0)
            {
                // Más jugadores → curar
                vida.vidaActual = Mathf.Min(vida.vidaActual + cambio, vida.vidaInicial);
                vida.ActualizarInterfaz();
            }
            else
            {
                // Más enemigos → dañar
                vida.vidaActual = Mathf.Max(vida.vidaActual - cambio, 0);
                vida.ActualizarInterfaz();

                if (vida.vidaActual <= 0f)
                {
                    vida.estaMuerto = true;

                    Debug.Log("pilar caido");

                    PilarScoreManager.instance.RegistrarPilarMuerto();

                    if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
                    {
                        ControlObjetivos.singleton.objetivos.Remove(transform.parent);
                    }

                    gameObject.SetActive(false);
                }
            }
        }

        // Si se curó al máximo → dejar de ser objetivo
        if (vida.vidaActual >= vida.vidaInicial)
        {
            if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
            {
                ControlObjetivos.singleton.objetivos.Remove(transform.parent);
            }

            PilarScoreManager.instance.RegistrarPilarRecuperado();
            canvasVidaMundo.SetActive(false);
            indicador.SetActive(false);
            zonaParticula.Stop();
        }
    }
}