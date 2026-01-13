using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.AI
{
    /// <summary>
    /// Patrol state - enemy patrols between waypoints.
    /// </summary>
    public class PatrolState : AIStateBase
    {
        private int currentPatrolIndex;
        private float waitTimer;
        private bool isWaiting;

        public PatrolState(EnemyAI enemyAI) : base(enemyAI) { }

        public override void OnEnter()
        {
            currentPatrolIndex = 0;
            isWaiting = false;
            
            if (enemyAI.PatrolPoints != null && enemyAI.PatrolPoints.Length > 0)
            {
                MoveToNextPatrolPoint();
            }
        }

        public override void OnUpdate()
        {
            // Check if we can see the target
            if (enemyAI.CanSeeTarget())
            {
                enemyAI.ChangeState(AIState.Chase);
                return;
            }

            // Handle patrol behavior
            if (enemyAI.PatrolPoints == null || enemyAI.PatrolPoints.Length == 0)
            {
                // No patrol points, just stand idle
                return;
            }

            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0)
                {
                    isWaiting = false;
                    MoveToNextPatrolPoint();
                }
            }
            else
            {
                // Check if reached patrol point
                if (!enemyAI.NavAgent.pathPending && enemyAI.NavAgent.remainingDistance <= enemyAI.NavAgent.stoppingDistance)
                {
                    StartWaiting();
                }
            }
        }

        public override void OnExit()
        {
            // Clean up
        }

        public override AIState GetStateType()
        {
            return AIState.Patrol;
        }

        /// <summary>
        /// Moves to the next patrol point.
        /// </summary>
        private void MoveToNextPatrolPoint()
        {
            if (enemyAI.PatrolPoints.Length == 0)
                return;

            Transform targetPoint = enemyAI.PatrolPoints[currentPatrolIndex];
            
            if (targetPoint != null)
            {
                enemyAI.NavAgent.SetDestination(targetPoint.position);
            }

            currentPatrolIndex = (currentPatrolIndex + 1) % enemyAI.PatrolPoints.Length;
        }

        /// <summary>
        /// Starts waiting at patrol point.
        /// </summary>
        private void StartWaiting()
        {
            isWaiting = true;
            waitTimer = enemyAI.PatrolWaitTime;
            enemyAI.NavAgent.ResetPath();
        }
    }
}
