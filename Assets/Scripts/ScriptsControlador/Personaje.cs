using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personaje : MonoBehaviour
{
    public static Personaje personajeLocal;

    public Vida vida;
    void Awake()
    {
        if (personajeLocal == null)
        {
            personajeLocal = this;
        }
        else
        {
            //DestroyImmediate(this.gameObject);
        }
    }
}
