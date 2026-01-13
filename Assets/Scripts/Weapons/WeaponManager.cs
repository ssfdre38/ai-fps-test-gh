using UnityEngine;
using System.Collections.Generic;
using FPSGame.Utilities;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Manages weapon switching and handles input for weapon actions.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapon Slots")]
        [SerializeField] private List<WeaponBase> weapons = new List<WeaponBase>();
        
        [Header("Settings")]
        [SerializeField] private float weaponSwitchSpeed = 0.5f;
        
        private int currentWeaponIndex = 0;
        private WeaponBase currentWeapon;
        private bool isSwitching = false;
        private float switchTimer;

        // Events
        public delegate void WeaponChangedHandler(WeaponBase newWeapon, int weaponIndex);
        public event WeaponChangedHandler OnWeaponChanged;

        /// <summary>
        /// Gets the currently equipped weapon.
        /// </summary>
        public WeaponBase CurrentWeapon => currentWeapon;

        /// <summary>
        /// Gets the current weapon index.
        /// </summary>
        public int CurrentWeaponIndex => currentWeaponIndex;

        private void Start()
        {
            InitializeWeapons();
        }

        private void Update()
        {
            HandleWeaponSwitching();
        }

        /// <summary>
        /// Initializes all weapons and equips the first one.
        /// </summary>
        private void InitializeWeapons()
        {
            // Deactivate all weapons
            foreach (var weapon in weapons)
            {
                if (weapon != null)
                {
                    weapon.OnUnequip();
                }
            }

            // Equip first weapon
            if (weapons.Count > 0 && weapons[0] != null)
            {
                EquipWeapon(0);
            }
        }

        /// <summary>
        /// Handles weapon switching input.
        /// </summary>
        private void HandleWeaponSwitching()
        {
            if (isSwitching)
            {
                switchTimer -= Time.deltaTime;
                if (switchTimer <= 0)
                {
                    isSwitching = false;
                }
                return;
            }

            // Number key weapon switching
            if (Input.GetKeyDown(KeyCode.Alpha1) && weapons.Count > 0)
            {
                SwitchToWeapon(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count > 1)
            {
                SwitchToWeapon(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count > 2)
            {
                SwitchToWeapon(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && weapons.Count > 3)
            {
                SwitchToWeapon(3);
            }

            // Mouse wheel weapon switching
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel > 0f)
            {
                SwitchToNextWeapon();
            }
            else if (scrollWheel < 0f)
            {
                SwitchToPreviousWeapon();
            }
        }

        /// <summary>
        /// Switches to the specified weapon index.
        /// </summary>
        /// <param name="weaponIndex">Index of the weapon to switch to</param>
        public void SwitchToWeapon(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= weapons.Count || weaponIndex == currentWeaponIndex)
                return;

            if (weapons[weaponIndex] == null)
                return;

            if (currentWeapon != null && currentWeapon.IsReloading)
                return;

            EquipWeapon(weaponIndex);
        }

        /// <summary>
        /// Switches to the next weapon in the list.
        /// </summary>
        public void SwitchToNextWeapon()
        {
            if (weapons.Count <= 1)
                return;

            int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
            SwitchToWeapon(nextIndex);
        }

        /// <summary>
        /// Switches to the previous weapon in the list.
        /// </summary>
        public void SwitchToPreviousWeapon()
        {
            if (weapons.Count <= 1)
                return;

            int prevIndex = currentWeaponIndex - 1;
            if (prevIndex < 0)
                prevIndex = weapons.Count - 1;
            
            SwitchToWeapon(prevIndex);
        }

        /// <summary>
        /// Equips the weapon at the specified index.
        /// </summary>
        private void EquipWeapon(int weaponIndex)
        {
            // Unequip current weapon
            if (currentWeapon != null)
            {
                currentWeapon.OnUnequip();
            }

            // Equip new weapon
            currentWeaponIndex = weaponIndex;
            currentWeapon = weapons[currentWeaponIndex];
            currentWeapon.OnEquip();

            // Start switch cooldown
            isSwitching = true;
            switchTimer = weaponSwitchSpeed;

            OnWeaponChanged?.Invoke(currentWeapon, currentWeaponIndex);
        }

        /// <summary>
        /// Attempts to fire the current weapon.
        /// </summary>
        public bool Fire()
        {
            if (currentWeapon == null || isSwitching)
                return false;

            return currentWeapon.Fire();
        }

        /// <summary>
        /// Reloads the current weapon.
        /// </summary>
        public void Reload()
        {
            if (currentWeapon == null || isSwitching)
                return;

            currentWeapon.Reload();
        }

        /// <summary>
        /// Sets aiming state for the current weapon.
        /// </summary>
        public void SetAiming(bool aiming)
        {
            if (currentWeapon == null)
                return;

            currentWeapon.SetAiming(aiming);
        }

        /// <summary>
        /// Adds a weapon to the weapon list.
        /// </summary>
        public void AddWeapon(WeaponBase weapon)
        {
            if (weapon == null)
                return;

            weapons.Add(weapon);
            weapon.OnUnequip();
        }

        /// <summary>
        /// Removes a weapon from the weapon list.
        /// </summary>
        public void RemoveWeapon(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= weapons.Count)
                return;

            if (weaponIndex == currentWeaponIndex)
            {
                // Switch to another weapon before removing
                SwitchToNextWeapon();
            }

            weapons.RemoveAt(weaponIndex);
        }

        /// <summary>
        /// Adds ammo to a specific weapon type.
        /// </summary>
        public void AddAmmoToWeapon(int weaponIndex, int amount)
        {
            if (weaponIndex >= 0 && weaponIndex < weapons.Count && weapons[weaponIndex] != null)
            {
                weapons[weaponIndex].AddAmmo(amount);
            }
        }
    }
}
