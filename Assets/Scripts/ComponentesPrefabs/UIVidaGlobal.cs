using UnityEngine;
using UnityEngine.UI;

public class UIVidaGlobal : MonoBehaviour
{
    public static UIVidaGlobal instancia;

    public GameObject panelVidaPilar; // El panel UI superior
    public Image barraVida;           // Imagen de la barra

    private void Awake()
    {
        instancia = this;
        panelVidaPilar.SetActive(false);
    }

    public void ActivarBarra(Vida vida)
    {
        if (vida == null || vida.gameObject == null) return;

        if (panelVidaPilar == null) return;

        panelVidaPilar.SetActive(true);
        StartCoroutine(SeguirBarra(vida));
    }

    public void DesactivarBarra()
    {
        if (panelVidaPilar != null && panelVidaPilar.gameObject != null)
        {
            panelVidaPilar.SetActive(false);
        }

        StopAllCoroutines(); // por seguridad si la corrutina sigue corriendo
    }

    private System.Collections.IEnumerator SeguirBarra(Vida vida)
    {
        while (vida != null && !vida.estaMuerto && vida.gameObject != null)
        {
            barraVida.fillAmount = vida.vidaActual / vida.vidaInicial;
            yield return null;
        }
        panelVidaPilar.SetActive(false);
    }
}
