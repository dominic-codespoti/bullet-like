using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 300f;
    [SerializeField, Tooltip("Clamp vertical look angle to prevent flipping upside down.")]
    private float verticalLookLimit = 80f;

    private CharacterController _controller;
    private Transform _cameraTransform;

    private float _cameraPitch = 0f;
    private float _verticalVelocity = 0f;
    private bool _isSprinting;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleMovement()
    {
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = _isSprinting ? sprintSpeed : walkSpeed;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        if (_controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * targetSpeed + Vector3.up * _verticalVelocity;

        _controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        _cameraPitch -= mouseY;  
        _cameraPitch = Mathf.Clamp(_cameraPitch, -verticalLookLimit, verticalLookLimit);

        _cameraTransform.localEulerAngles = new Vector3(_cameraPitch, 0f, 0f);
    }
}
