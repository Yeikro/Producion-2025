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

        yield return null; // Esperar a sincronización
        yield return null;

        vida.vidaActual *= 0.5f;
        vida.ActualizarInterfaz();

        if (Photon.Pun.PhotonView.Get(this).IsMine)
        {
            ControlObjetivos.singleton.objetivos.Add(transform);

            vida.eventoMorir.AddListener(() =>
            {
                ControlObjetivos.singleton.objetivos.Remove(transform);
            });

            // Registrar nuevo pilar al manager como observador
            PilarSubject subject = GetComponent<PilarSubject>();
            if (subject != null)
            {
                PilarScoreManager.instance?.RegistrarNuevoPilar(subject);
            }
        }
    }
}
