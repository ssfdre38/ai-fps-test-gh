using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.Player
{
    /// <summary>
    /// Handles player audio including footsteps on different surfaces.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Footstep Settings")]
        [SerializeField] private float walkStepInterval = 0.5f;
        [SerializeField] private float sprintStepInterval = 0.3f;
        [SerializeField] private float footstepVolume = 0.5f;
        
        [Header("Footstep Sounds")]
        [SerializeField] private AudioClip[] concreteFootsteps;
        [SerializeField] private AudioClip[] metalFootsteps;
        [SerializeField] private AudioClip[] woodFootsteps;
        [SerializeField] private AudioClip[] grassFootsteps;
        [SerializeField] private AudioClip[] waterFootsteps;
        
        [Header("Jump & Land")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip landSound;
        [SerializeField] private float landSoundVelocityThreshold = 5f;

        private AudioSource audioSource;
        private float stepTimer;
        private bool wasGrounded;
        private bool isMoving;
        private bool isSprinting;
        private SurfaceType currentSurface = SurfaceType.Concrete;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = footstepVolume;
            audioSource.spatialBlend = 0f; // 2D sound for player
        }

        private void Update()
        {
            HandleFootsteps();
        }

        /// <summary>
        /// Updates movement state from FPSController.
        /// </summary>
        public void UpdateMovement(bool moving, bool sprinting, bool grounded)
        {
            isMoving = moving;
            isSprinting = sprinting;
            
            // Handle landing sound
            if (grounded && !wasGrounded)
            {
                PlayLandSound();
            }
            
            wasGrounded = grounded;
        }

        /// <summary>
        /// Handles footstep sound playback.
        /// </summary>
        private void HandleFootsteps()
        {
            if (!isMoving || !wasGrounded)
            {
                stepTimer = 0f;
                return;
            }

            float interval = isSprinting ? sprintStepInterval : walkStepInterval;
            stepTimer += Time.deltaTime;

            if (stepTimer >= interval)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
        }

        /// <summary>
        /// Plays a footstep sound based on current surface.
        /// </summary>
        private void PlayFootstepSound()
        {
            AudioClip[] footsteps = GetFootstepsForSurface(currentSurface);
            
            if (footsteps != null && footsteps.Length > 0)
            {
                AudioClip clip = footsteps[Random.Range(0, footsteps.Length)];
                audioSource.PlayOneShot(clip, footstepVolume);
            }
        }

        /// <summary>
        /// Gets footstep sounds for the given surface type.
        /// </summary>
        private AudioClip[] GetFootstepsForSurface(SurfaceType surface)
        {
            switch (surface)
            {
                case SurfaceType.Concrete:
                    return concreteFootsteps;
                case SurfaceType.Metal:
                    return metalFootsteps;
                case SurfaceType.Wood:
                    return woodFootsteps;
                case SurfaceType.Grass:
                    return grassFootsteps;
                case SurfaceType.Water:
                    return waterFootsteps;
                default:
                    return concreteFootsteps;
            }
        }

        /// <summary>
        /// Plays the jump sound.
        /// </summary>
        public void PlayJumpSound()
        {
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, footstepVolume);
            }
        }

        /// <summary>
        /// Plays the landing sound.
        /// </summary>
        private void PlayLandSound()
        {
            if (landSound != null)
            {
                audioSource.PlayOneShot(landSound, footstepVolume * 1.5f);
            }
        }

        /// <summary>
        /// Sets the current surface type for footstep sounds.
        /// </summary>
        public void SetSurfaceType(SurfaceType surface)
        {
            currentSurface = surface;
        }

        /// <summary>
        /// Detects surface type from raycast hit.
        /// </summary>
        public void DetectSurface(RaycastHit hit)
        {
            // You can use tags or materials to determine surface type
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Metal"))
                    currentSurface = SurfaceType.Metal;
                else if (hit.collider.CompareTag("Wood"))
                    currentSurface = SurfaceType.Wood;
                else if (hit.collider.CompareTag("Grass"))
                    currentSurface = SurfaceType.Grass;
                else if (hit.collider.CompareTag("Water"))
                    currentSurface = SurfaceType.Water;
                else
                    currentSurface = SurfaceType.Concrete;
            }
        }
    }
}
