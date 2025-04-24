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
    public float modificador = 1;//hace que tenga mas o menos daño, sirve para la trasformacion de los animales//

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

        vidaActual -= cuanto*modificador;
        healthbarImage.fillAmount = vidaActual / vidaInicial;

        if (vidaActual <= 0)
        {
            print("Muerto!! ->" + gameObject.name);
            eventoMorir.Invoke();
            estaMuerto = true;
        }
        CameraShake.Instance.Shake(0.5f, 0.3f);
    }

    public void Reiniciar()
    {
        vidaActual = vidaInicial;
        healthbarImage.fillAmount = vidaActual / vidaInicial;
        estaMuerto = false;
    }

}
