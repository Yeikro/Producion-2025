using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineFreeLook cameraFreeLook;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void Inicializar(Transform seguir, Transform mirar)
    {
        cameraFreeLook.Follow = seguir;
        cameraFreeLook.LookAt = mirar;
    }
}
