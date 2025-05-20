using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectGroup
{
    public List<GameObject> objects;
}

public class GroupedActivator : MonoBehaviour
{
    [Tooltip("Lista de grupos de GameObjects que se activar�n uno a uno.")]
    public List<GameObjectGroup> objectGroups = new List<GameObjectGroup>();

    public int indice;
    public BlendShapeController blendController;

    private Coroutine currentCoroutine;

    /// <summary>
    /// Llama este m�todo para activar un grupo por su �ndice.
    /// </summary>
    /// <param name="groupIndex">�ndice del grupo que deseas activar.</param>
    public void ActivateGroup(int groupIndex)
    {

        print("Se activo el indigena ----- " + groupIndex);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        if (groupIndex ==0)
        {
            blendController.Desactivar();
        }
        else
        {
            blendController.ActivateBlendShape(groupIndex-1);
        }


        currentCoroutine = StartCoroutine(ActivateGroupCoroutine(groupIndex));
    }
    [ContextMenu("Activar")]
    public void Activar()
    {
        ActivateGroup(indice);
    }

    private IEnumerator ActivateGroupCoroutine(int groupIndex)
    {
        // Desactiva todos los objetos de todos los grupos
        foreach (var group in objectGroups)
        {
            foreach (var obj in group.objects)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        // Validaci�n de �ndice
        if (groupIndex < 0 || groupIndex >= objectGroups.Count)
        {
            yield break;
        }

        // Activa objetos del grupo seleccionado, uno a uno
        foreach (var obj in objectGroups[groupIndex].objects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
