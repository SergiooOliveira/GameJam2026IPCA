using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Mobile Controls")]
    public FixedJoystick joystick;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (joystick != null)
        {
            movementX = joystick.Horizontal;
            movementY = joystick.Vertical;
        }
    }

    void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(movementX, 0f, movementY).normalized;

        if (joystick != null && new Vector2(movementX, movementY).magnitude < 0.1f)
        {
            inputDirection = Vector3.zero;
        }

        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        float targetSpeed = inputDirection.magnitude * moveSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 velocity = transform.forward * currentSpeed;

        CurrentVelocity = velocity;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        if (joystick == null)
        {
            Vector2 inputvector = callbackContext.ReadValue<Vector2>();
            movementX = inputvector.x;
            movementY = inputvector.y;
        }
    }
}