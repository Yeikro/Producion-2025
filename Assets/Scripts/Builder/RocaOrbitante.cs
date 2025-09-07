using UnityEngine;

public class RocaOrbitante : MonoBehaviour
{
    private IRocaMovimiento movimiento;
    private Transform pilarCentro;

    private void Start()
    {
        pilarCentro = transform.parent; // el pilar es el padre
    }

    public void SetMovimiento(IRocaMovimiento movimiento)
    {
        this.movimiento = movimiento;
    }

    private void Update()
    {
        if (movimiento != null)
        {
            movimiento.Mover(transform, pilarCentro);
        }
    }
}


