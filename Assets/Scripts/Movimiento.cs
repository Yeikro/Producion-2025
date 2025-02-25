using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public float rotationSpeed = 700f;  
    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        // Obtener la entrada de teclado para movimiento
        float moveX = Input.GetAxis("Horizontal");  
        float moveZ = Input.GetAxis("Vertical");    

        // Crear un vector de movimiento
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed * Time.deltaTime;

        // Mover el personaje aplicando el movimiento al Rigidbody
        rb.MovePosition(transform.position + movement);

        // Rotación del personaje hacia la dirección del movimiento
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}


