using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform; // La c�mara (hijo del jugador)
    [SerializeField] private Rigidbody rb;              // El Rigidbody del jugador

    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 0;
    [SerializeField] private float runSpeed = 0;      // (Opcional) si quieres correr
    [SerializeField] private float jumpForce = 0;

    [Header("Raton")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;  // L�mite para mirar arriba/abajo

    // Variables de entrada
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;

    // Variables de estado
    private float verticalRotation = 0f;

    // Referencias a las acciones del Input System (se asignan en Awake)
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private void Awake()
    {
        // Buscar las acciones por nombre (deben coincidir con tu Input Action Asset)
        // Si usas el Input System por defecto, los nombres suelen ser "Move", "Look", "Jump"
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");

        // Si no tienes definidas estas acciones, puedes crearlas en el Editor o usar
        // InputSystem.actions.FindAction("Move") con los nombres que hayas puesto.
        // Tambi�n puedes asignarlas desde el Inspector si prefieres.

        // Bloquear el cursor al inicio
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        // Habilitar las acciones (opcional, pero recomendado)
        moveAction?.Enable();
        lookAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        lookAction?.Disable();
        jumpAction?.Disable();
    }

    private void Update()
    {
        // Leer inputs en Update (m�s responsivo para eventos)
        moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        lookInput = lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

        // Detectar pulso de salto (solo una vez al presionar)
        if (jumpAction != null)
        {
            if (jumpAction.WasPressedThisFrame())
                jumpPressed = true;
        }

        // Rotaci�n horizontal del cuerpo (girar a la izquierda/derecha con el rat�n)
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX);

        // Rotaci�n vertical de la c�mara (mirar arriba/abajo)
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void FixedUpdate()
    {
        // Movimiento en FixedUpdate (f�sicas)
        if (rb == null) return;

        // Obtener direcci�n de movimiento relativa a la orientaci�n del jugador
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Movimiento en el plano horizontal (ignoramos la Y)
        Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);
        moveDirection.Normalize();

        // Velocidad deseada (usamos walkSpeed por defecto)
        float currentSpeed = walkSpeed;
        // Si quieres correr, puedes a�adir una tecla (ej: Shift) y cambiar la velocidad
        // if (InputSystem.GetKey(KeyCode.LeftShift)) currentSpeed = runSpeed;

        // Aplicar velocidad directamente al Rigidbody (manteniendo la velocidad vertical)
        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = rb.linearVelocity.y; // Conservar la velocidad vertical (gravedad, saltos)
        rb.linearVelocity = targetVelocity;

        // Salto (si se ha pulsado y est� en el suelo)
        if (jumpPressed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false; // Resetear para que no salte repetido
        }
    }

    // M�todo para comprobar si el jugador esta tocando el suelo
    private bool IsGrounded()
    {
        // Lanza un rayo hacia abajo desde el centro del personaje
        float distanceToGround = 0.2f;
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround);
    }

    // (Opcional) Para soltar/ocultar el cursor con Escape
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}