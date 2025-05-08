using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

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

    // Post procesamiento
    ChromaticAberration chromatic;
    Vignette vignette;
    PostProcessVolume postProcesamiento;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
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
        GameObject postFX = GameObject.Find("PostProcessing");
        if (postFX != null)
        {
            postProcesamiento = postFX.GetComponent<PostProcessVolume>();
            if (postProcesamiento != null && postProcesamiento.profile != null)
            {
                if (postProcesamiento.profile.HasSettings<ChromaticAberration>())
                {
                    chromatic = postProcesamiento.profile.GetSetting<ChromaticAberration>();
                }

                if (postProcesamiento.profile.HasSettings<Vignette>())
                {
                    vignette = postProcesamiento.profile.GetSetting<Vignette>();
                }
            }
        }
    }

    public void Saltar()
    {
        if (!PV.IsMine || animaciones == null) return;
        animaciones.SetTrigger("Jump");
    }

    public void AplicarSalto()
    {
        if (!PV.IsMine) return;
        rb.velocity = fuerzaSalto;
    }

    public void Ataque()
    {
        if (!PV.IsMine || animaciones == null) return;
        animaciones.SetTrigger("Atack");
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

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

        // Aplicar efectos post procesamiento al correr
        if (chromatic != null && vignette != null)
        {
            float velocidadCambio = 1.5f * Time.deltaTime;

            float intensidadChromaticObjetivo = (estaCorriendo && energiaActual > 0) ? 0.523f : 0f;
            float intensidadVignetteObjetivo = (estaCorriendo && energiaActual > 0) ? 0.304f : 0f;

            chromatic.intensity.value = Mathf.MoveTowards(chromatic.intensity.value, intensidadChromaticObjetivo, velocidadCambio);
            vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, intensidadVignetteObjetivo, velocidadCambio);
        }
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
}
