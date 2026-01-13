using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.AI
{
    /// <summary>
    /// Cover state - enemy takes cover and occasionally fires from cover.
    /// </summary>
    public class CoverState : AIStateBase
    {
        private bool reachedCover;
        private float peekTimer;
        private float peekInterval = 3f;
        private bool isPeeking;

        public CoverState(EnemyAI enemyAI) : base(enemyAI) { }

        public override void OnEnter()
        {
            reachedCover = false;
            isPeeking = false;
            peekTimer = 0f;
            
            // Move to cover position
            enemyAI.NavAgent.SetDestination(enemyAI.CoverPosition);
        }

        public override void OnUpdate()
        {
            if (enemyAI.Target == null)
            {
                enemyAI.ChangeState(AIState.Patrol);
                return;
            }

            // Check if reached cover
            if (!reachedCover)
            {
                if (!enemyAI.NavAgent.pathPending && enemyAI.NavAgent.remainingDistance <= enemyAI.NavAgent.stoppingDistance)
                {
                    reachedCover = true;
                    enemyAI.SetInCover(true);
                }
                return;
            }

            // Peek from cover behavior
            peekTimer += Time.deltaTime;
            
            if (peekTimer >= peekInterval)
            {
                peekTimer = 0f;
                isPeeking = !isPeeking;
            }

            if (isPeeking)
            {
                // Peek and fire
                if (enemyAI.CanSeeTarget())
                {
                    // Look at target
                    Vector3 directionToTarget = (enemyAI.Target.position - enemyAI.transform.position).normalized;
                    directionToTarget.y = 0;
                    if (directionToTarget != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                        enemyAI.transform.rotation = Quaternion.Slerp(
                            enemyAI.transform.rotation,
                            targetRotation,
                            Time.deltaTime * 5f
                        );
                    }

                    enemyAI.FireAtTarget();
                }
            }

            // If health is good enough, leave cover
            if (enemyAI.GetComponent<EnemyHealth>().HealthPercentage > 0.6f)
            {
                enemyAI.SetInCover(false);
                
                if (enemyAI.IsTargetInAttackRange() && enemyAI.CanSeeTarget())
                {
                    enemyAI.ChangeState(AIState.Attack);
                }
                else
                {
                    enemyAI.ChangeState(AIState.Chase);
                }
            }
        }

        public override void OnExit()
        {
            enemyAI.SetInCover(false);
        }

        public override AIState GetStateType()
        {
            return AIState.TakeCover;
        }
    }
}
