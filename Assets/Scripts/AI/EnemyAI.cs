using UnityEngine;
using UnityEngine.AI;
using FPSGame.Utilities;
using System.Collections.Generic;

namespace FPSGame.AI
{
    /// <summary>
    /// Main AI controller for enemies. Manages state machine and behavior.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private EnemyType enemyType = EnemyType.Basic;
        [SerializeField] private float detectionRange = 20f;
        [SerializeField] private float attackRange = 10f;
        [SerializeField] private float fieldOfView = 90f;
        
        [Header("Combat")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;
        
        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 2f;
        
        [Header("Cover")]
        [SerializeField] private float coverHealthThreshold = 30f;
        [SerializeField] private float coverSearchRadius = 15f;
        [SerializeField] private LayerMask coverMask;
        
        [Header("References")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private Animator animator;

        // Components
        private NavMeshAgent navAgent;
        private EnemyHealth health;
        
        // State Machine
        private AIStateBase currentState;
        private Dictionary<AIState, AIStateBase> states;
        
        // Target
        private Transform target;
        private float lastFireTime;
        
        // Cover
        private Vector3 coverPosition;
        private bool isInCover;

        // Events
        public delegate void StateChangedHandler(AIState newState);
        public event StateChangedHandler OnStateChanged;

        /// <summary>
        /// Gets the NavMeshAgent component.
        /// </summary>
        public NavMeshAgent NavAgent => navAgent;

        /// <summary>
        /// Gets the current target.
        /// </summary>
        public Transform Target => target;

        /// <summary>
        /// Gets the current state type.
        /// </summary>
        public AIState CurrentStateType => currentState?.GetStateType() ?? AIState.Idle;

        /// <summary>
        /// Gets the patrol points.
        /// </summary>
        public Transform[] PatrolPoints => patrolPoints;

        /// <summary>
        /// Gets the patrol wait time.
        /// </summary>
        public float PatrolWaitTime => patrolWaitTime;

        /// <summary>
        /// Gets the attack range.
        /// </summary>
        public float AttackRange => attackRange;

        /// <summary>
        /// Gets the damage this enemy deals.
        /// </summary>
        public float Damage => damage;

        /// <summary>
        /// Gets the fire rate.
        /// </summary>
        public float FireRate => fireRate;

        /// <summary>
        /// Gets the fire point transform.
        /// </summary>
        public Transform FirePoint => firePoint;

        /// <summary>
        /// Gets the animator.
        /// </summary>
        public Animator Animator => animator;

        /// <summary>
        /// Gets whether this enemy is in cover.
        /// </summary>
        public bool IsInCover => isInCover;

        /// <summary>
        /// Gets or sets the cover position.
        /// </summary>
        public Vector3 CoverPosition
        {
            get => coverPosition;
            set => coverPosition = value;
        }

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<EnemyHealth>();
            
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Start()
        {
            InitializeStates();
            ChangeState(AIState.Patrol);
            
            // Find player as target
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        private void Update()
        {
            if (health.IsDead)
                return;

            currentState?.OnUpdate();
            UpdateAnimator();
        }

        /// <summary>
        /// Initializes all AI states.
        /// </summary>
        private void InitializeStates()
        {
            states = new Dictionary<AIState, AIStateBase>
            {
                { AIState.Patrol, new PatrolState(this) },
                { AIState.Chase, new ChaseState(this) },
                { AIState.Attack, new AttackState(this) },
                { AIState.TakeCover, new CoverState(this) }
            };
        }

        /// <summary>
        /// Changes to a new state.
        /// </summary>
        public void ChangeState(AIState newState)
        {
            currentState?.OnExit();
            
            if (states.ContainsKey(newState))
            {
                currentState = states[newState];
                currentState.OnEnter();
                OnStateChanged?.Invoke(newState);
            }
        }

        /// <summary>
        /// Checks if the target is visible.
        /// </summary>
        public bool CanSeeTarget()
        {
            if (target == null)
                return false;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Check distance
            if (distanceToTarget > detectionRange)
                return false;

            // Check field of view
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            if (angle > fieldOfView / 2f)
                return false;

            // Check line of sight
            if (Physics.Raycast(transform.position + Vector3.up, directionToTarget, distanceToTarget, obstacleMask))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the target is in attack range.
        /// </summary>
        public bool IsTargetInAttackRange()
        {
            if (target == null)
                return false;

            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= attackRange;
        }

        /// <summary>
        /// Attempts to fire at the target.
        /// </summary>
        public void FireAtTarget()
        {
            if (target == null || Time.time - lastFireTime < 1f / fireRate)
                return;

            lastFireTime = Time.time;

            // Perform raycast to target
            Vector3 directionToTarget = (target.position - (firePoint != null ? firePoint.position : transform.position)).normalized;
            
            if (Physics.Raycast(firePoint != null ? firePoint.position : transform.position, directionToTarget, out RaycastHit hit, attackRange))
            {
                // Check if we hit the player
                var playerHealth = hit.collider.GetComponent<Player.PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage, DamageType.Bullet, directionToTarget);
                }
            }

            // Trigger fire animation
            if (animator != null)
            {
                animator.SetTrigger("Fire");
            }
        }

        /// <summary>
        /// Called when this enemy takes damage.
        /// </summary>
        public void OnTakeDamage(float damage)
        {
            // If health is low, seek cover
            if (health.HealthPercentage * 100f <= coverHealthThreshold && currentState.GetStateType() != AIState.TakeCover)
            {
                if (FindCoverPosition())
                {
                    ChangeState(AIState.TakeCover);
                }
            }
        }

        /// <summary>
        /// Finds a suitable cover position.
        /// </summary>
        public bool FindCoverPosition()
        {
            Collider[] coverObjects = Physics.OverlapSphere(transform.position, coverSearchRadius, coverMask);
            
            if (coverObjects.Length == 0)
                return false;

            // Find the best cover position (furthest from target, closest to us)
            float bestScore = float.MinValue;
            Vector3 bestPosition = transform.position;

            foreach (Collider coverObject in coverObjects)
            {
                Vector3 coverPos = coverObject.transform.position;
                
                // Calculate score based on distance to target and distance to us
                float distanceToTarget = target != null ? Vector3.Distance(coverPos, target.position) : 0;
                float distanceToUs = Vector3.Distance(coverPos, transform.position);
                
                float score = distanceToTarget - (distanceToUs * 0.5f);
                
                if (score > bestScore)
                {
                    // Check if position is reachable
                    NavMeshPath path = new NavMeshPath();
                    if (navAgent.CalculatePath(coverPos, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        bestScore = score;
                        bestPosition = coverPos;
                    }
                }
            }

            if (bestScore > float.MinValue)
            {
                coverPosition = bestPosition;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates animator parameters.
        /// </summary>
        private void UpdateAnimator()
        {
            if (animator == null)
                return;

            // Update speed parameter
            float speed = navAgent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            
            // Update state parameters
            animator.SetBool("IsInCover", isInCover);
        }

        /// <summary>
        /// Sets whether this enemy is in cover.
        /// </summary>
        public void SetInCover(bool inCover)
        {
            isInCover = inCover;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Draw field of view
            Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
            Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2f, Vector3.up) * transform.forward * detectionRange;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + fovLine1);
            Gizmos.DrawLine(transform.position, transform.position + fovLine2);
        }
    }
}
