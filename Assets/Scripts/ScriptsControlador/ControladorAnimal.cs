using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private float tiempoProximoMovimiento;

    private Rigidbody rb;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");

        if (accionInteractuar != null)
            accionInteractuar.action.Enable();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Evitar giros no deseados por colisiones
        }

        EscogerNuevoPunto();
    }

    private void FixedUpdate()
    {
        if (estaInteractuando || objetivoActual == null) return;

        // Movimiento y rotación suave
        Vector3 direccion = (objetivoActual.position - transform.position);
        direccion.y = 0f; // Mantener en plano horizontal

        if (direccion.magnitude > 0.1f)
        {
            Vector3 nuevaPos = Vector3.MoveTowards(transform.position, objetivoActual.position, velocidadMovimiento * Time.fixedDeltaTime);
            rb.MovePosition(nuevaPos);

            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);
            Quaternion rotacionSuave = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.fixedDeltaTime);
            rb.MoveRotation(rotacionSuave);
        }

        if (Vector3.Distance(transform.position, objetivoActual.position) < 0.2f)
        {
            tiempoProximoMovimiento = Time.time + tiempoEntreMovimientos;
            EscogerNuevoPunto();
        }
    }

    private void Update()
    {
        if (estaInteractuando) return;

        if (Time.time >= tiempoProximoMovimiento && objetivoActual != null)
        {
            // Deja que FixedUpdate haga el movimiento
        }

        if (accionInteractuar.action.triggered && Vector3.Distance(transform.position, jugador.transform.position) <= rangoInteraccion)
        {
            StartCoroutine(InteraccionConJugador());
        }
    }

    private void EscogerNuevoPunto()
    {
        if (puntosMovimiento.Count == 0) return;
        objetivoActual = puntosMovimiento[Random.Range(0, puntosMovimiento.Count)];
    }

    private IEnumerator InteraccionConJugador()
    {
        estaInteractuando = true;

        Debug.Log(">> Inicia secuencia de cámara...");
        yield return new WaitForSeconds(3f); // Espacio para insertar la secuencia real

        Transform nuevoPunto = puntosMovimiento[Random.Range(0, puntosMovimiento.Count)];
        rb.position = nuevoPunto.position;

        tiempoProximoMovimiento = Time.time + tiempoEntreMovimientos;
        estaInteractuando = false;
    }
}
