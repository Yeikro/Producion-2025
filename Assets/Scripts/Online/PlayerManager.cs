using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            StartCoroutine(CreateController());
        }
    }

    IEnumerator CreateController()
    {
        GameObject spawnJugadores = GameObject.Find("Spawn Jugadores");
        if (spawnJugadores == null)
        {
            Debug.LogError("No se encontró el GameObject llamado 'Spawn Jugadores'");
            yield break;
        }

        Vector3 centro = spawnJugadores.transform.position;
        Vector3 offset = Random.insideUnitSphere * 5f;
        offset.y = 0f;
        Vector3 posicionJugador = centro + offset;

        GameObject jugador = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), posicionJugador, Quaternion.identity);
        if (jugador.GetComponent<PhotonView>().IsMine)
        {
            jugador.tag = "Jugador";
        }

        yield return new WaitForSeconds(0.1f); // Espera para asegurar que el jugador se instancie

        GameObject[] puntosSpawnEnemigos = GameObject.FindGameObjectsWithTag("SpawnEnemigo");
        if (puntosSpawnEnemigos.Length == 0)
        {
            Debug.LogError("No se encontraron puntos de spawn para enemigos con tag 'SpawnEnemigo'");
            yield break;
        }

        int indiceAleatorio = Random.Range(0, puntosSpawnEnemigos.Length);
        Vector3 posicionEnemigo = puntosSpawnEnemigos[indiceAleatorio].transform.position;

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Enemigo"), posicionEnemigo, Quaternion.identity);

        Debug.Log("Jugador y enemigo instanciados correctamente.");
    }
}

