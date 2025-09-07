using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PilarSubject : MonoBehaviour
{
    public event Action OnMuerto;
    public event Action OnRecuperado;

    public void NotificarMuerte()
    {
        Debug.Log("PilarSubject: Notifica muerte");
        OnMuerto?.Invoke();
    }

    public void NotificarRecuperacion()
    {

        Debug.Log("PilarSubject: Notifica recuperación");
        OnRecuperado?.Invoke();
    }

}
