using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Transicion : MonoBehaviour
{
    public Image imagenObjetivo;  // Imagen cuyo fillAmount es el objetivo
    public Image imagenDelay;     // Imagen cuyo fillAmount se actualizará suavemente
    public float velocidad;       // Velocidad de la transición

    private void Start()
    {
        // Inicia la corutina que actualiza constantemente el fillAmount
        StartCoroutine(ActualizarFillAmountSuavizado());
    }

    // Esta corutina actualizará el fillAmount de imagenDelay constantemente
    private IEnumerator ActualizarFillAmountSuavizado()
    {
        while (true) // Se ejecutará constantemente
        {
            // Interpolamos suavemente entre el fillAmount actual de imagenDelay y el de imagenObjetivo
            imagenDelay.fillAmount = Mathf.Lerp(imagenDelay.fillAmount, imagenObjetivo.fillAmount, Time.deltaTime * velocidad);

            // Esperamos hasta el siguiente frame
            yield return null;
        }
    }
}
