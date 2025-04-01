using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Vida : MonoBehaviour
{
    public float vidaInicial;
    public float vidaActual;
    public UnityEvent eventoMorir;
    public bool estaMuerto = false;

    void Start()
    {
        Reiniciar();
    }


    public void CausasDaño(float cuanto)
    {
        vidaActual -= cuanto;
        if (vidaActual < 0)
        {
            print("Muerto!! ->" + gameObject.name);
            eventoMorir.Invoke();
            estaMuerto= true;
        }
    }

    public void Reiniciar()
    {
        vidaActual = vidaInicial;
        estaMuerto= false;
    }

}
