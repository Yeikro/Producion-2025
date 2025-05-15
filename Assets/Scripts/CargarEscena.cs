using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargarEscena : MonoBehaviour
{
    public string nombreEscena;

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
