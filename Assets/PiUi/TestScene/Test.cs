using UnityEngine;
using UnityEngine.InputSystem; // NUEVO
using UnityEngine.UI;
using System.Collections;


public class Test : MonoBehaviour
{
    [SerializeField]
    PiUIManager piUi;
    public PiUI piMenu;

    private PiUI normalMenu;
    public InputActionProperty menuRadial;
    public ControlDePersonaje controlDePersonaje;
    public float duracionPoder = 5f;

    // Valores originales
    private Vector3 fuerzaSaltoOriginal;
    private float dañoRaycastOriginal;
    private float multiplicadorCorrerOriginal;
    private float velocidadGastoOriginal;
    private float velocidadRecargaOriginal;

    private void Awake()
    {
        menuRadial.action.Enable();
        menuRadial.action.performed += OnOpenMenu;
    }

    private void OnDisable()
    {
        menuRadial.action.performed -= OnOpenMenu; // NUEVO
        menuRadial.action.Disable(); // NUEVO
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

    // NUEVO - Esta función será llamada cuando presiones "E"
    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        bool isOpen = piUi.PiOpened("Normal Menu");

        if (!isOpen)
        {
            int i = 0;
            foreach (PiUI.PiData data in normalMenu.piData)
            {
                if (string.IsNullOrWhiteSpace(data.sliceLabel))
                {
                    data.sliceLabel = "Slice " + i.ToString();
                }

                if (data.onSlicePressed == null || data.onSlicePressed.GetPersistentEventCount() == 0)
                {
                    data.onSlicePressed = new UnityEngine.Events.UnityEvent();
                    data.onSlicePressed.AddListener(Jaguar);
                }

                i++;
            }

            piUi.UpdatePiMenu("Normal Menu");

            // ✅ Mostrar cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // ✅ Ocultar cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        piUi.ChangeMenuState("Normal Menu", new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    public void Jaguar()
    {
        piUi.ChangeMenuState("Normal Menu");

        // ✅ Ocultar cursor al cerrar el menú
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ActivarPoderJaguar();
        PiPiece slice = piMenu.piList[0]; // Primer slice
        piMenu.piData[0].isInteractable = false;
        slice.SetData(piMenu.piData[0], piMenu.innerRadius, piMenu.outerRadius, piMenu);
        StartCoroutine(HabilitarConFade(slice, 2f));
        Debug.Log("Jaguar");
    }

    public void Tucan()
    {
        piUi.ChangeMenuState("Normal Menu");

        // ✅ Ocultar cursor al cerrar el menú
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        Debug.Log("Jaguar");
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

    private IEnumerator RestaurarValoresOriginalesJaguar()
    {
        yield return new WaitForSeconds(duracionPoder);

        // Restaurar valores originales
        controlDePersonaje.fuerzaSalto = fuerzaSaltoOriginal;
        controlDePersonaje.dañoRaycast = dañoRaycastOriginal;
        controlDePersonaje.multiplicadorCorrer = multiplicadorCorrerOriginal;
        controlDePersonaje.velocidadGasto = velocidadGastoOriginal;
        controlDePersonaje.velocidadRecarga = velocidadRecargaOriginal;
    }

    private IEnumerator HabilitarConFade(PiPiece slice, float delay)
    {
        yield return new WaitForSeconds(delay);
        slice.FadeInAndEnable(1.5f); // Duración de la transición a blanco
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


