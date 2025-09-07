using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlerManager : MonoBehaviour
{
    public CursorLockMode modo;
    void Start()
    {
        Cursor.lockState = modo;
    }

}
