using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Vida))]

public class Pilar : MonoBehaviour
{
    Vida vida;
    IEnumerator Start()
    {
        vida = GetComponent<Vida>();
        yield return null;
        yield return null;
        vida.vidaActual *= 0.5f;
        vida.ActualizarInterfaz();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
