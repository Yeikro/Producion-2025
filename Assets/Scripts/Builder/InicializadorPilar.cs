using UnityEngine;

public class InicializadorPilar : MonoBehaviour
{
    public GameObject[] prefabsRocas; // Prefabs asignados en inspector
    public int minRocas = 2;
    public int maxRocas = 5;

    private PilarBuilder builder;

    private void Start()
    {
        builder = new PilarBuilder(prefabsRocas, transform);

        int cantidadRocas = Random.Range(minRocas, maxRocas + 1);
        builder.ConstruirPilar(cantidadRocas);
    }
}

