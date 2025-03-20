using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vida : MonoBehaviour
{
    public float vidaInicial;
    public float vidaActual;
    void Start()
    {
        vidaActual = vidaInicial;
    }

    void CausarDaño(float cuanto)
    {
        vidaActual -= cuanto;
        if (vidaActual < 0)
        {

        }
    }
}
