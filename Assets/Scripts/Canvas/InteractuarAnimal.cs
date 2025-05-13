using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class InteractuarAnimal : MonoBehaviour
{
    public Text textoInteraccion;
    public string nombreAnimal = "Jaguar";
    public ControladorAnimal controladorAnimal;
    private bool jugadorEnRango = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                jugadorEnRango = true;

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
                jugadorEnRango = false;
                textoInteraccion.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!jugadorEnRango) return;

        GameObject localJugador = GetLocalPlayer();
        if (localJugador != null && textoInteraccion.gameObject.activeSelf)
        {
            var registro = localJugador.GetComponent<RegistroInteracciones>();
            if (registro != null && registro.YaInteractuoCon(nombreAnimal))
            {
                textoInteraccion.gameObject.SetActive(false);
            }
        }
    }

    private GameObject GetLocalPlayer()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Jugador"))
        {
            var pv = go.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                return go;
            }
        }
        return null;
    }
}