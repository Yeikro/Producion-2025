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

    // Valores originales
    private Vector3 fuerzaSaltoOriginal;
    private float dañoRaycastOriginal;
    private float multiplicadorCorrerOriginal;
    private float velocidadGastoOriginal;
    private float velocidadRecargaOriginal;
    private float rangoAtaqueOriginal;
    private float coberturaOriginal;

    private void Awake()
    {
        menuRadial.action.Enable();
        menuRadial.action.performed += OnOpenMenu;
        freeLookCam = FindFirstObjectByType<CinemachineFreeLook>();
        if (freeLookCam != null)
        {
            originalXSpeed = freeLookCam.m_XAxis.m_MaxSpeed;
            originalYSpeed = freeLookCam.m_YAxis.m_MaxSpeed;
        }

        fuerzaSaltoOriginal = controlDePersonaje.fuerzaSalto;
        dañoRaycastOriginal = controlDePersonaje.dañoRaycast;
        multiplicadorCorrerOriginal = controlDePersonaje.multiplicadorCorrer;
        velocidadGastoOriginal = controlDePersonaje.velocidadGasto;
        velocidadRecargaOriginal = controlDePersonaje.velocidadRecarga;
        rangoAtaqueOriginal = controlDePersonaje.rangoAtaque;
        coberturaOriginal = vida.cobertura;
        inmune = false;
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
        // Guardar valores originales
        fuerzaSaltoOriginal = controlDePersonaje.fuerzaSalto;
        dañoRaycastOriginal = controlDePersonaje.dañoRaycast;
        multiplicadorCorrerOriginal = controlDePersonaje.multiplicadorCorrer;
        velocidadGastoOriginal = controlDePersonaje.velocidadGasto;
        velocidadRecargaOriginal = controlDePersonaje.velocidadRecarga;

        // Aplicar los cambios
        controlDePersonaje.fuerzaSalto *= 1.5f;
        controlDePersonaje.daño = 4;
        controlDePersonaje.multiplicadorCorrer = 10f;
        controlDePersonaje.velocidadGasto = 0.2f;
        controlDePersonaje.velocidadRecarga = 0.5f;

        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesJaguar());
    }

    public void ActivarPoderTucan()
    {
        // Guardar valores originales
        fuerzaSaltoOriginal = controlDePersonaje.fuerzaSalto;

        // Aplicar los cambios
        controlDePersonaje.fuerzaSalto *= 3.5f;

        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesTucan());
    }

    public void ActivarPoderRana()
    {
        // Guardar valores originales
        rangoAtaqueOriginal = controlDePersonaje.rangoAtaque;

        // Aplicar los cambios
        controlDePersonaje.rangoAtaque *= 3.5f;

        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesRana());
    }

    public void ActivarPoderMono()
    {
        coberturaOriginal = vida.cobertura;

        inmune = true;

        vida.cubierto = true;

        if(vida.cubierto)
        {
            vida.cobertura = 1f;
        }

        // Iniciar la corrutina para restaurar valores
        StartCoroutine(RestaurarValoresOriginalesMono());
    }

    private IEnumerator RestaurarValoresOriginalesJaguar()
    {
        yield return new WaitForSeconds(duracionPoderJaguar);

        // Restaurar valores originales
        controlDePersonaje.fuerzaSalto = fuerzaSaltoOriginal;
        controlDePersonaje.dañoRaycast = dañoRaycastOriginal;
        controlDePersonaje.multiplicadorCorrer = multiplicadorCorrerOriginal;
        controlDePersonaje.velocidadGasto = velocidadGastoOriginal;
        controlDePersonaje.velocidadRecarga = velocidadRecargaOriginal;
        StartCoroutine(HabilitarConFadeJaguar(piMenu.piList[0], enfriamientoHabilidad));
    }
    private IEnumerator RestaurarValoresOriginalesTucan()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        controlDePersonaje.fuerzaSalto = fuerzaSaltoOriginal;

        StartCoroutine(HabilitarConFadeTucan(piMenu.piList[3], enfriamientoHabilidad));
    }

    private IEnumerator RestaurarValoresOriginalesRana()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        controlDePersonaje.rangoAtaque = rangoAtaqueOriginal;

        StartCoroutine(HabilitarConFadeRana(piMenu.piList[2], enfriamientoHabilidad));
    }

    private IEnumerator RestaurarValoresOriginalesMono()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        vida.cubierto = false;
        vida.cobertura = coberturaOriginal;
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
        controlDePersonaje.fuerzaSalto = fuerzaSaltoOriginal;
        controlDePersonaje.dañoRaycast = dañoRaycastOriginal;
        controlDePersonaje.multiplicadorCorrer = multiplicadorCorrerOriginal;
        controlDePersonaje.velocidadGasto = velocidadGastoOriginal;
        controlDePersonaje.velocidadRecarga = velocidadRecargaOriginal;
        controlDePersonaje.rangoAtaque = rangoAtaqueOriginal;
        vida.cobertura = coberturaOriginal;
    }

    public void OnHoverEnter()
    {
        Debug.Log("Hey get off of me!");
    }

    public void OnHoverExit()
    {
        Debug.Log("That's right and dont come back!");
    }
}
