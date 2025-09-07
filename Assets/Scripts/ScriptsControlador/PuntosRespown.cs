using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuntosRespown : MonoBehaviour
{
    public Transform[] PosEnemigos, PosPersonaje;
    public static PuntosRespown singleton;

    public Transform GetPosEnemigo()
    {
        return PosEnemigos[Random.Range(0, PosEnemigos.Length)];
    }
    public Transform GetPosPersonaje()
    {
        return PosPersonaje[Random.Range(0, PosPersonaje.Length)];
    }

    private void Awake()
    {
        singleton = this;
    }
}
