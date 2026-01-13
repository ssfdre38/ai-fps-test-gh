using UnityEngine;

namespace FPSGame.Player
{
    /// <summary>
    /// Controls camera rotation and weapon sway.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float smoothing = 2f;
        
        [Header("Weapon Sway")]
        [SerializeField] private bool enableWeaponSway = true;
        [SerializeField] private float swayAmount = 0.02f;
        [SerializeField] private float swaySmoothing = 6f;
        [SerializeField] private Transform weaponHolder;
        
        [Header("Tilt")]
        [SerializeField] private bool enableTilt = true;
        [SerializeField] private float tiltAngle = 2f;
        [SerializeField] private float tiltSpeed = 3f;

        private Transform playerBody;
        private Camera playerCamera;
        
        private float rotationX;
        private float rotationY;
        private Vector2 currentMouseDelta;
        private Vector2 currentMouseDeltaVelocity;
        private Vector3 weaponSwayPosition;
        private Quaternion weaponSwayRotation;
        private float currentTilt;

        /// <summary>
        /// Gets or sets the mouse sensitivity.
        /// </summary>
        public float MouseSensitivity
        {
            get => mouseSensitivity;
            set => mouseSensitivity = Mathf.Clamp(value, 0.1f, 10f);
        }

        private void Awake()
        {
            playerCamera = GetComponent<Camera>();
            playerBody = transform.parent;
            
            if (weaponHolder == null)
            {
                // Try to find weapon holder if not assigned
                weaponHolder = transform.Find("WeaponHolder");
            }
        }

        private void Start()
        {
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            HandleMouseLook();
            HandleWeaponSway();
            HandleTilt();
        }

        /// <summary>
        /// Handles mouse look input and camera rotation.
        /// </summary>
        private void HandleMouseLook()
        {
            // Get mouse input
            Vector2 targetMouseDelta = new Vector2(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")
            );

            // Smooth mouse input
            currentMouseDelta = Vector2.SmoothDamp(
                currentMouseDelta,
                targetMouseDelta,
                ref currentMouseDeltaVelocity,
                1f / smoothing
            );

            // Apply sensitivity
            rotationX -= currentMouseDelta.y * mouseSensitivity * (invertY ? -1f : 1f);
            rotationY += currentMouseDelta.x * mouseSensitivity;

            // Clamp vertical rotation
            rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

            // Apply rotation
            transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            
            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.Euler(0f, rotationY, 0f);
            }
        }

        /// <summary>
        /// Handles weapon sway based on mouse movement.
        /// </summary>
        private void HandleWeaponSway()
        {
            if (!enableWeaponSway || weaponHolder == null)
                return;

            // Calculate target sway
            float swayX = -currentMouseDelta.x * swayAmount;
            float swayY = -currentMouseDelta.y * swayAmount;

            // Target position and rotation
            Vector3 targetPosition = new Vector3(swayX, swayY, 0);
            Quaternion targetRotation = Quaternion.Euler(
                swayY * 10f,
                swayX * 10f,
                swayX * 10f
            );

            // Smoothly interpolate
            weaponSwayPosition = Vector3.Lerp(
                weaponSwayPosition,
                targetPosition,
                swaySmoothing * Time.deltaTime
            );
            
            weaponSwayRotation = Quaternion.Slerp(
                weaponSwayRotation,
                targetRotation,
                swaySmoothing * Time.deltaTime
            );

            // Apply to weapon holder
            weaponHolder.localPosition = weaponSwayPosition;
            weaponHolder.localRotation = weaponSwayRotation;
        }

        /// <summary>
        /// Handles camera tilt based on horizontal movement.
        /// </summary>
        private void HandleTilt()
        {
            if (!enableTilt)
                return;

            // Get horizontal input
            float horizontalInput = Input.GetAxis("Horizontal");
            float targetTilt = -horizontalInput * tiltAngle;

            // Smoothly interpolate tilt
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);

            // Apply tilt to camera
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localRotation = Quaternion.Euler(
                currentRotation.x,
                currentRotation.y,
                currentTilt
            );
        }

        /// <summary>
        /// Applies camera shake effect.
        /// </summary>
        /// <param name="intensity">Shake intensity</param>
        /// <param name="duration">Shake duration</param>
        public void ApplyCameraShake(float intensity, float duration)
        {
            StartCoroutine(CameraShakeCoroutine(intensity, duration));
        }

        private System.Collections.IEnumerator CameraShakeCoroutine(float intensity, float duration)
        {
            Vector3 originalPosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                transform.localPosition = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }

        /// <summary>
        /// Resets camera rotation.
        /// </summary>
        public void ResetRotation()
        {
            rotationX = 0f;
            rotationY = 0f;
            transform.localRotation = Quaternion.identity;
            
            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Sets whether the camera controls are enabled.
        /// </summary>
        public void SetControlsEnabled(bool enabled)
        {
            this.enabled = enabled;
            
            if (enabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
