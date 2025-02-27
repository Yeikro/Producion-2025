using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDePersonaje : MonoBehaviour
{
    public Animator animaciones;
    void Update()
    {
        animaciones.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
        animaciones.SetFloat("Vertical", Input.GetAxis("Vertical"));

        if(Input.GetKey(KeyCode.Space))
        {
            animaciones.SetTrigger("Jump");
        }
        if (Input.GetMouseButtonDown(0))
        {
            animaciones.SetTrigger("Atack");
        }
        if (Input.GetMouseButtonDown(1))
        {
            animaciones.SetTrigger("Defens");
        }
    }

}
