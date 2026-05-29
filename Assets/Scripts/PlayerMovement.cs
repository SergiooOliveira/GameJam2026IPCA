using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;
    
    private float movementX;
    private float movementY;

    private CharacterController controller;
    private Transform cameraTransform;
    private CameraMove cameraMove;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

        cameraTransform = Camera.main.transform;
        //cameraMove = GetComponentInChildren<CameraMove>();
    }

    void Update() // Yaw must be in Update(), same as camera
    {
        // Player body rotates horizontally, in sync with mouse
        //float mouseX = cameraMove.GetMouseX(cameraMove.sensibilidadeMouse);
        //transform.Rotate(Vector3.up * mouseX);
    }

    void FixedUpdate()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * movementY + camRight * movementX);
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
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
}
