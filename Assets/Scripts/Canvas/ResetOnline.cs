using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ResetOnline : MonoBehaviour
{
    void Start()
    {
        try
        {
            Destroy(GameObject.Find("RoomManager"));
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void ResetearOnline()
    {
        SceneManager.LoadScene("online");
    }
}
