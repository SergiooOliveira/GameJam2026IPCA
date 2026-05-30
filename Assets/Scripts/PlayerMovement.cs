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

    [Header("Stacker")]
    public GameObject itemPrefab;
    public Transform spawnPoint;

    //private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(movementX, 0f, movementY).normalized;

        if (inputDirection != Vector3.zero)
        {
            // 1. Rotate the player toward the input direction first
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            // Note: Swapped Slerp for RotateTowards. It creates a perfectly consistent turning circle.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // 2. Smoothly calculate how fast we should be going
        float targetSpeed = inputDirection.magnitude * moveSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // 3. THE MAGIC: We only ever move exactly where the player is currently facing. 
        // This forces the U-Turn!
        Vector3 velocity = transform.forward * currentSpeed;

        // 4. Save and move
        CurrentVelocity = velocity;
        controller.Move(velocity * Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext callbackContext)
    {
        Vector2 inputvector = callbackContext.ReadValue<Vector2>();

        movementX = inputvector.x;
        movementY = inputvector.y;
    }

    //public void OnAttack(InputAction.CallbackContext callbackContext)
    //{
    //    if (callbackContext.performed)
    //        animator.SetTrigger("wave");
    //}

    //public void OnInteract(InputAction.CallbackContext callbackContext)
    //{        
    //    // TODO: Swap the fixed rotation for something random in the x or z axis
    //    if (callbackContext.started)
    //    {            
    //        Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint.transform);
    //    }
    //}
}
