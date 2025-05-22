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

        // Aseg�rate de que el evento est� conectado
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

