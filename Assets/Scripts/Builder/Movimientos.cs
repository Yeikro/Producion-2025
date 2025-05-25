using UnityEngine;

public class MovimientoIzquierdaDerecha : IRocaMovimiento
{
    public void Mover(Transform rocaTransform, Transform centroPilar)
    {
        rocaTransform.RotateAround(centroPilar.position, Vector3.up, 50 * Time.deltaTime);
    }
}

public class MovimientoDerechaIzquierda : IRocaMovimiento
{
    public void Mover(Transform rocaTransform, Transform centroPilar)
    {
        rocaTransform.RotateAround(centroPilar.position, Vector3.down, 50 * Time.deltaTime);
    }
}

public class MovimientoArribaAbajo : IRocaMovimiento
{
    public void Mover(Transform rocaTransform, Transform centroPilar)
    {
        float altura = Mathf.Sin(Time.time * 2f) * 0.5f;
        rocaTransform.localPosition = new Vector3(rocaTransform.localPosition.x, altura, rocaTransform.localPosition.z);
    }
}

