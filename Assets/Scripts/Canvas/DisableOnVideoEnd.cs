using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DisableOnVideoEnd : MonoBehaviour
{
    public VideoPlayer videoPlayer;       // Referencia al VideoPlayer
    public GameObject uiToDisable;        // UI que quieres desactivar cuando termine el video
    public AudioSource audioSource;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Asegúrate de que el evento esté conectado
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (uiToDisable != null)
        {
            audioSource.Play();
            uiToDisable.SetActive(false);
        }
    }
}

