using UnityEngine;
using System.Collections;

public class BlendShapeController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    /// <summary>
    /// Llama esta función para iniciar la transición a un blend shape específico.
    /// </summary>
    /// <param name="shapeIndex">Índice del blend shape que quieres activar (0 a n).</param>
    /// 

    [ContextMenu("Activar 0")]
    public void A0()
    {
        ActivateBlendShape(0);//mono

    }

    [ContextMenu("Activar 1")]
    public void A1()
    {
        ActivateBlendShape(1);//Rana
    }

    [ContextMenu("Activar 2")]
    public void A2()
    {
        ActivateBlendShape(2);//Jaguar
    }

    [ContextMenu("Activar 3")]
    public void A3()
    {
        ActivateBlendShape(3);//Tucan
    }

    [ContextMenu("Desactiva")]
    public void Desactivar()
    {
        StartCoroutine(TransitionDesactivar());
    }
    public void ActivateBlendShape(int shapeIndex)
    {
        StartCoroutine(TransitionBlendShape(shapeIndex));
    }

    private IEnumerator TransitionBlendShape(int targetIndex)
    {
        int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        float duration = 1f;
        float elapsed = 0f;

        // Captura los pesos iniciales
        float[] startWeights = new float[blendShapeCount];
        for (int i = 0; i < blendShapeCount; i++)
        {
            startWeights[i] = skinnedMeshRenderer.GetBlendShapeWeight(i);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            for (int i = 0; i < blendShapeCount; i++)
            {
                float targetWeight = (i == targetIndex) ? 100f : 0f;
                float newWeight = Mathf.Lerp(startWeights[i], targetWeight, t);
                skinnedMeshRenderer.SetBlendShapeWeight(i, newWeight);
            }

            yield return null;
        }

        // Asegura que al final los valores estén perfectamente definidos
        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, i == targetIndex ? 100f : 0f);
        }
    }



    private IEnumerator TransitionDesactivar()
    {
        int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        float duration = 1f;
        float elapsed = 0f;

        // Captura los pesos iniciales
        float[] startWeights = new float[blendShapeCount];
        for (int i = 0; i < blendShapeCount; i++)
        {
            startWeights[i] = skinnedMeshRenderer.GetBlendShapeWeight(i);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            for (int i = 0; i < blendShapeCount; i++)
            {
                float targetWeight = 0f;
                float newWeight = Mathf.Lerp(startWeights[i], targetWeight, t);
                skinnedMeshRenderer.SetBlendShapeWeight(i, newWeight);
            }

            yield return null;
        }

        // Asegura que al final los valores estén perfectamente definidos
        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
        }
    }
}
