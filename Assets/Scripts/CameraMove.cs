using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    //public InputActionReference lookAction;

    [Header("Setup")]
    public Transform playerTransform;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    //[Header("Settings")]
    //public float sensibilidadeMouse = 15.0f;
    private float fixedPitch = 45.0f;

    private float currentYaw;

    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        if (playerTransform != null)
            currentYaw = playerTransform.eulerAngles.y;
    }

    void LateUpdate() // LateUpdate ensures the camera moves strictly AFTER the player
    {
        //if (lookAction != null)
        //{
        //    float mouseX = lookAction.action.ReadValue<Vector2>().x * sensibilidadeMouse * Time.deltaTime;
        //    currentYaw += mouseX;
        //}

        //// Apply the fixed pitch and dynamic yaw
        Quaternion rotation = Quaternion.Euler(fixedPitch, currentYaw, 0f);

        //// Keep the camera hovering at the exact offset, orbiting based on the yaw
        transform.position = playerTransform.position + rotation * offset;
        transform.rotation = rotation;
    }
}