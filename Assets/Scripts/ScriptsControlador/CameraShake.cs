using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton

    private Transform camTransform;
    private Vector3 originalPos;

    private void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        camTransform = Camera.main.transform;
        originalPos = camTransform.localPosition;
    }

    /// <summary>
    /// Llama a este método para sacudir la cámara.
    /// </summary>
    /// <param name="duration">Duración del efecto en segundos.</param>
    /// <param name="magnitude">Intensidad de la sacudida.</param>
    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines(); // Por si hay otra sacudida activa
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            camTransform.localPosition = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPos;
    }
}
