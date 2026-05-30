using UnityEngine;
using UnityEngine.InputSystem; // Mantido caso use para outras ações (como atacar/interagir)

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] private float currentSpeed;
    [SerializeField] float acceleration = 15f;
    [SerializeField] float rotationSpeed = 720f;

    public Vector3 CurrentVelocity { get; private set; }

    private float movementX;
    private float movementY;

    private CharacterController controller;

    [Header("Stacker")]
    public GameObject itemPrefab;
    public Transform spawnPoint;

    [Header("Mobile Controls")]
    public FixedJoystick joystick; // O seu joystick do script antigo

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Se o joystick estiver devidamente arrastado no Inspetor, ele sobrescreve os inputs
        if (joystick != null)
        {
            movementX = joystick.Horizontal;
            movementY = joystick.Vertical;
        }
    }

    void FixedUpdate()
    {
        // Criamos a direção com base no input (seja do Joystick ou do Input System)
        Vector3 inputDirection = new Vector3(movementX, 0f, movementY).normalized;

        // Se o joystick se mover quase nada, evitamos rotações fantasmas
        if (joystick != null && new Vector2(movementX, movementY).magnitude < 0.1f)
        {
            inputDirection = Vector3.zero;
        }

        if (inputDirection != Vector3.zero)
        {
            // 1. Rotaciona o jogador na direção do input
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // 2. Calcula suavemente a velocidade
        float targetSpeed = inputDirection.magnitude * moveSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // 3. Move exatamente para onde o jogador está olhando (Garante o U-Turn)
        Vector3 velocity = transform.forward * currentSpeed;

        // 4. Salva e move
        CurrentVelocity = velocity;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    // Mantido caso você ainda queira testar no PC usando o teclado pelo New Input System
    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        if (joystick == null) // Só usa o Input System se NÃO houver joystick na tela
        {
            Vector2 inputvector = callbackContext.ReadValue<Vector2>();
            movementX = inputvector.x;
            movementY = inputvector.y;
        }
    }
}