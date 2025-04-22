using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


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

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
                Time.timeScale = 1f; // reanudar el tiempo si se pausó
                StartCoroutine(VolverAlInicio());
            });
        }
    }

    private IEnumerator VolverAlInicio()
    {
        PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        // Destruye todos los objetos marcados como DontDestroyOnLoad si los tienes
        foreach (var go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.scene.name == null || go.scene.name == "") // está fuera de la escena actual
            {
                Destroy(go);
            }
        }

        SceneManager.LoadScene("online");
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

        if (pilaresCaidos >= 3)
        {
            FinDelJuego("¡Victoria de los españoles!");
        }
        else if (pilaresVivos >= 1)
        {
            FinDelJuego("¡Victoria de los jugadores!");
        }
    }

    private void FinDelJuego(string mensaje)
    {
        juegoFinalizado = true;
        Time.timeScale = 0f;

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
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
        }
    }
}
