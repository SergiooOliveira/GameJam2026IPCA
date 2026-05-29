using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    public Vector3 CurrentVelocity { get; private set; }
    [SerializeField] float rotationSpeed = 15f;

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
        //cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        //Vector3 camForward = cameraTransform.forward;
        //Vector3 camRight = cameraTransform.right;
        //camForward.y = 0f;
        //camRight.y = 0f;
        //camForward.Normalize();
        //camRight.Normalize();

        Vector3 moveDirection = new Vector3(movementX, 0f, movementY).normalized;
        Vector3 velocity = moveDirection * moveSpeed;
        CurrentVelocity = velocity;

        controller.Move(velocity * Time.fixedDeltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
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
