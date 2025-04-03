using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject[] animalPrefabs; // Lista de prefabs de animales
    public List<Transform> spawnPoints = new List<Transform>(); // Puntos de spawn
    public int numeroAnimales = 5; // Número de animales a generar
    public float distanciaMinima = 5f; // Distancia mínima entre animales
    public float rangoMovimiento = 3f; // Rango de movimiento de los animales
    public float velocidadMovimiento = 2f; // Velocidad de los animales

    private List<Transform> puntosOcupados = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < numeroAnimales; i++)
        {
            Transform selectedPoint = ObtenerPuntoAlejado();
            if (selectedPoint != null)
            {
                GameObject animalPrefab = animalPrefabs[Random.Range(0, animalPrefabs.Length)]; // Selecciona un animal aleatorio
                GameObject animal = Instantiate(animalPrefab, selectedPoint.position, Quaternion.identity);
                puntosOcupados.Add(selectedPoint);
                spawnPoints.Remove(selectedPoint);

                // Agregar script de movimiento al animal
                AnimalMover mover = animal.AddComponent<AnimalMover>();
                mover.rangoMovimiento = rangoMovimiento;
                mover.velocidadMovimiento = velocidadMovimiento;
            }
            else
            {
                Debug.LogWarning("No se encontró un punto válido para un animal.");
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
