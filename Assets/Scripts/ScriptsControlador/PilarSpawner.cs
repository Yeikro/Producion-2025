using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilarSpawner : MonoBehaviour
{
    public GameObject pillarPrefab;
    public List<Transform> spawnPoints = new List<Transform>();

    public int numeroPilares = 3;

    private void Start()
    {
        for (int i = 0; i < numeroPilares; i++)
        {
            if (spawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Transform selectedPoint = spawnPoints[randomIndex];

                Instantiate(pillarPrefab, selectedPoint.position, Quaternion.identity);

                spawnPoints.RemoveAt(randomIndex);
            }
            else
            {
                Debug.LogWarning("No hay punto de spawn disponibles.");
            }
        }
    }
}
