using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrupoDePuntos
{
    public string nombreGrupo;
    public List<Transform> puntos;
    public float alturaDesdeElTerreno = 0f;
    public Color gizmoColor = Color.white;
}

public class AjustarPuntosSobreTerreno : MonoBehaviour
{
    public Terrain terreno;
    public List<GrupoDePuntos> grupos = new List<GrupoDePuntos>();

    [ContextMenu("Ajustar Todos Los Puntos")]
    public void AjustarTodosLosPuntos()
    {
        if (terreno == null)
        {
            Debug.LogError("No se ha asignado el componente Terrain.");
            return;
        }

        foreach (var grupo in grupos)
        {
            foreach (Transform punto in grupo.puntos)
            {
                if (punto == null) continue;

                Vector3 pos = punto.position;
                float alturaTerreno = terreno.SampleHeight(pos) + terreno.transform.position.y;
                punto.position = new Vector3(pos.x, alturaTerreno + grupo.alturaDesdeElTerreno, pos.z);
            }
        }

        Debug.Log("✅ Todos los puntos fueron ajustados sobre el terreno.");
    }

    private void OnDrawGizmos()
    {
        if (grupos == null) return;

        foreach (var grupo in grupos)
        {
            Gizmos.color = grupo.gizmoColor;
            foreach (Transform punto in grupo.puntos)
            {
                if (punto != null)
                    Gizmos.DrawSphere(punto.position, 0.3f);
            }
        }
    }
}

