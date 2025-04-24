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
        Vector3 posicionRandomJugador = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), posicionRandomJugador, Quaternion.identity);
        Vector3 posicionRandom = new Vector3(Random.Range(-100f, 100f), 0f, Random.Range(-100f, 100f));
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Enemigo"), posicionRandom, Quaternion.identity);
        Debug.Log("si");
    }
}
