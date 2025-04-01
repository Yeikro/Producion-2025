using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Cinemachine;

public class ControlDePersonaje : MonoBehaviour
{
    public Animator animaciones;
    public InputActionProperty controlMover;
    public InputActionProperty controlSalto;
    public InputActionProperty controlAtaque;
    public InputActionProperty controlDefender;
    public InputActionProperty controlAgacharse;
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
        controlSalto.action.performed += _ => Saltar();
        controlAtaque.action.performed += _ => Ataque();
        CinemachineFreeLook cfl = Camera.main.GetComponent<CinemachineFreeLook>();
        if (cfl!=null)
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

        animaciones.SetBool("Defens", controlDefender.action.ReadValue<float>()>0.5f);
        animaciones.SetBool("Down" , controlAgacharse.action.ReadValue<float>()>0.5f);

        pivot.position = transform.position;
        pivot.forward = (pivot.position - camara.position).normalized;
        pivot.eulerAngles = new Vector3(0, pivot.eulerAngles.y, 0);

        if (movimiento.y > 0.5)
        {
            transform.forward = pivot.forward;
        }
    }

    public void Atacar()
    {
        //Personaje.personajeLocal.vida.CausasDaño(daño);
        if (!PV.IsMine) return;

        // Crear un raycast desde el personaje hacia adelante
        RaycastHit hit;
        Vector3 direccionAtaque = transform.forward;
        Vector3 origen = transform.position + Vector3.up; // Ajusta la altura según necesites

        Debug.DrawRay(origen, direccionAtaque * rangoAtaque, Color.red, 1f);

        if (Physics.Raycast(origen, direccionAtaque, out hit, rangoAtaque, capaEnemigos))
        {
            // Verificar si el objeto golpeado es un enemigo español
            Enemigo enemigo = hit.collider.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                // Aplicar daño al enemigo
                enemigo.RecibirDaño(dañoRaycast);
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

        // Obtiene una posición aleatoria de respawn
        Transform puntoRespawn = PuntosRespown.singleton.GetPosPersonaje();
        transform.position = puntoRespawn.position;

        vida.Reiniciar();

        Debug.Log("¡Has reaparecido!");
    }
}
