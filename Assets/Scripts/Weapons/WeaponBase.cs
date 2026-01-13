using UnityEngine;
using FPSGame.Utilities;
using System.Collections;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Abstract base class for all weapons. Handles common weapon functionality.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] protected WeaponData weaponData;
        
        [Header("References")]
        [SerializeField] protected Transform firePoint;
        [SerializeField] protected Camera playerCamera;
        [SerializeField] protected Animator weaponAnimator;
        [SerializeField] protected ParticleSystem muzzleFlash;
        [SerializeField] protected Transform shellEjectionPoint;
        
        // Ammo
        protected int currentAmmo;
        protected int reserveAmmo;
        
        // Timing
        protected float lastFireTime;
        protected bool isReloading;
        protected bool isAiming;
        
        // Spread
        protected float currentSpread;
        
        // Recoil
        protected Vector3 currentRecoil;
        protected Vector3 targetRecoil;
        
        // Events
        public delegate void AmmoChangedHandler(int current, int reserve);
        public event AmmoChangedHandler OnAmmoChanged;
        
        public delegate void WeaponFiredHandler();
        public event WeaponFiredHandler OnWeaponFired;

        /// <summary>
        /// Gets the weapon data.
        /// </summary>
        public WeaponData WeaponData => weaponData;

        /// <summary>
        /// Gets whether the weapon is currently reloading.
        /// </summary>
        public bool IsReloading => isReloading;

        /// <summary>
        /// Gets the current ammo count.
        /// </summary>
        public int CurrentAmmo => currentAmmo;

        /// <summary>
        /// Gets the reserve ammo count.
        /// </summary>
        public int ReserveAmmo => reserveAmmo;

        protected virtual void Awake()
        {
            if (playerCamera == null)
                playerCamera = Camera.main;
            
            InitializeWeapon();
        }

        /// <summary>
        /// Initializes the weapon with starting ammo.
        /// </summary>
        protected virtual void InitializeWeapon()
        {
            currentAmmo = weaponData.magazineSize;
            reserveAmmo = weaponData.maxReserveAmmo;
            currentSpread = weaponData.baseSpread;
            OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
        }

        protected virtual void Update()
        {
            // Decrease spread over time
            if (currentSpread > weaponData.baseSpread)
            {
                currentSpread -= weaponData.spreadDecreaseRate * Time.deltaTime;
                currentSpread = Mathf.Max(currentSpread, weaponData.baseSpread);
            }
            
            // Handle recoil
            HandleRecoil();
        }

        /// <summary>
        /// Attempts to fire the weapon.
        /// </summary>
        /// <returns>True if weapon fired successfully</returns>
        public virtual bool Fire()
        {
            if (isReloading || currentAmmo <= 0)
            {
                if (currentAmmo <= 0 && weaponData.emptySound != null)
                {
                    AudioSource.PlayClipAtPoint(weaponData.emptySound, transform.position);
                }
                return false;
            }

            if (Time.time - lastFireTime < weaponData.fireRate)
                return false;

            lastFireTime = Time.time;
            
            PerformShot();
            
            currentAmmo--;
            OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
            OnWeaponFired?.Invoke();
            
            return true;
        }

        /// <summary>
        /// Performs the actual shot logic.
        /// </summary>
        protected virtual void PerformShot()
        {
            // Add recoil
            ApplyRecoil();
            
            // Increase spread
            currentSpread = Mathf.Min(currentSpread + weaponData.spreadIncreasePerShot, weaponData.maxSpread);
            
            // Perform raycast
            Vector3 direction = GetShootDirection();
            
            if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, weaponData.range))
            {
                ProcessHit(hit);
            }
            
            // Visual effects
            PlayFireEffects();
            
            // Audio
            if (weaponData.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
            }
        }

        /// <summary>
        /// Gets the shooting direction with spread applied.
        /// </summary>
        protected virtual Vector3 GetShootDirection()
        {
            Vector3 forward = playerCamera.transform.forward;
            
            float spread = currentSpread;
            if (isAiming)
                spread *= weaponData.adsSpreadMultiplier;
            
            // Add random spread
            float spreadX = Random.Range(-spread, spread);
            float spreadY = Random.Range(-spread, spread);
            
            return (forward + playerCamera.transform.right * spreadX + playerCamera.transform.up * spreadY).normalized;
        }

        /// <summary>
        /// Processes what the raycast hit.
        /// </summary>
        protected virtual void ProcessHit(RaycastHit hit)
        {
            // Check if we hit an enemy
            var enemyHealth = hit.collider.GetComponent<AI.EnemyHealth>();
            if (enemyHealth != null)
            {
                float damage = weaponData.damage;
                
                // Check for headshot
                if (hit.collider.CompareTag("Head"))
                {
                    damage *= weaponData.headShotMultiplier;
                }
                
                enemyHealth.TakeDamage(damage, DamageType.Bullet);
                
                // Spawn blood effect
                if (weaponData.bloodEffectPrefab != null)
                {
                    GameObject bloodEffect = Instantiate(weaponData.bloodEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(bloodEffect, 2f);
                }
            }
            
            // Spawn bullet hole
            if (weaponData.bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(weaponData.bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                bulletHole.transform.SetParent(hit.transform);
                Destroy(bulletHole, 10f);
            }
        }

        /// <summary>
        /// Plays firing visual effects.
        /// </summary>
        protected virtual void PlayFireEffects()
        {
            // Muzzle flash
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
            
            // Shell ejection
            if (weaponData.shellEjectionPrefab != null && shellEjectionPoint != null)
            {
                GameObject shell = Instantiate(weaponData.shellEjectionPrefab, shellEjectionPoint.position, shellEjectionPoint.rotation);
                Rigidbody shellRb = shell.GetComponent<Rigidbody>();
                if (shellRb != null)
                {
                    shellRb.AddForce(shellEjectionPoint.right * 3f + Vector3.up * 2f, ForceMode.Impulse);
                    shellRb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
                }
                Destroy(shell, 3f);
            }
            
            // Weapon animation
            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger("Fire");
            }
        }

        /// <summary>
        /// Applies recoil to the weapon.
        /// </summary>
        protected virtual void ApplyRecoil()
        {
            targetRecoil += new Vector3(
                weaponData.recoilAmount.x,
                Random.Range(-weaponData.recoilAmount.y, weaponData.recoilAmount.y),
                Random.Range(-weaponData.recoilAmount.z, weaponData.recoilAmount.z)
            );
        }

        /// <summary>
        /// Handles recoil interpolation.
        /// </summary>
        protected virtual void HandleRecoil()
        {
            targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, weaponData.recoilReturnSpeed * Time.deltaTime);
            currentRecoil = Vector3.Slerp(currentRecoil, targetRecoil, weaponData.recoilSpeed * Time.deltaTime);
            
            if (playerCamera != null)
            {
                playerCamera.transform.localRotation *= Quaternion.Euler(-currentRecoil);
            }
        }

        /// <summary>
        /// Reloads the weapon.
        /// </summary>
        public virtual void Reload()
        {
            if (isReloading || currentAmmo >= weaponData.magazineSize || reserveAmmo <= 0)
                return;

            StartCoroutine(ReloadCoroutine());
        }

        /// <summary>
        /// Coroutine for reloading the weapon.
        /// </summary>
        protected virtual IEnumerator ReloadCoroutine()
        {
            isReloading = true;
            
            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger("Reload");
            }
            
            if (weaponData.reloadSound != null)
            {
                AudioSource.PlayClipAtPoint(weaponData.reloadSound, transform.position);
            }
            
            yield return new WaitForSeconds(weaponData.reloadTime);
            
            int ammoNeeded = weaponData.magazineSize - currentAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);
            
            currentAmmo += ammoToReload;
            reserveAmmo -= ammoToReload;
            
            isReloading = false;
            OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
        }

        /// <summary>
        /// Sets whether the weapon is aiming down sights.
        /// </summary>
        public virtual void SetAiming(bool aiming)
        {
            isAiming = aiming;
        }

        /// <summary>
        /// Adds ammo to the weapon's reserve.
        /// </summary>
        public virtual void AddAmmo(int amount)
        {
            reserveAmmo = Mathf.Min(reserveAmmo + amount, weaponData.maxReserveAmmo);
            OnAmmoChanged?.Invoke(currentAmmo, reserveAmmo);
        }

        /// <summary>
        /// Called when the weapon is equipped.
        /// </summary>
        public virtual void OnEquip()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Called when the weapon is unequipped.
        /// </summary>
        public virtual void OnUnequip()
        {
            gameObject.SetActive(false);
            isAiming = false;
        }
    }
}
