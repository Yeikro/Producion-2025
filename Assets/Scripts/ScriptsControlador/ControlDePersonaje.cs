using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;

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

    // 🏃‍♂️ Correr
    public float multiplicadorCorrer = 1.5f;
    public float energiaMaxima = 1f;
    public float energiaActual = 1f;
    public float velocidadGasto = 0.5f;
    public float velocidadRecarga = 0.3f;
    public Image barraEnergia;
    bool estaCorriendo = false;

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
    }

    public void Saltar()
    {
        if (!PV.IsMine) return;
        animaciones.SetTrigger("Jump");
    }

    public void AplicarSalto()
    {
        if (!PV.IsMine) return;
        rb.velocity = fuerzaSalto;
    }

    public void Ataque()
    {
        if (!PV.IsMine) return;
        animaciones.SetTrigger("Atack");
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        movimiento = Vector2.Lerp(movimiento, controlMover.action.ReadValue<Vector2>(), velSuavisada * Time.deltaTime);

        animaciones.SetFloat("Horizontal", movimiento.x);
        animaciones.SetFloat("Vertical", movimiento.y);
        animaciones.SetBool("Defens", controlDefender.action.ReadValue<float>() > 0.5f);
        animaciones.SetBool("Down", controlAgacharse.action.ReadValue<float>() > 0.5f);

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
}
