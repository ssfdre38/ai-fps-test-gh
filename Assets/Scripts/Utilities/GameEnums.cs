namespace FPSGame.Utilities
{
    /// <summary>
    /// Types of weapons available in the game.
    /// </summary>
    public enum WeaponType
    {
        Pistol,
        AssaultRifle,
        Shotgun,
        SniperRifle
    }

    /// <summary>
    /// Fire modes for weapons.
    /// </summary>
    public enum FireMode
    {
        SemiAuto,
        Burst,
        FullAuto
    }

    /// <summary>
    /// Types of damage that can be dealt.
    /// </summary>
    public enum DamageType
    {
        Bullet,
        Explosive,
        Fall,
        Melee
    }

    /// <summary>
    /// AI states for enemy behavior.
    /// </summary>
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        TakeCover,
        Dead
    }

    /// <summary>
    /// Game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    /// <summary>
    /// Enemy types with different behaviors.
    /// </summary>
    public enum EnemyType
    {
        Basic,
        Heavy,
        Fast,
        Sniper
    }

    /// <summary>
    /// Audio types for different sounds.
    /// </summary>
    public enum AudioType
    {
        WeaponFire,
        Reload,
        Footstep,
        Impact,
        EnemyAlert,
        EnemyDeath,
        BackgroundMusic,
        UI
    }

    /// <summary>
    /// Surface types for different footstep sounds.
    /// </summary>
    public enum SurfaceType
    {
        Concrete,
        Metal,
        Wood,
        Grass,
        Water
    }
}
