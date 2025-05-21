using UnityEngine;
using System.Collections;

public class BlendShapeController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material material;

    [ContextMenu("Activar 0")]
    public void A0()
    {
        ActivateBlendShape(0);
    }

    [ContextMenu("Activar 1")]
    public void A1()
    {
        ActivateBlendShape(1);
    }

    [ContextMenu("Activar 2")]
    public void A2()
    {
        ActivateBlendShape(2);
    }

    [ContextMenu("Activar 3")]
    public void A3()
    {
        ActivateBlendShape(3);
    }

    [ContextMenu("Desactiva")]
    public void Desactivar()
    {
        StartCoroutine(TransitionDesactivar());
    }


    private void Start()
    {
        material=skinnedMeshRenderer.sharedMaterial;
        skinnedMeshRenderer.material=material;
    }
    public void ActivateBlendShape(int shapeIndex)
    {
        StartCoroutine(TransitionBlendShape(shapeIndex));
    }

    private IEnumerator TransitionBlendShape(int targetIndex)
    {
        int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        float duration = 2f;
        float elapsed = 0f;

        Material material = skinnedMeshRenderer.material;
        float startImagensita = material.GetFloat("_Imagensita");

        float[] startWeights = new float[blendShapeCount];
        for (int i = 0; i < blendShapeCount; i++)
        {
            startWeights[i] = skinnedMeshRenderer.GetBlendShapeWeight(i);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Interpolación del blend shape
            for (int i = 0; i < blendShapeCount; i++)
            {
                float targetWeight = (i == targetIndex) ? 100f : 0f;
                float newWeight = Mathf.Lerp(startWeights[i], targetWeight, t);
                skinnedMeshRenderer.SetBlendShapeWeight(i, newWeight);
            }

            // Interpolación de _Imagensita
            float newImagensita = Mathf.Lerp(startImagensita, 1f, t);
            material.SetFloat("_Imagensita", newImagensita);

            yield return null;
        }

        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, i == targetIndex ? 100f : 0f);
        }

        material.SetFloat("_Imagensita", 1f);
    }

    private IEnumerator TransitionDesactivar()
    {
        int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        float duration = 2f;
        float elapsed = 0f;

        Material material = skinnedMeshRenderer.material;
        float startImagensita = material.GetFloat("_Imagensita");

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
                float newWeight = Mathf.Lerp(startWeights[i], 0f, t);
                skinnedMeshRenderer.SetBlendShapeWeight(i, newWeight);
            }

            float newImagensita = Mathf.Lerp(startImagensita, 0f, t);
            material.SetFloat("_Imagensita", newImagensita);

            yield return null;
        }

        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0f);
        }

        material.SetFloat("_Imagensita", 0f);
    }
}
