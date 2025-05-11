using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractuarAnimal : MonoBehaviour
{
    public Text textoInteraccion;
    public string nombreAnimal = "Jaguar";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jugador"))
        {
            textoInteraccion.gameObject.SetActive(true);
            textoInteraccion.text = $"Interactuar 'F' con el {nombreAnimal}";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Jugador"))
        {
            textoInteraccion.gameObject.SetActive(false);
        }
    }
}
