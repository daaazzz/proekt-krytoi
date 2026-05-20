using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Выносим enum наружу, чтобы Unity не ругался
    private enum RollType { Forward, Backward, Left, Right }

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 200f;
    public float upperLookLimit = 80f;
    public float lowerLookLimit = -80f;

    [Header("Roll Settings")]
    public float rollDuration = 0.4f;
    public float rollSpeed = 12f;

    [HideInInspector] public bool isRolling = false;
    [HideInInspector] public bool isInvincible = false;

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Vector3 moveDirection;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (isRolling) return;

        HandleLook();
        HandleMovement();
        HandleRollInput();
    }

    void HandleMovement()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, lowerLookLimit, upperLookLimit);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleRollInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveDirection.magnitude > 0 && characterController.isGrounded)
        {
            StartCoroutine(RollRoutine());
        }
    }

    IEnumerator RollRoutine()
    {
        isRolling = true;
        isInvincible = true;

        Vector3 rollDir = moveDirection;

        // Расчет направления относительно взгляда
        float sideRoll = Vector3.Dot(rollDir, transform.right);
        float forwardRoll = Vector3.Dot(rollDir, transform.forward);

        float elapsedTime = 0f;
        float initialXRotation = xRotation;

        RollType currentRollType = RollType.Forward;

        // Определяем, куда катимся
        if (Mathf.Abs(forwardRoll) > Mathf.Abs(sideRoll))
        {
            currentRollType = (forwardRoll > 0) ? RollType.Forward : RollType.Backward;
        }
        else
        {
            currentRollType = (sideRoll > 0) ? RollType.Right : RollType.Left;
        }

        while (elapsedTime < rollDuration)
        {
            characterController.Move(rollDir * rollSpeed * Time.deltaTime);
            float t = elapsedTime / rollDuration;

            float xOffset = 0f;
            float zRotation = 0f;

            switch (currentRollType)
            {
                case RollType.Right:
                    zRotation = Mathf.Lerp(0f, -360f, t);
                    break;

                case RollType.Left:
                    zRotation = Mathf.Lerp(0f, 360f, t);
                    break;

                case RollType.Backward:
                    float pitchCurveBack = Mathf.Sin(t * Mathf.PI);
                    xOffset = pitchCurveBack * 25f; // Слегка наклоняем камеру вниз при отпрыжке
                    break;

                case RollType.Forward:
                    float pitchCurveFwd = Mathf.Sin(t * Mathf.PI);
                    xOffset = -pitchCurveFwd * 25f; // Слегка наклоняем вверх
                    break;
            }

            cameraTransform.localRotation = Quaternion.Euler(initialXRotation + xOffset, 0f, zRotation);

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        isInvincible = false;
        isRolling = false;
    }
}