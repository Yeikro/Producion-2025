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

    private PilarSubject pilarSubject;

    private void Start()
    {
        if (vida == null)
            vida = GetComponentInParent<Vida>();

        pilarSubject = GetComponentInParent<PilarSubject>();
        if (pilarSubject == null)
            Debug.LogError("No se encontró PilarSubject en el padre del Pilar");
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
        if (!vida.GetComponent<PhotonView>().IsMine) return;

        if (vida.estaMuerto || vida.vidaActual >= vida.vidaInicial) return;

        float diferencia = jugadoresDentro - enemigosDentro;

        if (diferencia == 0)
        {
            if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
            {
                ControlObjetivos.singleton.objetivos.Remove(transform.parent);
            }
        }
        else
        {
            if (vida.vidaActual <= 0f)
            {
                vida.estaMuerto = true;
                canvasVidaMundo.SetActive(false);
                indicador.SetActive(false);
                zonaParticula.Stop();

                Debug.Log("pilar caido");

                pilarSubject?.NotificarMuerte(); // <--- INVOKE EN LUGAR DIRECTO

                if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
                {
                    ControlObjetivos.singleton.objetivos.Remove(transform.parent);
                }

                gameObject.SetActive(false);
                return;
            }

            if (!ControlObjetivos.singleton.objetivos.Contains(transform.parent))
            {
                ControlObjetivos.singleton.objetivos.Add(transform.parent);
            }

            float cambio = Time.deltaTime * Mathf.Abs(diferencia) * 2f;

            if (diferencia > 0)
            {
                vida.vidaActual = Mathf.Min(vida.vidaActual + cambio, vida.vidaInicial);
                vida.ActualizarInterfaz();
            }
            else
            {
                vida.vidaActual = Mathf.Max(vida.vidaActual - cambio, 0);
                vida.ActualizarInterfaz();

                if (vida.vidaActual <= 0f)
                {
                    vida.estaMuerto = true;

                    Debug.Log("pilar caido");

                    pilarSubject?.NotificarMuerte(); // <--- INVOKE EN LUGAR DIRECTO

                    if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
                    {
                        ControlObjetivos.singleton.objetivos.Remove(transform.parent);
                    }

                    gameObject.SetActive(false);
                }
            }
        }

        if (vida.vidaActual >= vida.vidaInicial)
        {
            if (ControlObjetivos.singleton.objetivos.Contains(transform.parent))
            {
                ControlObjetivos.singleton.objetivos.Remove(transform.parent);
            }

            pilarSubject?.NotificarRecuperacion(); // <--- INVOKE EN LUGAR DIRECTO

            Debug.Log("pilar recuperado");
            canvasVidaMundo.SetActive(false);
            indicador.SetActive(false);
            zonaParticula.Stop();
        }
    }
}


