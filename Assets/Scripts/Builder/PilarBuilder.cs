using UnityEngine;

public class PilarBuilder
{
    private GameObject[] prefabsRocas;
    private Transform pilarTransform;

    public PilarBuilder(GameObject[] prefabsRocas, Transform pilarTransform)
    {
        this.prefabsRocas = prefabsRocas;
        this.pilarTransform = pilarTransform;
    }

    public void ConstruirPilar(int cantidadRocas)
    {
        for (int i = 0; i < cantidadRocas; i++)
        {
            GameObject prefabRoca = prefabsRocas[Random.Range(0, prefabsRocas.Length)];
            GameObject roca = GameObject.Instantiate(prefabRoca, pilarTransform);

            float angulo = i * (360f / cantidadRocas);

            float altura = Random.Range(10f, 17f); 
            float radio = Random.Range(4f, 6f); 

            Vector3 posicion = new Vector3(
                pilarTransform.position.x + Mathf.Cos(angulo * Mathf.Deg2Rad) * radio,
                altura,
                pilarTransform.position.z + Mathf.Sin(angulo * Mathf.Deg2Rad) * radio
            );

            roca.transform.position = posicion;

            RocaOrbitante rocaOrbitante = roca.GetComponent<RocaOrbitante>();
            if (rocaOrbitante != null)
            {
                int tipoMovimiento = Random.Range(0, 3);
                switch (tipoMovimiento)
                {
                    case 0:
                        rocaOrbitante.SetMovimiento(new MovimientoIzquierdaDerecha());
                        break;
                    case 1:
                        rocaOrbitante.SetMovimiento(new MovimientoDerechaIzquierda());
                        break;
                    case 2:
                        rocaOrbitante.SetMovimiento(new MovimientoArribaAbajo());
                        break;
                }
            }
        }
    }

}


