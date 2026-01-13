using UnityEngine;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Sniper Rifle implementation with high damage and zoom capability.
    /// </summary>
    public class SniperRifle : WeaponBase
    {
        [Header("Sniper Settings")]
        [SerializeField] private float scopeZoomMultiplier = 4f;
        [SerializeField] private float breathHoldDuration = 5f;
        
        private bool triggerReleased = true;
        private bool isScoped = false;
        private float originalFOV;
        private float breathHoldTimer;

        protected override void Awake()
        {
            base.Awake();
            if (playerCamera != null)
            {
                originalFOV = playerCamera.fieldOfView;
            }
        }

        protected override void InitializeWeapon()
        {
            base.InitializeWeapon();
            breathHoldTimer = breathHoldDuration;
        }

        protected override void Update()
        {
            base.Update();
            
            // Handle breath holding when scoped
            if (isScoped && isAiming)
            {
                breathHoldTimer -= Time.deltaTime;
                if (breathHoldTimer <= 0)
                {
                    breathHoldTimer = 0;
                }
            }
            else if (!isAiming)
            {
                breathHoldTimer = breathHoldDuration;
            }
        }

        /// <summary>
        /// Sniper rifles are semi-auto with high precision.
        /// </summary>
        public override bool Fire()
        {
            if (!triggerReleased)
                return false;

            bool fired = base.Fire();
            if (fired)
            {
                triggerReleased = false;
            }
            return fired;
        }

        /// <summary>
        /// Sets aiming state and handles scope zoom.
        /// </summary>
        public override void SetAiming(bool aiming)
        {
            base.SetAiming(aiming);
            isScoped = aiming;
            
            if (playerCamera != null)
            {
                if (aiming)
                {
                    playerCamera.fieldOfView = originalFOV / scopeZoomMultiplier;
                }
                else
                {
                    playerCamera.fieldOfView = originalFOV;
                }
            }
        }

        /// <summary>
        /// Gets shooting direction with reduced spread when scoped and holding breath.
        /// </summary>
        protected override Vector3 GetShootDirection()
        {
            Vector3 direction = base.GetShootDirection();
            
            // Reduce spread significantly when scoped and holding breath
            if (isScoped && breathHoldTimer > 0)
            {
                // Nearly perfect accuracy when scoped
                return playerCamera.transform.forward;
            }
            
            return direction;
        }

        /// <summary>
        /// Must be called when fire button is released.
        /// </summary>
        public void ReleaseTrigger()
        {
            triggerReleased = true;
        }

        public override void OnUnequip()
        {
            base.OnUnequip();
            // Reset FOV when unequipping
            if (playerCamera != null && isScoped)
            {
                playerCamera.fieldOfView = originalFOV;
                isScoped = false;
            }
        }
    }
}
