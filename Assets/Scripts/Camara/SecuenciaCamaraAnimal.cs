using System.Collections;
using UnityEngine;
using Cinemachine;

public class SecuenciaCamaraAnimal : MonoBehaviour
{
    public CinemachineVirtualCamera CM_Contrapicado;
    public CinemachineVirtualCamera CM_OrbitaAnimal;
    public CinemachineVirtualCamera CM_PlanoGeneral;
    public CinemachineVirtualCamera CM_AcercamientoJugador;
    public CinemachineVirtualCamera CM_OrbitaJugador;
    public CinemachineVirtualCamera CM_PicadoFinal;

    [Tooltip("Referencia a la cámara principal del jugador local (si aplica)")]
    public GameObject camaraGameplay;

    private const int PRIORIDAD_ACTIVA = 20;
    private const int PRIORIDAD_INACTIVA = 10;

    void Start()
    {
        BajarPrioridades();
    }

    public void IniciarSecuencia()
    {
        StartCoroutine(Secuencia());
    }

    private IEnumerator Secuencia()
    {
        BajarPrioridades();
        CM_Contrapicado.Priority = PRIORIDAD_ACTIVA;
        yield return new WaitForSeconds(0.5f); // corta transición

        CM_OrbitaAnimal.Priority = PRIORIDAD_ACTIVA;
        yield return new WaitForSeconds(1.8f); // blend suave

        CM_PlanoGeneral.Priority = PRIORIDAD_ACTIVA;
        yield return new WaitForSeconds(1f); // corte rápido

        CM_AcercamientoJugador.Priority = PRIORIDAD_ACTIVA;
        yield return new WaitForSeconds(1.8f); // corte rápido

        CM_OrbitaJugador.Priority = PRIORIDAD_ACTIVA;
        yield return new WaitForSeconds(3f); // blend suave

        CM_PicadoFinal.Priority = PRIORIDAD_ACTIVA;
        yield return new WaitForSeconds(3f); // corte final

        // Regresa a cámara principal del jugador
        BajarPrioridades();
        if (camaraGameplay != null)
        {
            camaraGameplay.SetActive(true);
        }
    }

    private void BajarPrioridades()
    {
        CM_Contrapicado.Priority = PRIORIDAD_INACTIVA;
        CM_OrbitaAnimal.Priority = PRIORIDAD_INACTIVA;
        CM_PlanoGeneral.Priority = PRIORIDAD_INACTIVA;
        CM_AcercamientoJugador.Priority = PRIORIDAD_INACTIVA;
        CM_OrbitaJugador.Priority = PRIORIDAD_INACTIVA;
        CM_PicadoFinal.Priority = PRIORIDAD_INACTIVA;
    }
}