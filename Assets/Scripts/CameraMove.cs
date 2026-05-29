using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    public InputActionReference lookAction;

    [Header("Câmara")]
    public Transform cameraJogador;
    public float sensibilidadeMouse = 15.0f;
    public float limiteOlharCimaBaixo = 80.0f;

    private float rotacaoX = 0;
    private float rotacaoY; // 1. Added a variable to track Left/Right rotation

    public float CurrentYaw => rotacaoY;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rotacaoY = transform.parent.eulerAngles.y;
    }

    void FixedUpdate() // 2. Changed from FixedUpdate
    {
        //RodarCamera();
    }

    //void RodarCamera()
    //{
    //    if (lookAction == null) return;
    //    Vector2 inputLook = lookAction.action.ReadValue<Vector2>();

    //    float mouseX = inputLook.x * sensibilidadeMouse * Time.deltaTime;
    //    float mouseY = inputLook.y * sensibilidadeMouse * Time.deltaTime;

    //    //rotacaoY += mouseX; // Accumulate Left/Right movement
    //    rotacaoX -= mouseY; // Accumulate Up/Down movement
    //    rotacaoX = Mathf.Clamp(rotacaoX, -limiteOlharCimaBaixo, limiteOlharCimaBaixo);

    //    // 3. Apply both X and Y at the same time so nothing gets forced to zero
    //    cameraJogador.localRotation = Quaternion.Euler(rotacaoX, rotacaoY, 0f);
    //}

    public float GetMouseX(float sensibilidade)
    {
        if (lookAction == null) return 0f;
        return lookAction.action.ReadValue<Vector2>().x * sensibilidade * Time.deltaTime;
    }
}