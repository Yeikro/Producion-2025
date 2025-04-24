using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Vida))]

public class Pilar : MonoBehaviour
{
    Vida vida;
    IEnumerator Start()
    {
        vida = GetComponent<Vida>();

        // Espera a que se sincronice la vida si es necesario
        yield return null;
        yield return null;

        vida.vidaActual *= 0.5f;
        vida.ActualizarInterfaz();

        // REGISTRARSE como objetivo si es el dueño del objeto
        if (Photon.Pun.PhotonView.Get(this).IsMine)
        {
            ControlObjetivos.singleton.objetivos.Add(transform);

            // También registra la función para eliminarse al morir
            vida.eventoMorir.AddListener(() =>
            {
                ControlObjetivos.singleton.objetivos.Remove(transform);
            });
        }
    }
}
