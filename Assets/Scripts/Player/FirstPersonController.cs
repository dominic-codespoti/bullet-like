using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 6f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 600f;
        [SerializeField] private float verticalLookLimit = 80f;

        private float cameraPitch;
        private Transform cameraTransform;
        private CharacterController controller;

        private bool isSprinting;
        private float verticalVelocity;
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraTransform = Camera.main.transform;
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
            isSprinting = Input.GetKey(KeyCode.LeftShift);
            var targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;

            if (controller.isGrounded && Input.GetButtonDown("Jump"))
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;

            var velocity = moveDirection * targetSpeed + Vector3.up * verticalVelocity;

            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX);

            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);

            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }
}
