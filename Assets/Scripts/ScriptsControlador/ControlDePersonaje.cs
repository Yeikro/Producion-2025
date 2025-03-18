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
    public Transform camara;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
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
    }
    public void Saltar()
    {
        animaciones.SetTrigger("Jump");
    }
    public void AplicarSalto()
    {
        rb.velocity = fuerzaSalto;

    }
    public void Ataque()
    {
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

}
