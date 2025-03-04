using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private void Start()
    {
        controlMover.action.Enable();
        controlSalto.action.Enable();
        controlAtaque.action.Enable();
        controlDefender.action.Enable();
        controlAgacharse.action.Enable();
        controlSalto.action.performed += _ => Saltar();
        controlAtaque.action.performed += _ => Ataque();
        

    }
    public void Saltar()
    {
        animaciones.SetTrigger("Jump");
    }
    public void Ataque()
    {
        animaciones.SetTrigger("Atack");
    }
    

    void Update()
    {
        movimiento = Vector2.Lerp(movimiento, controlMover.action.ReadValue<Vector2>(), velSuavisada * Time.deltaTime);

        animaciones.SetFloat("Horizontal", movimiento.x);
        animaciones.SetFloat("Vertical", movimiento.y);

        animaciones.SetBool("Defens", controlDefender.action.ReadValue<float>()>0.5f);
        animaciones.SetBool("Down" , controlAgacharse.action.ReadValue<float>()>0.5f);


    }

}
