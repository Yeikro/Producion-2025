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
            CreateController();
        }
    }

    void CreateController()
    {
        // Buscar el GameObject llamado "Spawn Jugadores"
        GameObject spawnJugadores = GameObject.Find("Spawn Jugadores");
        if (spawnJugadores == null)
        {
            Debug.LogError("No se encontró el GameObject llamado 'Spawn Jugadores'");
            return;
        }

        // Posicionar jugador cerca del objeto Spawn Jugadores (radio de 5 unidades)
        Vector3 centro = spawnJugadores.transform.position;
        Vector3 offset = Random.insideUnitSphere * 5f;
        offset.y = 0f; // mantener en el plano horizontal
        Vector3 posicionJugador = centro + offset;

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), posicionJugador, Quaternion.identity);

        // Buscar puntos de spawn para enemigos
        GameObject[] puntosSpawnEnemigos = GameObject.FindGameObjectsWithTag("SpawnEnemigo");
        if (puntosSpawnEnemigos.Length == 0)
        {
            Debug.LogError("No se encontraron puntos de spawn para enemigos con tag 'SpawnEnemigo'");
            return;
        }

        // Seleccionar un punto aleatorio del arreglo
        int indiceAleatorio = Random.Range(0, puntosSpawnEnemigos.Length);
        Vector3 posicionEnemigo = puntosSpawnEnemigos[indiceAleatorio].transform.position;

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Enemigo"), posicionEnemigo, Quaternion.identity);

        Debug.Log("Jugador y enemigo instanciados correctamente.");
    }
}

