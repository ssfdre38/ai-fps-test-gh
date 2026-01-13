using FPSGame.Utilities;

namespace FPSGame.AI
{
    /// <summary>
    /// Abstract base class for AI states.
    /// </summary>
    public abstract class AIStateBase
    {
        protected EnemyAI enemyAI;

        /// <summary>
        /// Constructor for AI state.
        /// </summary>
        public AIStateBase(EnemyAI enemyAI)
        {
            this.enemyAI = enemyAI;
        }

        /// <summary>
        /// Called when entering this state.
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// Called every frame while in this state.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        public abstract void OnExit();

        /// <summary>
        /// Returns the state type.
        /// </summary>
        public abstract AIState GetStateType();
    }
}
