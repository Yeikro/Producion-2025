using System.Collections.Generic;
using UnityEngine;

public class RegistroInteracciones : MonoBehaviour
{
    private HashSet<string> animalesInteractuados = new HashSet<string>();

    public bool YaInteractuoCon(string nombreAnimal)
    {
        return animalesInteractuados.Contains(nombreAnimal);
    }

    public void RegistrarInteraccion(string nombreAnimal)
    {
        animalesInteractuados.Add(nombreAnimal);
    }
}