using UnityEngine;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Assault Rifle implementation with full-auto firing mode.
    /// </summary>
    public class AssaultRifle : WeaponBase
    {
        protected override void InitializeWeapon()
        {
            base.InitializeWeapon();
            // Assault rifle specific initialization if needed
        }

        /// <summary>
        /// Assault rifles support continuous firing when trigger is held.
        /// </summary>
        public override bool Fire()
        {
            // Full auto - can fire continuously
            return base.Fire();
        }
    }
}
