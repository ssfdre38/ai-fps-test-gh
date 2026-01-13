using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.AI
{
    /// <summary>
    /// Chase state - enemy chases the target.
    /// </summary>
    public class ChaseState : AIStateBase
    {
        private float updatePathTimer;
        private const float pathUpdateInterval = 0.5f;

        public ChaseState(EnemyAI enemyAI) : base(enemyAI) { }

        public override void OnEnter()
        {
            updatePathTimer = 0f;
            
            if (enemyAI.Target != null)
            {
                enemyAI.NavAgent.SetDestination(enemyAI.Target.position);
            }
        }

        public override void OnUpdate()
        {
            if (enemyAI.Target == null)
            {
                enemyAI.ChangeState(AIState.Patrol);
                return;
            }

            // Check if target is in attack range
            if (enemyAI.IsTargetInAttackRange() && enemyAI.CanSeeTarget())
            {
                enemyAI.ChangeState(AIState.Attack);
                return;
            }

            // Check if lost sight of target for too long
            if (!enemyAI.CanSeeTarget())
            {
                // Could add a timer here to go back to patrol after losing sight
                // For now, continue chasing to last known position
            }

            // Update path periodically
            updatePathTimer += Time.deltaTime;
            if (updatePathTimer >= pathUpdateInterval)
            {
                updatePathTimer = 0f;
                enemyAI.NavAgent.SetDestination(enemyAI.Target.position);
            }

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
        }

        public override void OnExit()
        {
            // Clean up
        }

        public override AIState GetStateType()
        {
            return AIState.Chase;
        }
    }
}
