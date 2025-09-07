using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Transicion : MonoBehaviour
{
    public Image imagenObjetivo;  // Imagen cuyo fillAmount es el objetivo
    public Image imagenDelay;     // Imagen cuyo fillAmount se actualizar� suavemente
    public float velocidad;       // Velocidad de la transici�n

    private void Start()
    {
        // Inicia la corutina que actualiza constantemente el fillAmount
        StartCoroutine(ActualizarFillAmountSuavizado());
    }

    // Esta corutina actualizar� el fillAmount de imagenDelay constantemente
    private IEnumerator ActualizarFillAmountSuavizado()
    {
        while (true) // Se ejecutar� constantemente
        {
            // Interpolamos suavemente entre el fillAmount actual de imagenDelay y el de imagenObjetivo
            imagenDelay.fillAmount = Mathf.Lerp(imagenDelay.fillAmount, imagenObjetivo.fillAmount, Time.deltaTime * velocidad);

            // Esperamos hasta el siguiente frame
            yield return null;
        }
    }
}
