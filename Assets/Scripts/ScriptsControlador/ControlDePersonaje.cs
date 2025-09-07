using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ControlDePersonaje : MonoBehaviour
{
    public Animator animaciones;
    public InputActionProperty controlMover;
    public InputActionProperty controlSalto;
    public InputActionProperty controlAtaque;
    public InputActionProperty controlDefender;
    public InputActionProperty controlAgacharse;
    public InputActionProperty controlCorrer;

    Vector2 movimiento;
    public float velSuavisada;
    public Rigidbody rb;
    public Vector3 fuerzaSalto;

    public Transform pivot;
    public float daño = 2;
    public Transform camara;
    public Transform camaraPunto;
    PhotonView PV;

    public float rangoAtaque = 3f;
    public float dañoRaycast = 5f;
    public LayerMask capaEnemigos;
    public Vida vida;
    public float defensa = 0.5f;
    public LayerMask obstaculos;
   

    //Correr
    public float multiplicadorCorrer = 1.5f;
    public float energiaMaxima = 1f;
    public float energiaActual = 1f;
    public float velocidadGasto = 0.5f;
    public float velocidadRecarga = 0.3f;
    public Image barraEnergia;
    bool estaCorriendo = false;

    public bool controlesBloqueados = false;

    // Post procesamiento
    ChromaticAberration chromatic;
    Vignette vignette;
    public Volume postProcesamiento;

    public ParticleSystem spwanParticulaJugador;
    private VolumeProfile perfilActual;

    [Header("Perfiles de habilidad")]
    public VolumeProfile jaguarProfile;
    public VolumeProfile tucanProfile;
    public VolumeProfile ranaProfile;
    public VolumeProfile monoProfile;
    public VolumeProfile perfilOriginal;
    public VolumeProfile perfilBase;
    public VolumeProfile perfilCorrer;

    private Coroutine efectoHabilidadActivo;
    private bool modoAnimalActivo = false;

    public GameObject UI;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        StartCoroutine(EnableUIAfterDelay(58f));
    }

    private void Start()
    {
        if (postProcesamiento != null)
        {
            perfilActual = perfilBase;
            postProcesamiento.profile = perfilActual;
        }

        if (PV.IsMine)
        {
            CameraManager.instance.Inicializar(transform, camaraPunto);
        }

        if (!PV.IsMine)
        {
            rb.isKinematic = true;
        }

        pivot = (new GameObject()).transform;
        camara = Camera.main.transform;

        controlMover.action.Enable();
        controlSalto.action.Enable();
        controlAtaque.action.Enable();
        controlDefender.action.Enable();
        controlAgacharse.action.Enable();
        controlCorrer.action.Enable();

        controlSalto.action.performed += _ => Saltar();
        controlAtaque.action.performed += _ => Ataque();

        CinemachineFreeLook cfl = Camera.main.GetComponent<CinemachineFreeLook>();
        if (cfl != null)
        {
            cfl.LookAt = transform;
        }

        ControlObjetivos.singleton.objetivos.Add(this.transform);

        // Obtener efectos de post procesamiento
        GameObject postFX = GameObject.Find("Global Volume");
        if (postFX != null)
        {
            postProcesamiento = postFX.GetComponent<Volume>();
            if (postProcesamiento != null && postProcesamiento.profile != null)
            {
                postProcesamiento.profile.TryGet(out chromatic);
                postProcesamiento.profile.TryGet(out vignette);
            }
        }

        ActualizarReferenciasPostProcesado();
    }

    public void Saltar()
    {
        if (controlesBloqueados || !PV.IsMine || animaciones == null) return;
        animaciones.SetTrigger("Jump");
    }

    public void AplicarSalto()
    {
        if (!PV.IsMine) return;
        rb.velocity = fuerzaSalto;
    }

    public void Ataque()
    {
        if (!PV.IsMine || animaciones == null || controlesBloqueados) return;
        animaciones.SetTrigger("Atack");
    }

    void Update()
    {
        if (!PV.IsMine || controlesBloqueados)
        {
            animaciones.SetFloat("Horizontal", 0f);
            animaciones.SetFloat("Vertical", 0f);
            animaciones.SetBool("Defens", false);
            animaciones.SetBool("Down", false);
            return;
        }

        movimiento = Vector2.Lerp(movimiento, controlMover.action.ReadValue<Vector2>(), velSuavisada * Time.deltaTime);

        if (animaciones != null)
        {
            animaciones.SetFloat("Horizontal", movimiento.x);
            animaciones.SetFloat("Vertical", movimiento.y);
            animaciones.SetBool("Defens", controlDefender.action.ReadValue<float>() > 0.5f);
            animaciones.SetBool("Down", controlAgacharse.action.ReadValue<float>() > 0.5f);
        }

        vida.cubierto = controlDefender.action.ReadValue<float>() > 0.5f;

        pivot.position = transform.position;
        pivot.forward = (pivot.position - camara.position).normalized;
        pivot.eulerAngles = new Vector3(0, pivot.eulerAngles.y, 0);

        if (movimiento.y > 0.5)
        {
            transform.forward = pivot.forward;
        }

        // Detectar Shift para correr
        estaCorriendo = controlCorrer.action.ReadValue<float>() > 0.5f && energiaActual > 0 && movimiento.y > 0.1f;

        // Aplicar correr
        float multiplicador = estaCorriendo ? multiplicadorCorrer : 1f;
        transform.position += transform.forward * movimiento.y * multiplicador * Time.deltaTime * 1f; // Ajusta el 5f a tu velocidad base si es necesario

        // Gastar energía
        if (estaCorriendo)
        {
            energiaActual -= velocidadGasto * Time.deltaTime;
            energiaActual = Mathf.Clamp01(energiaActual);
        }
        else
        {
            energiaActual += velocidadRecarga * Time.deltaTime;
            energiaActual = Mathf.Clamp01(energiaActual);
        }

        // Actualizar barra UI
        if (barraEnergia != null)
        {
            barraEnergia.fillAmount = energiaActual;
        }

        if (modoAnimalActivo)
        {
            return;
        }

        // Aplicar efectos post procesamiento al correr
        bool correr = estaCorriendo && energiaActual > 0;

        // Cambiar de perfil si es necesario
        if (correr && perfilActual != perfilCorrer)
        {
            CambiarPerfil(perfilCorrer);
        }
        else if (!correr && perfilActual != perfilBase)
        {
            CambiarPerfil(perfilBase);
        }
    }

    private void CambiarPerfil(VolumeProfile nuevoPerfil)
    {
        perfilActual = nuevoPerfil;
        postProcesamiento.profile = perfilActual;
    }

    public void Atacar()
    {
        if (!PV.IsMine) return;

        RaycastHit hit;
        Vector3 direccionAtaque = transform.forward;
        Vector3 origen = transform.position + Vector3.up;

        Debug.DrawRay(origen, direccionAtaque * rangoAtaque, Color.red, 1f);

        if (Physics.Raycast(origen, direccionAtaque, out hit, rangoAtaque, capaEnemigos))
        {
            Enemigo enemigo = hit.collider.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.PV.RPC("RPC_RecibirDaño", enemigo.PV.Owner, dañoRaycast);
                Debug.Log("¡Golpeaste a un enemigo español!");
            }
        }
    }

    public void Morir()
    {
        Invoke("Respawn", 5f);
        if (!vida.estaMuerto)
        {
            Invoke("Respawn", 5f);
        }
    }

    public void Respawn()
    {
        Transform puntoRespawn = PuntosRespown.singleton.GetPosPersonaje();
        transform.position = puntoRespawn.position;
        vida.Reiniciar();
        spwanParticulaJugador.Play();
        Debug.Log("¡Has reaparecido!");
    }

    public void BloquearAtaque(bool bloquear)
    {
        if (bloquear)
        {
            controlAtaque.action.Disable();
        }
        else
        {
            controlAtaque.action.Enable();
        }
    }

    private void ActualizarReferenciasPostProcesado()
    {
        chromatic = null;
        vignette = null;

        if (postProcesamiento != null && postProcesamiento.profile != null)
        {
            postProcesamiento.profile.TryGet(out chromatic);
            if (chromatic != null) chromatic.active = true;

            postProcesamiento.profile.TryGet(out vignette);
            if (vignette != null) vignette.active = true;
        }
    }

    public void ActivarPostProcesadoTemporal(VolumeProfile nuevoPerfil, float duracion)
    {
        if (efectoHabilidadActivo != null)
            StopCoroutine(efectoHabilidadActivo);

        modoAnimalActivo = true;
        efectoHabilidadActivo = StartCoroutine(RutinaEfectoPostProcesado(nuevoPerfil, duracion));
    }

    private IEnumerator RutinaEfectoPostProcesado(VolumeProfile nuevoPerfil, float duracion)
    {
        if (postProcesamiento == null || nuevoPerfil == null || perfilOriginal == null)
            yield break;

        // Cambiar al perfil del poder
        postProcesamiento.profile = nuevoPerfil;
        ActualizarReferenciasPostProcesado();
        postProcesamiento.enabled = true;

        float tiempo = 0f;
        float tiempoParpadeo = Mathf.Min(0.5f, duracion * 0.01f); // Parpadeo breve
        float tiempoNormal = duracion - tiempoParpadeo;

        // Fase de uso normal
        while (tiempo < tiempoNormal)
        {
            tiempo += Time.deltaTime;
            yield return null;
        }

        // Fase de parpadeo (rápida)
        float t = 0f;
        bool visible = true;
        while (t < tiempoParpadeo)
        {
            t += Time.deltaTime;
            visible = !visible;
            postProcesamiento.enabled = visible;
            yield return new WaitForSeconds(0.1f);
        }

        // Asegurar que está encendido para la transición suave
        postProcesamiento.enabled = true;

        // Restaurar el perfil original
        postProcesamiento.profile = perfilOriginal;
        modoAnimalActivo = false;
        ActualizarReferenciasPostProcesado();

        efectoHabilidadActivo = null;
    }

    IEnumerator EnableUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (UI != null)
        {
            UI.SetActive(true);
        }
    }
}
