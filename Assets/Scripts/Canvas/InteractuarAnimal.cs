using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractuarAnimal : MonoBehaviour
{
    public Text textoInteraccion;
    public string nombreAnimal = "Jaguar";
    public ControladorAnimal controladorAnimal;

    private void FixedUpdate()
    {
        if (controladorAnimal.estaInteractuando)
        {
            textoInteraccion.gameObject.SetActive(false);
        }
        else
        {
            // Oculta si el jugador ya interactuó (si tienes referencia a él)
            GameObject jugador = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            if (jugador != null)
            {
                var registro = jugador.GetComponent<RegistroInteracciones>();
                if (registro != null && registro.YaInteractuoCon(nombreAnimal))
                {
                    textoInteraccion.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                // Solo el jugador local ve el texto
                var registro = other.GetComponent<RegistroInteracciones>();
                if (registro != null && !registro.YaInteractuoCon(nombreAnimal))
                {
                    textoInteraccion.gameObject.SetActive(true);
                    textoInteraccion.text = $"Interactuar 'F' con el {nombreAnimal}";
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                textoInteraccion.gameObject.SetActive(false);
            }
        }
    }
}
