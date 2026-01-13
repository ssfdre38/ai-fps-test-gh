using UnityEngine;
using FPSGame.Utilities;

namespace FPSGame.AI
{
    /// <summary>
    /// Attack state - enemy attacks the target.
    /// </summary>
    public class AttackState : AIStateBase
    {
        private float strafeTimer;
        private float strafeDirection = 1f;
        private const float strafeInterval = 2f;

        public AttackState(EnemyAI enemyAI) : base(enemyAI) { }

        public override void OnEnter()
        {
            enemyAI.NavAgent.ResetPath();
            strafeTimer = 0f;
        }

        public override void OnUpdate()
        {
            if (enemyAI.Target == null)
            {
                enemyAI.ChangeState(AIState.Patrol);
                return;
            }

            // If target is out of attack range, chase
            if (!enemyAI.IsTargetInAttackRange())
            {
                enemyAI.ChangeState(AIState.Chase);
                return;
            }

            // If can't see target, chase
            if (!enemyAI.CanSeeTarget())
            {
                enemyAI.ChangeState(AIState.Chase);
                return;
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
                    Time.deltaTime * 10f
                );
            }

            // Strafe behavior for variety
            HandleStrafing();

            // Fire at target
            enemyAI.FireAtTarget();
        }

        public override void OnExit()
        {
            enemyAI.NavAgent.ResetPath();
        }

        public override AIState GetStateType()
        {
            return AIState.Attack;
        }

        /// <summary>
        /// Handles strafing movement during attack.
        /// </summary>
        private void HandleStrafing()
        {
            strafeTimer += Time.deltaTime;
            
            if (strafeTimer >= strafeInterval)
            {
                strafeTimer = 0f;
                strafeDirection = -strafeDirection;
            }

            // Calculate strafe position
            Vector3 strafePos = enemyAI.transform.position + enemyAI.transform.right * strafeDirection * 2f;
            
            // Only strafe if position is valid on NavMesh
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(strafePos, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
            {
                enemyAI.NavAgent.SetDestination(hit.position);
            }
        }
    }
}
