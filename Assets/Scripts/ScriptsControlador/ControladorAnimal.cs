using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControladorAnimal : MonoBehaviour
{
    [Header("Movimiento aleatorio")]
    public List<Transform> puntosMovimiento;
    public float velocidadMovimiento = 2f;
    public float tiempoEntreMovimientos = 5f;
    public float velocidadRotacion = 5f;

    [Header("Interacción")]
    public InputActionProperty accionInteractuar;
    public float rangoInteraccion = 3f;

    private bool estaInteractuando = false;
    private Transform objetivoActual;
    private GameObject jugador;
    private Rigidbody rb;
    private GameObject puntoTemporalAnterior;

    private bool estaEsperando = false;

    private IEnumerator Start()
    {
        // Esperar hasta que el jugador esté en escena
        while (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Jugador");
            yield return null; // espera un frame
        }

        if (accionInteractuar != null)
            accionInteractuar.action.Enable();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        EscogerNuevoPunto();
    }

    private void FixedUpdate()
    {
        if (estaInteractuando || objetivoActual == null || estaEsperando) return;

        Vector3 direccion = objetivoActual.position - transform.position;
        direccion.y = 0f;

        if (direccion.magnitude > 0.1f)
        {
            Vector3 nuevaPos = Vector3.MoveTowards(transform.position, objetivoActual.position, velocidadMovimiento * Time.fixedDeltaTime);
            rb.MovePosition(nuevaPos);

            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);
            Quaternion rotacionSuave = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(rotacionSuave);
        }
        else
        {
            // Llegó al punto
            StartCoroutine(EsperarYSiguientePunto());
        }
    }

    private void Update()
    {
        if (estaInteractuando) return;

        if (accionInteractuar.action.triggered && Vector3.Distance(transform.position, jugador.transform.position) <= rangoInteraccion)
        {
            StartCoroutine(InteraccionConJugador());
        }
    }

    private void EscogerNuevoPunto()
    {
        if (puntosMovimiento.Count == 0) return;

        // Destruir el punto temporal anterior si existe
        if (puntoTemporalAnterior != null)
            Destroy(puntoTemporalAnterior);

        Transform basePunto = puntosMovimiento[Random.Range(0, puntosMovimiento.Count)];
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        GameObject puntoTemporal = new GameObject("DestinoTemp");
        puntoTemporal.transform.position = basePunto.position + offset;

        puntoTemporalAnterior = puntoTemporal;
        objetivoActual = puntoTemporal.transform;
    }

    private IEnumerator EsperarYSiguientePunto()
    {
        estaEsperando = true;
        yield return new WaitForSeconds(tiempoEntreMovimientos);
        EscogerNuevoPunto();
        estaEsperando = false;
    }

    private IEnumerator InteraccionConJugador()
    {
        estaInteractuando = true;

        Debug.Log(">> Inicia secuencia de cámara...");
        yield return new WaitForSeconds(3f);

        Transform nuevoPunto = puntosMovimiento[Random.Range(0, puntosMovimiento.Count)];
        rb.MovePosition(nuevoPunto.position);

        EscogerNuevoPunto();
        estaInteractuando = false;
    }
}

