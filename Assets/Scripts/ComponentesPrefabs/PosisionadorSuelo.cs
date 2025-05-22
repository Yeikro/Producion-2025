using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosisionadorSuelo : MonoBehaviour
{
    [ContextMenu("Posisionar")]
    public void posisionar()
    {
        Ray ray = new Ray(transform.position+Vector3.up * 500, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            transform.position = hit.point;
        }
    }
}
