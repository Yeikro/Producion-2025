using UnityEngine;
using UnityEngine.InputSystem; // NUEVO
using UnityEngine.UI;
using System.Collections;
using Cinemachine;

public class MenuRadial : MonoBehaviour
{
    private CinemachineFreeLook freeLookCam;
    private float originalXSpeed;
    private float originalYSpeed;
    private bool sliceJaguarPendienteHabilitar = false;

    [SerializeField]
    PiUIManager piUi;
    public PiUI piMenu;
    public Vida vida;

    private PiUI normalMenu;
    public InputActionProperty menuRadial;
    public ControlDePersonaje controlDePersonaje;
    public float duracionPoderJaguar = 5f;
    public float duracionPoder = 15f;
    public float enfriamientoHabilidad = 10f;
    public bool inmune = false;
    public CustomGravity customGravity;

    [Header("UI")]
    public Image barraDuracionHabilidad;
    public GameObject barraHabilidad;

    // Valores originales
    public EstadoIndigena estadoOriginal;
    public EstadoIndigena[] estadosTransformaciones;

    public bool menuBloqueado = false;

    private Photon.Pun.PhotonView photonView;

    public ParticleSystem particulasGeneral;
    public ParticleSystem particulasJaguar;
    public ParticleSystem particulasTucan;
    public ParticleSystem particulasRana;
    public ParticleSystem particulasMono;

    public GroupedActivator groupedActivator;

    private void Awake()
    {
        photonView = GetComponent<Photon.Pun.PhotonView>();
        menuRadial.action.Enable();
        menuRadial.action.performed += OnOpenMenu;
        freeLookCam = FindFirstObjectByType<CinemachineFreeLook>();
        customGravity = GetComponent<CustomGravity>();
        if (freeLookCam != null)
        {
            originalXSpeed = freeLookCam.m_XAxis.m_MaxSpeed;
            originalYSpeed = freeLookCam.m_YAxis.m_MaxSpeed;
        }
        estadoOriginal = new EstadoIndigena();
        estadoOriginal.fuerzaSalto = controlDePersonaje.fuerzaSalto;
        estadoOriginal.dañoRaycast = controlDePersonaje.dañoRaycast;
        estadoOriginal.multiplicadorCorrer = controlDePersonaje.multiplicadorCorrer;
        estadoOriginal.velocidadGasto = controlDePersonaje.velocidadGasto;
        estadoOriginal.velocidadRecarga = controlDePersonaje.velocidadRecarga;
        estadoOriginal.rangoAtaque = controlDePersonaje.rangoAtaque;
        estadoOriginal.cobertura = vida.cobertura;
        estadoOriginal.gravedad = customGravity.gravityScale;
        inmune = false;
        for (int i = 0; i < piMenu.piData.Length; i++)
        {
            piMenu.piData[i].isInteractable = false;
            //piMenu.piList[i].SetData(piMenu.piData[i], piMenu.innerRadius, piMenu.outerRadius, piMenu);
        }
    }

    private void OnDisable()
    {
        menuRadial.action.performed -= OnOpenMenu;
        menuRadial.action.Disable();  
    }

    void Start()
    {
        normalMenu = piUi.GetPiUIOf("Normal Menu");
    }

    void Update()
    {
        // Puedes dejar el resto del update para joystick u otros propósitos
        normalMenu.joystickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        normalMenu.joystickButton = Input.GetButtonDown("Fire1");
        if (!normalMenu.joystickButton)
        {
            normalMenu.joystickButton = Input.GetButtonUp("Fire1");
        }
    }

    //Esta función será llamada cuando presiones "E"
    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || menuBloqueado)
            return;

        bool isOpen = piUi.PiOpened("Normal Menu");

        if (!isOpen)
        {
            if (sliceJaguarPendienteHabilitar)
            {
                sliceJaguarPendienteHabilitar = false;
                piMenu.piList[0].FadeInAndEnable(1.5f);
            }

            controlDePersonaje.BloquearAtaque(true);

            int i = 0;
            foreach (PiUI.PiData data in normalMenu.piData)
            {
                if (string.IsNullOrWhiteSpace(data.sliceLabel))
                {
                    data.sliceLabel = "Slice " + i.ToString();
                }
                i++;
            }

            piUi.UpdatePiMenu("Normal Menu");

            if (freeLookCam != null)
            {
                freeLookCam.m_XAxis.m_MaxSpeed = 0;
                freeLookCam.m_YAxis.m_MaxSpeed = 0;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (freeLookCam != null)
            {
                freeLookCam.m_XAxis.m_MaxSpeed = originalXSpeed;
                freeLookCam.m_YAxis.m_MaxSpeed = originalYSpeed;
            }

            controlDePersonaje.BloquearAtaque(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        piUi.ChangeMenuState("Normal Menu", new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    public void Jaguar()
    {
        RestaurarTodosLosValores();
        piUi.ChangeMenuState("Normal Menu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (freeLookCam != null)
        {
            freeLookCam.m_XAxis.m_MaxSpeed = originalXSpeed;
            freeLookCam.m_YAxis.m_MaxSpeed = originalYSpeed;
        }

        piMenu.piData[0].isInteractable = false;
        piMenu.piList[0].SetData(piMenu.piData[0], piMenu.innerRadius, piMenu.outerRadius, piMenu);
        controlDePersonaje.BloquearAtaque(false);
        ActivarPoderJaguar();
        Debug.Log("Jaguar");
    }

    public void Tucan()
    {
        RestaurarTodosLosValores();
        piUi.ChangeMenuState("Normal Menu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (freeLookCam != null)
        {
            freeLookCam.m_XAxis.m_MaxSpeed = originalXSpeed;
            freeLookCam.m_YAxis.m_MaxSpeed = originalYSpeed;
        }

        piMenu.piData[3].isInteractable = false;
        piMenu.piList[3].SetData(piMenu.piData[3], piMenu.innerRadius, piMenu.outerRadius, piMenu);
        controlDePersonaje.BloquearAtaque(false);
        ActivarPoderTucan();
        Debug.Log("Tucan");
    }

    public void Rana()
    {
        RestaurarTodosLosValores();
        piUi.ChangeMenuState("Normal Menu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (freeLookCam != null)
        {
            freeLookCam.m_XAxis.m_MaxSpeed = originalXSpeed;
            freeLookCam.m_YAxis.m_MaxSpeed = originalYSpeed;
        }

        piMenu.piData[2].isInteractable = false;
        piMenu.piList[2].SetData(piMenu.piData[2], piMenu.innerRadius, piMenu.outerRadius, piMenu);
        controlDePersonaje.BloquearAtaque(false);
        ActivarPoderRana();
        Debug.Log("Rana");
    }

    public void Mono()
    {
        RestaurarTodosLosValores();
        piUi.ChangeMenuState("Normal Menu");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (freeLookCam != null)
        {
            freeLookCam.m_XAxis.m_MaxSpeed = originalXSpeed;
            freeLookCam.m_YAxis.m_MaxSpeed = originalYSpeed;
        }

        piMenu.piData[1].isInteractable = false;
        piMenu.piList[1].SetData(piMenu.piData[1], piMenu.innerRadius, piMenu.outerRadius, piMenu);
        controlDePersonaje.BloquearAtaque(false);
        ActivarPoderMono();
        Debug.Log("Rana");
    }

    public void ActivarPoderJaguar()
    {
        // Aplicar los cambios
        particulasGeneral.Play();
        particulasJaguar.Play();
        ActivarPoder(estadosTransformaciones[0]);
        barraHabilidad.SetActive(true);
        controlDePersonaje.ActivarPostProcesadoTemporal(controlDePersonaje.jaguarProfile, duracionPoderJaguar);
        StartCoroutine(MostrarBarraDeDuracion(duracionPoderJaguar));
        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesJaguar());
        groupedActivator.ActivateGroup(3);
    }

    public void ActivarPoderTucan()
    {
        // Aplicar los cambios
        particulasGeneral.Play();
        particulasTucan.Play();
        ActivarPoder(estadosTransformaciones[1]);
        barraHabilidad.SetActive(true);
        controlDePersonaje.ActivarPostProcesadoTemporal(controlDePersonaje.tucanProfile, duracionPoder);
        StartCoroutine(MostrarBarraDeDuracion(duracionPoder));
        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesTucan());
        groupedActivator.ActivateGroup(4);
    }

    public void ActivarPoderRana()
    {
        // Aplicar los cambios
        particulasGeneral.Play();
        particulasRana.Play();
        ActivarPoder(estadosTransformaciones[2]);
        barraHabilidad.SetActive(true);
        controlDePersonaje.ActivarPostProcesadoTemporal(controlDePersonaje.ranaProfile, duracionPoder);
        StartCoroutine(MostrarBarraDeDuracion(duracionPoder));
        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesRana());
        groupedActivator.ActivateGroup(2);
    }

    public void ActivarPoderMono()
    {
        inmune = true;

        vida.cubierto = true;

        if(vida.cubierto)
        {
            particulasGeneral.Play();
            particulasMono.Play();
            ActivarPoder(estadosTransformaciones[3]);
            barraHabilidad.SetActive(true);
            controlDePersonaje.ActivarPostProcesadoTemporal(controlDePersonaje.monoProfile, duracionPoder);
            StartCoroutine(MostrarBarraDeDuracion(duracionPoder));
        }

        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesMono());
        groupedActivator.ActivateGroup(1);
    }

    private IEnumerator RestaurarValoresOriginalesJaguar()
    {
        yield return new WaitForSeconds(duracionPoderJaguar);

        // Restaurar valores originales
        groupedActivator.ActivateGroup(0);
        particulasGeneral.Stop();
        particulasJaguar.Stop();
        barraHabilidad.SetActive(false);
        ActivarPoder(estadoOriginal);
        StartCoroutine(HabilitarConFadeJaguar(piMenu.piList[0], enfriamientoHabilidad));
        
    }
    private IEnumerator RestaurarValoresOriginalesTucan()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        groupedActivator.ActivateGroup(0);
        particulasGeneral.Stop();
        particulasTucan.Stop();
        barraHabilidad.SetActive(false);
        ActivarPoder(estadoOriginal);

        StartCoroutine(HabilitarConFadeTucan(piMenu.piList[3], enfriamientoHabilidad));

    }

    private IEnumerator RestaurarValoresOriginalesRana()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        groupedActivator.ActivateGroup(0);
        particulasGeneral.Stop();
        particulasRana.Stop();
        barraHabilidad.SetActive(false);
        ActivarPoder(estadoOriginal);

        StartCoroutine(HabilitarConFadeRana(piMenu.piList[2], enfriamientoHabilidad));
    }

    private IEnumerator RestaurarValoresOriginalesMono()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        groupedActivator.ActivateGroup(0);
        particulasGeneral.Stop();
        particulasMono.Stop();
        barraHabilidad.SetActive(false);
        vida.cubierto = false;
        ActivarPoder(estadoOriginal);
        inmune = false;

        StartCoroutine(HabilitarConFadeMono(piMenu.piList[1], enfriamientoHabilidad));
    }

    private IEnumerator HabilitarConFadeJaguar(PiPiece slice, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (slice.gameObject.activeInHierarchy)
        {
            slice.FadeInAndEnable(1.5f);
        }
        else
        {
            sliceJaguarPendienteHabilitar = true;
        }

        piMenu.piData[0].isInteractable = true;
    }

    private IEnumerator HabilitarConFadeTucan(PiPiece slice, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (slice.gameObject.activeInHierarchy)
        {
            slice.FadeInAndEnable(1.5f);
        }
        else
        {
            sliceJaguarPendienteHabilitar = true;
        }

        piMenu.piData[3].isInteractable = true;
    }

    private IEnumerator HabilitarConFadeRana(PiPiece slice, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (slice.gameObject.activeInHierarchy)
        {
            slice.FadeInAndEnable(1.5f);
        }
        else
        {
            sliceJaguarPendienteHabilitar = true;
        }

        piMenu.piData[2].isInteractable = true;
    }

    private IEnumerator HabilitarConFadeMono(PiPiece slice, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (slice.gameObject.activeInHierarchy)
        {
            slice.FadeInAndEnable(1.5f);
        }
        else
        {
            sliceJaguarPendienteHabilitar = true;
        }

        piMenu.piData[1].isInteractable = true;
    }

    public void RestaurarTodosLosValores()
    {
        ActivarPoder(estadoOriginal);
        particulasGeneral.Stop();
        particulasJaguar.Stop();
        particulasTucan.Stop();
        particulasMono.Stop();
        particulasRana.Stop();
       
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hey get off of me!");
    }

    public void OnHoverExit()
    {
        Debug.Log("That's right and dont come back!");
    }

    private IEnumerator MostrarBarraDeDuracion(float duracion)
    {
        if (barraDuracionHabilidad == null)
            yield break;

        barraDuracionHabilidad.fillAmount = 1f;

        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            barraDuracionHabilidad.fillAmount = 1f - (tiempo / duracion);
            yield return null;
        }

        barraDuracionHabilidad.fillAmount = 0f;
    }

    void ActivarPoder(EstadoIndigena ei)
    {
        controlDePersonaje.fuerzaSalto = ei.fuerzaSalto;
        controlDePersonaje.dañoRaycast = ei.dañoRaycast;
        controlDePersonaje.multiplicadorCorrer = ei.multiplicadorCorrer;
        controlDePersonaje.velocidadGasto = ei.velocidadGasto;
        controlDePersonaje.velocidadRecarga = ei.velocidadRecarga;
        controlDePersonaje.rangoAtaque = ei.rangoAtaque;
        vida.cobertura = ei.cobertura;
        customGravity.gravityScale = ei.gravedad;
    }
}
