using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    public InputActionReference lookAction;

    [Header("Setup")]
    public Transform playerTransform;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Settings")]
    public float sensibilidadeMouse = 15.0f;
    public float fixedPitch = 45.0f;

    private float currentYaw;

    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        if (playerTransform != null)
            currentYaw = playerTransform.eulerAngles.y;
    }

    //void FixedUpdate() // 2. Changed from FixedUpdate
    //{
    //    RodarCamera();
    //}

    //void RodarCamera()
    //{
    //    if (lookAction == null) return;
    //    Vector2 inputLook = lookAction.action.ReadValue<Vector2>();

    //    float mouseX = inputLook.x * sensibilidadeMouse * Time.deltaTime;
    //    //float mouseY = inputLook.y * sensibilidadeMouse * Time.deltaTime;

    //    rotacaoY += mouseX; // Accumulate Left/Right movement
    //    //rotacaoX -= mouseY; // Accumulate Up/Down movement
    //    //rotacaoX = Mathf.Clamp(rotacaoX, -limiteOlharCimaBaixo, limiteOlharCimaBaixo);

    //    // 3. Apply both X and Y at the same time so nothing gets forced to zero
    //    cameraJogador.localRotation = Quaternion.Euler(45f, rotacaoY, 0f);
    //}

    //public float GetMouseX(float sensibilidade)
    //{
    //    if (lookAction == null) return 0f;
    //    return lookAction.action.ReadValue<Vector2>().x * sensibilidade * Time.deltaTime;
    //}

    void LateUpdate() // LateUpdate ensures the camera moves strictly AFTER the player
    {
        if (lookAction != null)
        {
            float mouseX = lookAction.action.ReadValue<Vector2>().x * sensibilidadeMouse * Time.deltaTime;
            currentYaw += mouseX;
        }

        // Apply the fixed pitch and dynamic yaw
        Quaternion rotation = Quaternion.Euler(fixedPitch, currentYaw, 0f);

        // Keep the camera hovering at the exact offset, orbiting based on the yaw
        transform.position = playerTransform.position + rotation * offset;
        transform.rotation = rotation;
    }
}