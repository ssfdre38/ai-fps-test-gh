using UnityEngine;
using FPSGame.Weapons;

namespace FPSGame.Player
{
    /// <summary>
    /// First-person controller for player movement and input.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float gravity = 20f;
        
        [Header("Crouch Settings")]
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float crouchTransitionSpeed = 10f;
        
        [Header("Head Bob Settings")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float bobFrequency = 2f;
        [SerializeField] private float bobAmplitude = 0.05f;
        
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private PlayerAudio playerAudio;
        
        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float verticalVelocity;
        private bool isCrouching;
        private bool isSprinting;
        private float currentHeight;
        private float headBobTimer;
        private Vector3 originalCameraPosition;
        
        // Input
        private float moveX;
        private float moveZ;
        private bool jumpPressed;
        private bool crouchPressed;
        private bool sprintPressed;
        private bool firePressed;
        private bool reloadPressed;
        private bool aimPressed;

        /// <summary>
        /// Gets whether the player is currently sprinting.
        /// </summary>
        public bool IsSprinting => isSprinting;

        /// <summary>
        /// Gets whether the player is currently crouching.
        /// </summary>
        public bool IsCrouching => isCrouching;

        /// <summary>
        /// Gets whether the player is grounded.
        /// </summary>
        public bool IsGrounded => characterController.isGrounded;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            if (playerCamera != null)
            {
                originalCameraPosition = playerCamera.transform.localPosition;
            }
            
            currentHeight = standingHeight;
            characterController.height = currentHeight;
        }

        private void Start()
        {
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleInput();
            HandleMovement();
            HandleCrouch();
            HandleHeadBob();
            HandleWeaponInput();
        }

        /// <summary>
        /// Handles player input.
        /// </summary>
        private void HandleInput()
        {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
            jumpPressed = Input.GetButtonDown("Jump");
            crouchPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
            sprintPressed = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
            
            // Weapon input
            firePressed = Input.GetButton("Fire1");
            reloadPressed = Input.GetKeyDown(KeyCode.R);
            aimPressed = Input.GetButton("Fire2");
        }

        /// <summary>
        /// Handles player movement.
        /// </summary>
        private void HandleMovement()
        {
            float currentSpeed = walkSpeed;
            
            if (isCrouching)
            {
                currentSpeed = crouchSpeed;
                isSprinting = false;
            }
            else if (sprintPressed && moveZ > 0) // Can only sprint forward
            {
                currentSpeed = sprintSpeed;
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }

            // Calculate movement direction
            Vector3 forward = transform.forward * moveZ;
            Vector3 right = transform.right * moveX;
            moveDirection = (forward + right).normalized * currentSpeed;

            // Handle jumping
            if (characterController.isGrounded)
            {
                verticalVelocity = -gravity * Time.deltaTime; // Small downward force to keep grounded
                
                if (jumpPressed && !isCrouching)
                {
                    verticalVelocity = jumpForce;
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            moveDirection.y = verticalVelocity;

            // Move the character
            characterController.Move(moveDirection * Time.deltaTime);
            
            // Notify audio system
            if (playerAudio != null)
            {
                bool isMoving = moveX != 0 || moveZ != 0;
                playerAudio.UpdateMovement(isMoving, isSprinting, characterController.isGrounded);
            }
        }

        /// <summary>
        /// Handles crouching logic.
        /// </summary>
        private void HandleCrouch()
        {
            float targetHeight = crouchPressed ? crouchHeight : standingHeight;
            
            // Check if there's room to stand up
            if (!crouchPressed && isCrouching)
            {
                // Raycast upward to check for obstacles
                if (Physics.Raycast(transform.position, Vector3.up, standingHeight))
                {
                    targetHeight = crouchHeight; // Can't stand up, stay crouched
                }
            }
            
            isCrouching = targetHeight == crouchHeight;
            
            // Smoothly transition height
            if (Mathf.Abs(currentHeight - targetHeight) > 0.01f)
            {
                currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
                characterController.height = currentHeight;
                
                // Adjust camera position
                if (playerCamera != null)
                {
                    Vector3 cameraPos = playerCamera.transform.localPosition;
                    cameraPos.y = originalCameraPosition.y + (currentHeight - standingHeight);
                    playerCamera.transform.localPosition = cameraPos;
                }
            }
        }

        /// <summary>
        /// Handles head bobbing effect while moving.
        /// </summary>
        private void HandleHeadBob()
        {
            if (!enableHeadBob || playerCamera == null)
                return;

            if (characterController.isGrounded && (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f))
            {
                // Player is moving
                headBobTimer += Time.deltaTime * bobFrequency;
                
                float bobOffset = Mathf.Sin(headBobTimer) * bobAmplitude;
                
                Vector3 newCameraPos = playerCamera.transform.localPosition;
                newCameraPos.y = originalCameraPosition.y + (currentHeight - standingHeight) + bobOffset;
                playerCamera.transform.localPosition = newCameraPos;
            }
            else
            {
                // Reset head bob
                headBobTimer = 0f;
            }
        }

        /// <summary>
        /// Handles weapon-related input.
        /// </summary>
        private void HandleWeaponInput()
        {
            if (weaponManager == null)
                return;

            // Can't aim while sprinting
            if (isSprinting)
            {
                weaponManager.SetAiming(false);
            }
            else
            {
                weaponManager.SetAiming(aimPressed);
            }

            // Fire weapon
            if (firePressed && !isSprinting)
            {
                weaponManager.Fire();
            }

            // Reload weapon
            if (reloadPressed)
            {
                weaponManager.Reload();
            }
        }

        /// <summary>
        /// Gets the current movement velocity.
        /// </summary>
        public Vector3 GetVelocity()
        {
            return characterController.velocity;
        }

        /// <summary>
        /// Enables or disables player controls.
        /// </summary>
        public void SetControlsEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
    }
}
