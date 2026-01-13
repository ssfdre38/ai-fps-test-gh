using UnityEngine;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Pistol implementation with semi-automatic firing mode.
    /// </summary>
    public class Pistol : WeaponBase
    {
        private bool triggerReleased = true;

        protected override void InitializeWeapon()
        {
            base.InitializeWeapon();
            // Pistol specific initialization
        }

        /// <summary>
        /// Pistols require trigger release between shots (semi-auto).
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
        /// Must be called when fire button is released.
        /// </summary>
        public void ReleaseTrigger()
        {
            triggerReleased = true;
        }
    }
}
