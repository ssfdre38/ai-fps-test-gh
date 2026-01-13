using UnityEngine;

namespace FPSGame.Weapons
{
    /// <summary>
    /// Handles projectile behavior for weapons that use projectiles instead of raycasts.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float damage = 50f;
        [SerializeField] private float speed = 50f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private float explosionRadius = 0f;
        [SerializeField] private bool isExplosive = false;
        
        [Header("Effects")]
        [SerializeField] private GameObject impactEffect;
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private LayerMask damageableLayers;

        private Rigidbody rb;
        private bool hasHit = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            // Set initial velocity
            if (rb != null)
            {
                rb.velocity = transform.forward * speed;
            }

            // Destroy after lifetime
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasHit)
                return;

            hasHit = true;

            if (isExplosive)
            {
                Explode();
            }
            else
            {
                DealDamage(collision.gameObject);
            }

            // Spawn impact effect
            if (impactEffect != null)
            {
                ContactPoint contact = collision.GetContact(0);
                GameObject effect = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
                Destroy(effect, 2f);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Deals damage to a single target.
        /// </summary>
        private void DealDamage(GameObject target)
        {
            var enemyHealth = target.GetComponent<AI.EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, Utilities.DamageType.Bullet);
            }
        }

        /// <summary>
        /// Handles explosion damage in a radius.
        /// </summary>
        private void Explode()
        {
            // Spawn explosion effect
            if (explosionEffect != null)
            {
                GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // Deal damage to all objects in radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);
            
            foreach (Collider hitCollider in hitColliders)
            {
                var enemyHealth = hitCollider.GetComponent<AI.EnemyHealth>();
                if (enemyHealth != null)
                {
                    // Calculate damage falloff based on distance
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    float damageMultiplier = 1f - (distance / explosionRadius);
                    float finalDamage = damage * damageMultiplier;
                    
                    enemyHealth.TakeDamage(finalDamage, Utilities.DamageType.Explosive);
                }
                
                // Apply explosion force
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(damage * 10f, transform.position, explosionRadius);
                }
            }
        }

        /// <summary>
        /// Sets the projectile damage.
        /// </summary>
        public void SetDamage(float newDamage)
        {
            damage = newDamage;
        }

        /// <summary>
        /// Sets the projectile speed.
        /// </summary>
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
            if (rb != null)
            {
                rb.velocity = transform.forward * speed;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (isExplosive)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, explosionRadius);
            }
        }
    }
}
