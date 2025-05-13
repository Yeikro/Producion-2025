using UnityEngine;
using Photon.Pun;

public class InicializadorJugador : MonoBehaviour
{
    public GameObject uiCanvas; // Asigna aquí el objeto que contiene el PiUIManager en el inspector

    void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (!pv.IsMine)
        {
            if (uiCanvas != null)
                uiCanvas.SetActive(false); // Oculta el menú UI de otros jugadores
        }
    }
}