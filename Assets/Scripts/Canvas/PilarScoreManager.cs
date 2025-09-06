using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class PilarScoreManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public CursorLockMode modo;

    public static PilarScoreManager instance;

    public Text textoPilaresVivos;
    public Text textoPilaresCaidos;

    private int pilaresVivos = 0;
    private int pilaresCaidos = 0;

    public GameObject panelVictoria;
    public Text textoVictoria;
    public Button botonVolverInicio;
    private bool juegoFinalizado = false;

    private PhotonView PV;

    public bool acaboJuego = false;

    private List<PilarSubject> pilaresRegistrados = new List<PilarSubject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        ActualizarUI();

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (botonVolverInicio != null)
        {
            botonVolverInicio.gameObject.SetActive(false);
            botonVolverInicio.onClick.AddListener(() =>
            {
                VolverAlInicioSeguro();
            });
        }
    }

    public void LateUpdate()
    {
        if (acaboJuego)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void VolverAlInicioSeguro()
    {
        Time.timeScale = 1f;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Se salió de la sala, ahora desconectando...");
        SceneManager.LoadScene("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Desconectado de Photon. Cargando escena online...");

        foreach (GameObject go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.scene.buildIndex == -1)
            {
                Destroy(go);
            }
        }

        SceneManager.LoadScene("Menu");
    }

    public void RegistrarPilarRecuperado()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pilaresVivos++;
            ActualizarUI();
            RevisarCondicionesVictoria();
        }
    }

    public void RegistrarPilarMuerto()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pilaresCaidos++;
            ActualizarUI();
            RevisarCondicionesVictoria();
        }
    }

    private void ActualizarUI()
    {
        if (textoPilaresVivos != null)
            textoPilaresVivos.text = "Pilares Vivos: " + pilaresVivos.ToString();

        if (textoPilaresCaidos != null)
            textoPilaresCaidos.text = "Pilares Caídos: " + pilaresCaidos.ToString();
    }

    private void RevisarCondicionesVictoria()
    {
        if (juegoFinalizado) return;
        if (!PhotonNetwork.IsMasterClient) return;

        if (pilaresCaidos >= 3)
        {
            PV.RPC("RPC_FinDelJuego", RpcTarget.All, "¡Victoria de los españoles!");
        }
        else if (pilaresVivos >= 3)
        {
            PV.RPC("RPC_FinDelJuego", RpcTarget.All, "¡Victoria de los jugadores!");
        }
    }

    [PunRPC]
    private void RPC_FinDelJuego(string mensaje)
    {
        if (juegoFinalizado) return;

        juegoFinalizado = true;
        Time.timeScale = 0f;

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            acaboJuego = true;
        }

        if (textoVictoria != null)
            textoVictoria.text = mensaje;

        if (botonVolverInicio != null)
            botonVolverInicio.gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(pilaresVivos);
            stream.SendNext(pilaresCaidos);
        }
        else
        {
            pilaresVivos = (int)stream.ReceiveNext();
            pilaresCaidos = (int)stream.ReceiveNext();
            ActualizarUI();
            RevisarCondicionesVictoria();
        }
    }

    public void RegistrarNuevoPilar(PilarSubject subject)
    {
        if (!pilaresRegistrados.Contains(subject))
        {
            pilaresRegistrados.Add(subject);
            subject.OnMuerto += RegistrarPilarMuerto;
            subject.OnRecuperado += RegistrarPilarRecuperado;
        }
    }
}


