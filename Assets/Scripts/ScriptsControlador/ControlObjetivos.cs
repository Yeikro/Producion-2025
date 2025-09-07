using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObjetivos : MonoBehaviour
{
    public List<Transform> objetivos = new List<Transform>();
    public static ControlObjetivos singleton;

    private void Awake()
    {
        singleton = this;
    }
}
