using System.Collections.Generic;
using UnityEngine;

public class PilarSpawner : MonoBehaviour
{
    public GameObject[] pilares;
    public List<Transform> spawnPoints = new List<Transform>();
    public float distanciaMinima = 10f;
    public int intentosMaximos = 10; // Número de intentos para encontrar un punto válido

    private List<Transform> puntosOcupados = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < pilares.Length; i++)
        {
            Transform selectedPoint = ObtenerPuntoAlejado();
            if (selectedPoint != null)
            {
                pilares[i].transform.position = selectedPoint.position;
                puntosOcupados.Add(selectedPoint);
                spawnPoints.Remove(selectedPoint);
            }
            else
            {
                Debug.LogWarning($"No se encontró un punto válido para el pilar {i + 1}. Reduciendo restricciones...");
                distanciaMinima /= 2; // Reduce la distancia mínima para evitar bloqueos
                selectedPoint = ObtenerPuntoAlejado(); // Intenta de nuevo con menor distancia
                if (selectedPoint != null)
                {
                    pilares[i].transform.position = selectedPoint.position;
                    puntosOcupados.Add(selectedPoint);
                    spawnPoints.Remove(selectedPoint);
                }
                else
                {
                    Debug.LogWarning("No se pudo colocar el pilar a pesar de reducir la distancia mínima.");
                }
            }
        }
    }

    private Transform ObtenerPuntoAlejado()
    {
        List<Transform> puntosValidos = new List<Transform>();

        foreach (Transform punto in spawnPoints)
        {
            if (EsPuntoValido(punto))
            {
                puntosValidos.Add(punto);
            }
        }

        if (puntosValidos.Count > 0)
        {
            return puntosValidos[Random.Range(0, puntosValidos.Count)];
        }
        return null;
    }

    private bool EsPuntoValido(Transform candidato)
    {
        foreach (Transform ocupado in puntosOcupados)
        {
            if (Vector3.Distance(candidato.position, ocupado.position) < distanciaMinima)
            {
                return false;
            }
        }
        return true;
    }
}
