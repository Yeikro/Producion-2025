using System.Collections;
using UnityEngine;

public class AnimalMover : MonoBehaviour
{
    public float rangoMovimiento = 3f; // Rango de movimiento
    public float velocidadMovimiento = 2f; // Velocidad del animal

    private Vector3 destino;

    private void Start()
    {
        StartCoroutine(MoverAleatoriamente());
    }

    private IEnumerator MoverAleatoriamente()
    {
        while (true)
        {
            // Genera un nuevo destino dentro del rango
            Vector3 nuevaPosicion = new Vector3(
                transform.position.x + Random.Range(-rangoMovimiento, rangoMovimiento),
                transform.position.y,
                transform.position.z + Random.Range(-rangoMovimiento, rangoMovimiento)
            );

            destino = nuevaPosicion;

            // Moverse hacia el destino
            while (Vector3.Distance(transform.position, destino) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
                yield return null;
            }

            // Espera un tiempo antes de moverse nuevamente
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
