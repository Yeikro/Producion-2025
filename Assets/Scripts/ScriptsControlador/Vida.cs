using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Vida : MonoBehaviour, IPunObservable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    PhotonView PV;
    public MenuRadial menuRadial;

    public float vidaInicial;
    public float vidaActual;
    public UnityEvent eventoMorir;
    public bool estaMuerto = false;
    public float modificador = 1;//hace que tenga mas o menos da�o, sirve para la trasformacion de los animales//

    public bool cubierto = false;
    public float cobertura = 0.5f;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        Reiniciar();

        if (CompareTag("Jugador") && !PV.IsMine)
            Destroy(ui);

        if (menuRadial == null && PV.IsMine)
        {
            menuRadial = FindAnyObjectByType<MenuRadial>();
        }
    }

    [PunRPC]
    public void CausarDañoRPC(float cuanto)
    {
        if (estaMuerto) return;

        if (menuRadial!=null && menuRadial.inmune)
        {
            vidaActual -= cuanto * (1 - cobertura);
        }
        else
        {
            vidaActual -= cubierto ? cuanto * (1 - cobertura) : cuanto;
        }

        ActualizarInterfaz();

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            print("Muerto!! ->" + gameObject.name);
            eventoMorir.Invoke();
            estaMuerto = true;
        }
        
        CameraShake.Instance.ShakeCamera(0.3f, 0.5f, 3f);


    }

    public void ActualizarInterfaz()
    {
        healthbarImage.fillAmount = vidaActual / vidaInicial;
    }

    public void Reiniciar()
    {
        vidaActual = vidaInicial;
        healthbarImage.fillAmount = vidaActual / vidaInicial;
        estaMuerto = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(vidaActual);
            stream.SendNext(estaMuerto);
        }
        else
        {
            vidaActual = (float)stream.ReceiveNext();
            estaMuerto = (bool)stream.ReceiveNext();
            ActualizarInterfaz();
        }
    }
}
