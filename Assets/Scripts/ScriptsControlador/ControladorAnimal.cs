using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun;

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

    public bool estaInteractuando = false;
    private Transform objetivoActual;
    private GameObject jugador;
    private Rigidbody rb;
    private GameObject puntoTemporalAnterior;

    private bool estaEsperando = false;
    public string nombreAnimal = "Jaguar";

    private IEnumerator Start()
    {
        // Esperar hasta que el jugador esté en escena
        /*while (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Jugador");
            yield return null; // espera un frame
        }*/

        if (accionInteractuar != null)
            accionInteractuar.action.Enable();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        EscogerNuevoPunto();

        yield return null;
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

        if (accionInteractuar.action.triggered)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Jugador"))
            {
                if (Vector3.Distance(transform.position, go.transform.position) <= rangoInteraccion)
                {
                    var photonView = go.GetComponent<PhotonView>();
                    if (photonView != null && photonView.IsMine)
                    {
                        var registro = go.GetComponent<RegistroInteracciones>();
                        if (registro != null && !registro.YaInteractuoCon(nombreAnimal))
                        {
                            jugador = go;
                            StartCoroutine(InteraccionConJugador());
                        }
                        else
                        {
                            Debug.Log($"[{nombreAnimal}] Ya interactuaste con este animal.");
                        }
                        break;
                    }
                }
            }
        }

        /*if (accionInteractuar.action.triggered && Vector3.Distance(transform.position, jugador.transform.position) <= rangoInteraccion)
        {
            StartCoroutine(InteraccionConJugador());
        }*/
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

        var piMenu = jugador.GetComponentInChildren<PiUI>();

        if (piMenu != null)
        {
            switch (nombreAnimal)
            {
                case "Jaguar":
                    piMenu.piData[0].isInteractable = true;
                    piMenu.piList[0].SetData(piMenu.piData[0], piMenu.innerRadius, piMenu.outerRadius, piMenu);
                    break;

                case "Mono":
                    piMenu.piData[1].isInteractable = true;
                    piMenu.piList[1].SetData(piMenu.piData[1], piMenu.innerRadius, piMenu.outerRadius, piMenu);
                    break;

                case "Rana":
                    piMenu.piData[2].isInteractable = true;
                    piMenu.piList[2].SetData(piMenu.piData[2], piMenu.innerRadius, piMenu.outerRadius, piMenu);
                    break;

                case "Tucan":
                    piMenu.piData[3].isInteractable = true;
                    piMenu.piList[3].SetData(piMenu.piData[3], piMenu.innerRadius, piMenu.outerRadius, piMenu);
                    break;
            }
        }

        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            pv.RPC("ApuntarHacia", RpcTarget.All, jugador.transform.position);
        }

        var controlPersonaje = jugador.GetComponent<ControlDePersonaje>();
        if (controlPersonaje != null)
        {
            controlPersonaje.controlesBloqueados = true;
        }

        var menuRadial = jugador.GetComponent<MenuRadial>();
        if (menuRadial != null)
        {
            menuRadial.menuBloqueado = true;
        }

        var camara = GetComponentInChildren<SecuenciaCamaraAnimal>();

        camara.IniciarSecuencia();
        
        yield return new WaitForSeconds(12f);

        controlPersonaje.controlesBloqueados = false;
        menuRadial.menuBloqueado = false;

        Transform nuevoPunto = puntosMovimiento[Random.Range(0, puntosMovimiento.Count)];
        rb.MovePosition(nuevoPunto.position);

        EscogerNuevoPunto();
        estaInteractuando = false;

        var registro = jugador.GetComponent<RegistroInteracciones>();
        if (registro != null)
        {
            registro.RegistrarInteraccion(nombreAnimal);
        }
    }

    [PunRPC]
    public void ApuntarHacia(Vector3 posicionJugador)
    {
        Vector3 direccion = posicionJugador - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion.normalized);
            transform.rotation = rotacionObjetivo;
        }

        estaInteractuando = true; // También puedes detener el movimiento si quieres
    }
}