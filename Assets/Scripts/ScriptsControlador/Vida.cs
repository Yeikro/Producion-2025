using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Vida : MonoBehaviour
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    PhotonView PV;

    public float vidaInicial;
    public float vidaActual;
    public UnityEvent eventoMorir;
    public bool estaMuerto = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        Reiniciar();

        if (!PV.IsMine)
            Destroy(ui);
    }

    [PunRPC]
    public void CausarDañoRPC(float cuanto)
    {
        if (estaMuerto) return;

        vidaActual -= cuanto;
        healthbarImage.fillAmount = vidaActual / vidaInicial;

        if (vidaActual <= 0)
        {
            print("Muerto!! ->" + gameObject.name);
            eventoMorir.Invoke();
            estaMuerto = true;
        }
    }

    public void Reiniciar()
    {
        vidaActual = vidaInicial;
        healthbarImage.fillAmount = vidaActual / vidaInicial;
        estaMuerto = false;
    }

}
