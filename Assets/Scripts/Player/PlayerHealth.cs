using UnityEngine;
using FPSGame.Utilities;
using System.Collections;

namespace FPSGame.Player
{
    /// <summary>
    /// Manages player health, damage, and regeneration.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        [Header("Armor Settings")]
        [SerializeField] private float maxArmor = 100f;
        [SerializeField] private float currentArmor;
        [SerializeField] private float armorDamageReduction = 0.5f; // 50% damage reduction
        
        [Header("Regeneration")]
        [SerializeField] private bool enableHealthRegeneration = true;
        [SerializeField] private float regenDelay = 5f; // Delay after taking damage
        [SerializeField] private float regenRate = 5f; // Health per second
        [SerializeField] private float regenThreshold = 100f; // Stop regenerating at this health
        
        [Header("Fall Damage")]
        [SerializeField] private float fallDamageThreshold = 10f;
        [SerializeField] private float fallDamageMultiplier = 10f;
        
        private float lastDamageTime;
        private bool isDead;
        private CharacterController characterController;
        private float lastYPosition;

        // Events
        public delegate void HealthChangedHandler(float current, float max);
        public event HealthChangedHandler OnHealthChanged;
        
        public delegate void ArmorChangedHandler(float current, float max);
        public event ArmorChangedHandler OnArmorChanged;
        
        public delegate void DamageReceivedHandler(float damage, DamageType damageType, Vector3 damageDirection);
        public event DamageReceivedHandler OnDamageReceived;
        
        public delegate void DeathHandler();
        public event DeathHandler OnDeath;

        /// <summary>
        /// Gets whether the player is dead.
        /// </summary>
        public bool IsDead => isDead;

        /// <summary>
        /// Gets the current health percentage (0-1).
        /// </summary>
        public float HealthPercentage => currentHealth / maxHealth;

        /// <summary>
        /// Gets the current armor percentage (0-1).
        /// </summary>
        public float ArmorPercentage => currentArmor / maxArmor;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            currentHealth = maxHealth;
            currentArmor = 0f;
            lastYPosition = transform.position.y;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
        }

        private void Update()
        {
            HandleRegeneration();
            HandleFallDamage();
        }

        /// <summary>
        /// Handles health regeneration.
        /// </summary>
        private void HandleRegeneration()
        {
            if (!enableHealthRegeneration || isDead || currentHealth >= regenThreshold)
                return;

            if (Time.time - lastDamageTime >= regenDelay)
            {
                currentHealth += regenRate * Time.deltaTime;
                currentHealth = Mathf.Min(currentHealth, regenThreshold);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }

        /// <summary>
        /// Handles fall damage calculation.
        /// </summary>
        private void HandleFallDamage()
        {
            if (characterController == null || !characterController.isGrounded)
            {
                lastYPosition = transform.position.y;
                return;
            }

            float fallDistance = lastYPosition - transform.position.y;
            
            if (fallDistance > fallDamageThreshold)
            {
                float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;
                TakeDamage(damage, DamageType.Fall, Vector3.up);
            }
            
            lastYPosition = transform.position.y;
        }

        /// <summary>
        /// Applies damage to the player.
        /// </summary>
        /// <param name="damage">Amount of damage</param>
        /// <param name="damageType">Type of damage</param>
        /// <param name="damageDirection">Direction the damage came from</param>
        public void TakeDamage(float damage, DamageType damageType, Vector3 damageDirection)
        {
            if (isDead)
                return;

            lastDamageTime = Time.time;

            // Apply armor damage reduction
            if (currentArmor > 0)
            {
                float armorDamage = damage * armorDamageReduction;
                float healthDamage = damage * (1f - armorDamageReduction);
                
                currentArmor -= armorDamage;
                
                if (currentArmor < 0)
                {
                    // Overflow damage to health
                    healthDamage += Mathf.Abs(currentArmor);
                    currentArmor = 0;
                }
                
                damage = healthDamage;
                OnArmorChanged?.Invoke(currentArmor, maxArmor);
            }

            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnDamageReceived?.Invoke(damage, damageType, damageDirection);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Heals the player.
        /// </summary>
        /// <param name="amount">Amount to heal</param>
        public void Heal(float amount)
        {
            if (isDead)
                return;

            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Adds armor to the player.
        /// </summary>
        /// <param name="amount">Amount of armor to add</param>
        public void AddArmor(float amount)
        {
            if (isDead)
                return;

            currentArmor += amount;
            currentArmor = Mathf.Min(currentArmor, maxArmor);
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
        }

        /// <summary>
        /// Handles player death.
        /// </summary>
        private void Die()
        {
            if (isDead)
                return;

            isDead = true;
            OnDeath?.Invoke();
            
            // Disable player controls
            var fpsController = GetComponent<FPSController>();
            if (fpsController != null)
            {
                fpsController.SetControlsEnabled(false);
            }
            
            Debug.Log("Player died!");
        }

        /// <summary>
        /// Respawns the player.
        /// </summary>
        public void Respawn()
        {
            isDead = false;
            currentHealth = maxHealth;
            currentArmor = 0f;
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
            
            // Re-enable player controls
            var fpsController = GetComponent<FPSController>();
            if (fpsController != null)
            {
                fpsController.SetControlsEnabled(true);
            }
        }

        /// <summary>
        /// Sets the player to invulnerable for a duration.
        /// </summary>
        public void SetInvulnerable(float duration)
        {
            StartCoroutine(InvulnerabilityCoroutine(duration));
        }

        private IEnumerator InvulnerabilityCoroutine(float duration)
        {
            float tempMaxHealth = maxHealth;
            maxHealth = float.MaxValue; // Temporary invulnerability
            
            yield return new WaitForSeconds(duration);
            
            maxHealth = tempMaxHealth;
        }
    }
}
