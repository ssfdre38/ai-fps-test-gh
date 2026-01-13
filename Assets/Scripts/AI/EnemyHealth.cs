using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.AI
{
    /// <summary>
    /// Manages enemy health and death.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        
        [Header("Ragdoll")]
        [SerializeField] private bool enableRagdoll = true;
        [SerializeField] private Rigidbody[] ragdollRigidbodies;
        
        [Header("Effects")]
        [SerializeField] private GameObject deathEffect;
        [SerializeField] private float despawnDelay = 10f;

        private bool isDead = false;
        private EnemyAI enemyAI;

        // Events
        public delegate void HealthChangedHandler(float current, float max);
        public event HealthChangedHandler OnHealthChanged;
        
        public delegate void DamagedHandler(float damage, DamageType damageType);
        public event DamagedHandler OnDamaged;
        
        public delegate void DeathHandler();
        public event DeathHandler OnDeath;

        /// <summary>
        /// Gets whether this enemy is dead.
        /// </summary>
        public bool IsDead => isDead;

        /// <summary>
        /// Gets the health percentage (0-1).
        /// </summary>
        public float HealthPercentage => currentHealth / maxHealth;

        private void Awake()
        {
            enemyAI = GetComponent<EnemyAI>();
            currentHealth = maxHealth;
            
            if (enableRagdoll)
            {
                SetRagdollEnabled(false);
            }
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Applies damage to this enemy.
        /// </summary>
        public void TakeDamage(float damage, DamageType damageType)
        {
            if (isDead)
                return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnDamaged?.Invoke(damage, damageType);

            // Notify AI of damage
            if (enemyAI != null)
            {
                enemyAI.OnTakeDamage(damage);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Heals this enemy.
        /// </summary>
        public void Heal(float amount)
        {
            if (isDead)
                return;

            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Handles enemy death.
        /// </summary>
        private void Die()
        {
            if (isDead)
                return;

            isDead = true;
            OnDeath?.Invoke();

            // Disable AI
            if (enemyAI != null)
            {
                enemyAI.enabled = false;
            }

            // Enable ragdoll
            if (enableRagdoll)
            {
                SetRagdollEnabled(true);
            }

            // Spawn death effect
            if (deathEffect != null)
            {
                GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(effect, 3f);
            }

            // Disable colliders except ragdoll
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                if (!col.GetComponent<Rigidbody>())
                {
                    col.enabled = false;
                }
            }

            // Schedule destruction
            Destroy(gameObject, despawnDelay);
        }

        /// <summary>
        /// Enables or disables ragdoll physics.
        /// </summary>
        private void SetRagdollEnabled(bool enabled)
        {
            if (ragdollRigidbodies == null || ragdollRigidbodies.Length == 0)
            {
                // Try to find ragdoll rigidbodies
                ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            }

            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != null)
                {
                    rb.isKinematic = !enabled;
                    
                    Collider col = rb.GetComponent<Collider>();
                    if (col != null)
                    {
                        col.enabled = enabled;
                    }
                }
            }

            // Disable animator when ragdoll is enabled
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = !enabled;
            }
        }

        /// <summary>
        /// Applies an explosion force to the ragdoll.
        /// </summary>
        public void ApplyExplosionForce(float force, Vector3 position, float radius)
        {
            if (!enableRagdoll || ragdollRigidbodies == null)
                return;

            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != null)
                {
                    rb.AddExplosionForce(force, position, radius);
                }
            }
        }
    }
}
