using UnityEngine;
using System.Collections;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Shotgun implementation with multiple pellets per shot.
    /// </summary>
    public class Shotgun : WeaponBase
    {
        [Header("Shotgun Settings")]
        [SerializeField] private int pelletsPerShot = 8;
        [SerializeField] private float pelletSpreadAngle = 5f;

        private bool triggerReleased = true;

        protected override void InitializeWeapon()
        {
            base.InitializeWeapon();
        }

        /// <summary>
        /// Shotguns are semi-auto and fire multiple pellets.
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
        /// Performs shotgun shot with multiple pellets.
        /// </summary>
        protected override void PerformShot()
        {
            // Don't call base, we'll do our own pellet shooting
            ApplyRecoil();
            
            // Fire multiple pellets
            for (int i = 0; i < pelletsPerShot; i++)
            {
                Vector3 direction = GetShotgunPelletDirection();
                
                if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, weaponData.range))
                {
                    ProcessHit(hit);
                }
            }
            
            PlayFireEffects();
            
            if (weaponData.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
            }
        }

        /// <summary>
        /// Gets direction for a single shotgun pellet with spread.
        /// </summary>
        private Vector3 GetShotgunPelletDirection()
        {
            Vector3 forward = playerCamera.transform.forward;
            
            // Apply shotgun-specific spread
            float spreadX = Random.Range(-pelletSpreadAngle, pelletSpreadAngle);
            float spreadY = Random.Range(-pelletSpreadAngle, pelletSpreadAngle);
            
            return Quaternion.Euler(spreadY, spreadX, 0) * forward;
        }

        /// <summary>
        /// Must be called when fire button is released.
        /// </summary>
        public void ReleaseTrigger()
        {
            triggerReleased = true;
        }
    }
}
