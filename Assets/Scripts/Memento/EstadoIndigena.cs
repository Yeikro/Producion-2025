using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perfil", menuName = "Indigena/Estado")]
public class EstadoIndigena : ScriptableObject
{
    public Vector3 fuerzaSalto;
    public float dañoRaycast;
    public float multiplicadorCorrer;
    public float velocidadGasto;
    public float velocidadRecarga;
    public float rangoAtaque;
    public float cobertura;
    public float gravedad;
    public Texture2D textura;
}
