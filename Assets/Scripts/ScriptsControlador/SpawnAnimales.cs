using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimales : MonoBehaviour
{
    public GameObject[] animales;
    public List<Transform> spawnPoints = new List<Transform>();
    public float distanciaMinima = 10f;
    public int intentosMaximos = 10;

    private List<Transform> puntosOcupados = new List<Transform>();

    private void Start()
    {
        float distanciaActual = distanciaMinima;

        for (int i = 0; i < animales.Length; i++)
        {
            Transform selectedPoint = null;
            int intentos = 0;

            while (selectedPoint == null && intentos < intentosMaximos)
            {
                Transform candidato = spawnPoints[Random.Range(0, spawnPoints.Count)];

                if (EsPuntoValido(candidato, distanciaActual))
                {
                    selectedPoint = candidato;
                }
                intentos++;
            }

            // Si no encontró uno suficientemente alejado, reduce la distancia mínima e intenta otra vez
            if (selectedPoint == null)
            {
                Debug.LogWarning($"No se encontró un punto suficientemente alejado para el animal {i + 1}. Intentando con menor distancia.");
                distanciaActual /= 2f;

                foreach (Transform punto in spawnPoints)
                {
                    if (EsPuntoValido(punto, distanciaActual))
                    {
                        selectedPoint = punto;
                        break;
                    }
                }
            }

            if (selectedPoint != null)
            {
                animales[i].transform.position = selectedPoint.position;
                puntosOcupados.Add(selectedPoint);
                spawnPoints.Remove(selectedPoint);
            }
            else
            {
                Debug.LogWarning($"No se pudo posicionar al animal {i + 1} incluso con restricciones relajadas.");
            }
        }
    }

    private bool EsPuntoValido(Transform candidato, float distancia)
    {
        foreach (Transform ocupado in puntosOcupados)
        {
            if (Vector3.Distance(candidato.position, ocupado.position) < distancia)
            {
                return false;
            }
        }
        return true;
    }
}
