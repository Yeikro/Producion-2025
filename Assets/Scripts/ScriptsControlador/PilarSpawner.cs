using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilarSpawner : MonoBehaviour
{
    public GameObject pillarPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public int numeroPilares = 3;
    public float distanciaMinima = 10f;
    public int intentosMaximos = 10; // Número de intentos para encontrar un punto válido

    private List<Transform> puntosOcupados = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < numeroPilares; i++)
        {
            Transform selectedPoint = ObtenerPuntoAlejado();
            if (selectedPoint != null)
            {
                Instantiate(pillarPrefab, selectedPoint.position, Quaternion.identity);
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
                    Instantiate(pillarPrefab, selectedPoint.position, Quaternion.identity);
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
