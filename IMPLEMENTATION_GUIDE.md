# Unity FPS Game Framework - Complete Implementation Guide

## 🎮 Overview

This is a production-ready, modular First-Person Shooter (FPS) game framework built with C# for Unity. It features professional architecture, comprehensive gameplay systems, and follows industry best practices for game development.

## ✨ Features

### Core Systems
- **Player Controller**: Smooth WASD movement, sprint, jump, crouch, and head bobbing
- **Advanced Weapon System**: Multiple weapon types with realistic mechanics
- **AI System**: NavMesh-based pathfinding with state machine behavior
- **Health & Damage System**: Comprehensive damage types and health management
- **UI System**: Complete HUD, menus, and settings
- **Game Management**: Singleton-based game state and flow control
- **Audio System**: Spatial 3D audio with volume controls
- **Visual Effects**: Particle systems and pooling

### Weapon Types
1. **Pistol** - Semi-automatic, balanced damage
2. **Assault Rifle** - Full-auto, medium range
3. **Shotgun** - Multiple pellets, close range
4. **Sniper Rifle** - High damage, long range with scope

### AI Features
- State Machine (Patrol, Chase, Attack, Take Cover)
- Line of sight detection
- Cover system with dynamic cover finding
- Squad coordination basics
- Ragdoll death physics

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── FPSController.cs          # Player movement and input
│   │   ├── PlayerHealth.cs           # Health, damage, regeneration
│   │   ├── PlayerAudio.cs            # Footstep sounds
│   │   └── CameraController.cs       # Mouse look and weapon sway
│   ├── Weapons/
│   │   ├── WeaponBase.cs            # Abstract base class
│   │   ├── WeaponManager.cs         # Weapon switching
│   │   ├── WeaponData.cs            # ScriptableObject for config
│   │   ├── AssaultRifle.cs          # Full-auto implementation
│   │   ├── Pistol.cs                # Semi-auto implementation
│   │   ├── Shotgun.cs               # Multi-pellet implementation
│   │   ├── SniperRifle.cs           # High-precision implementation
│   │   └── Projectile.cs            # Projectile physics
│   ├── AI/
│   │   ├── EnemyAI.cs               # Main AI controller
│   │   ├── EnemyHealth.cs           # Enemy health and death
│   │   ├── AIStateBase.cs           # Abstract state class
│   │   ├── PatrolState.cs           # Patrol behavior
│   │   ├── ChaseState.cs            # Chase behavior
│   │   ├── AttackState.cs           # Attack behavior
│   │   └── CoverState.cs            # Cover behavior
│   ├── Systems/
│   │   ├── GameManager.cs           # Main game state manager
│   │   ├── WaveSpawner.cs           # Enemy wave spawning
│   │   ├── ObjectPooler.cs          # Object pooling system
│   │   └── AudioManager.cs          # Audio management
│   ├── UI/
│   │   ├── HUDManager.cs            # In-game HUD
│   │   ├── MainMenu.cs              # Main menu
│   │   ├── PauseMenu.cs             # Pause menu
│   │   └── SettingsMenu.cs          # Settings management
│   └── Utilities/
│       ├── Singleton.cs             # Generic singleton pattern
│       └── GameEnums.cs             # All game enumerations
├── Scenes/
│   ├── MainMenu.unity               # Main menu scene
│   ├── Level01.unity                # First level
│   └── TestArena.unity              # Test scene
├── Prefabs/
│   ├── Player.prefab
│   ├── Weapons/
│   ├── Enemies/
│   └── Effects/
└── ScriptableObjects/
    └── WeaponData/
```

## 🚀 Quick Start Guide

### 1. Unity Setup

**Requirements:**
- Unity 2021.3 LTS or newer
- TextMeshPro package (should be auto-imported)
- Unity Input System (optional, currently uses legacy input)

### 2. Scene Setup

#### Main Menu Scene
1. Create a new scene named "MainMenu"
2. Add a Canvas (UI > Canvas)
3. Add a GameObject and attach the `MainMenu.cs` script
4. Configure buttons in the inspector

#### Game Level Scene
1. Create a new scene named "Level01"
2. Add the following GameObjects:

**Game Manager Setup:**
```
- GameObject "GameManager"
  - Add GameManager.cs component
  - Set player spawn point
  - Assign player prefab
```

**Player Setup:**
```
- GameObject "Player" (Tag: Player)
  - Add CharacterController component
  - Add FPSController.cs
  - Add PlayerHealth.cs
  - Add PlayerAudio.cs
  - Add AudioSource component
  
  - Child: "Camera" (Tag: MainCamera)
    - Add Camera component
    - Add CameraController.cs
    - Add AudioListener component
    
  - Child: "WeaponHolder"
    - Add WeaponManager.cs
    
    - Child: "Pistol"
      - Add Pistol.cs
      - Add weapon model
```

**Enemy Setup:**
```
- GameObject "Enemy" (Tag: Enemy)
  - Add NavMeshAgent component
  - Add EnemyAI.cs
  - Add EnemyHealth.cs
  - Add Animator component
  - Add enemy model with rigidbodies for ragdoll
  - Create patrol point transforms
```

**Wave Spawner Setup:**
```
- GameObject "WaveSpawner"
  - Add WaveSpawner.cs
  - Assign enemy prefabs
  - Create spawn point transforms
```

**Audio Manager Setup:**
```
- GameObject "AudioManager"
  - Add AudioManager.cs
  - Configure sound clips
```

**UI Setup:**
```
- Canvas
  - HUDManager (add HUDManager.cs)
  - PauseMenu (add PauseMenu.cs)
  - SettingsMenu (add SettingsMenu.cs)
```

### 3. NavMesh Setup

1. Select all walkable surfaces
2. Open Window > AI > Navigation
3. Mark surfaces as "Walkable"
4. Click "Bake"

### 4. Creating Weapon Data

1. Right-click in Project window
2. Create > FPS Game > Weapon Data
3. Configure weapon properties:
   - Damage, fire rate, ammo capacity
   - Recoil, spread, accuracy
   - Audio clips
   - Effect prefabs

### 5. Layer Setup

Create the following layers:
- Player
- Enemy
- Ground
- Cover
- Projectile

Configure collision matrix (Edit > Project Settings > Physics):
- Player shouldn't collide with player projectiles
- Enemies shouldn't collide with enemy projectiles

### 6. Tag Setup

Create the following tags:
- Player
- Enemy
- Ground
- Cover
- Head (for headshot detection)

## 🎯 Usage Examples

### Creating a New Weapon

```csharp
// 1. Create a new weapon class
public class GrenadeLauncher : WeaponBase
{
    [SerializeField] private GameObject grenadePrefab;
    
    protected override void PerformShot()
    {
        // Custom grenade launching logic
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
        // Configure grenade...
    }
}

// 2. Create WeaponData ScriptableObject
// 3. Assign to weapon prefab
// 4. Add to WeaponManager's weapon list
```

### Creating a Custom AI State

```csharp
public class FleeState : AIStateBase
{
    public FleeState(EnemyAI enemyAI) : base(enemyAI) { }
    
    public override void OnEnter()
    {
        // Run away from player
    }
    
    public override void OnUpdate()
    {
        // Continue fleeing logic
    }
    
    public override void OnExit()
    {
        // Cleanup
    }
    
    public override AIState GetStateType()
    {
        return AIState.Idle; // Add new state to enum
    }
}
```

### Spawning Enemies

```csharp
// Use WaveSpawner
WaveSpawner spawner = FindObjectOfType<WaveSpawner>();
spawner.TriggerNextWave();

// Or spawn manually
GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
```

### Playing Audio

```csharp
// Play 2D sound
AudioManager.Instance.Play("WeaponFire");

// Play 3D sound at position
AudioManager.Instance.Play3D("Explosion", transform.position);
```

### Managing Game State

```csharp
// Pause game
GameManager.Instance.PauseGame();

// Resume game
GameManager.Instance.ResumeGame();

// Add score
GameManager.Instance.AddScore(100);

// Trigger game over
GameManager.Instance.GameOver();
```

## 🎨 Design Patterns Used

### Singleton Pattern
Used for managers that need global access:
- GameManager
- AudioManager
- ObjectPooler

### State Machine Pattern
Used for AI behavior:
- AIStateBase (abstract)
- PatrolState, ChaseState, AttackState, CoverState

### Object Pool Pattern
Used for frequently instantiated objects:
- Bullets
- Particles
- Shell casings

### Observer Pattern
Event-driven communication:
- Weapon events (OnAmmoChanged, OnWeaponFired)
- Health events (OnHealthChanged, OnDeath)
- Game events (OnGameStateChanged, OnScoreChanged)

### Strategy Pattern
Weapon behavior variations:
- WeaponBase (abstract strategy)
- Specific weapon implementations

## 📝 Code Quality Features

### XML Documentation
All public methods include XML documentation:
```csharp
/// <summary>
/// Applies damage to the player.
/// </summary>
/// <param name="damage">Amount of damage</param>
/// <param name="damageType">Type of damage</param>
public void TakeDamage(float damage, DamageType damageType)
```

### Naming Conventions
- PascalCase for public members
- camelCase for private members
- Descriptive names

### Encapsulation
- Private fields with public properties
- Protected methods for inheritance
- Clear interfaces

### Event-Driven Architecture
- Loose coupling between systems
- Subscribe/unsubscribe pattern
- Clean event handlers

## ⚡ Performance Optimization

### Object Pooling
```csharp
// Use ObjectPooler for bullets
GameObject bullet = ObjectPooler.Instance.SpawnFromPool("Bullet", position, rotation);
```

### Efficient Updates
- Minimal Update() calls
- Cached component references
- Distance checks before expensive operations

### Memory Management
- Proper cleanup in OnDestroy()
- Event unsubscription
- Destroy temporary objects

## 🔧 Configuration

### Input Configuration
Current input uses Unity's legacy Input Manager. To customize:
1. Edit > Project Settings > Input Manager
2. Configure axes:
   - Horizontal (A/D)
   - Vertical (W/S)
   - Mouse X
   - Mouse Y
   - Fire1 (Left Mouse)
   - Fire2 (Right Mouse)
   - Jump (Space)

### Audio Configuration
Configure AudioManager sounds:
```csharp
// Add sounds in inspector or via code
AudioManager.Instance.AddSound("CustomSound", audioClip, AudioType.SFX);
```

### Graphics Settings
Adjust in SettingsMenu.cs:
- Quality levels
- Resolution
- Fullscreen
- VSync

## 🐛 Debugging

### Debug Visualization
Enable Gizmos in Scene view to see:
- Enemy detection ranges (yellow sphere)
- Enemy attack ranges (red sphere)
- Enemy field of view (blue lines)
- Spawn points (red spheres)

### Console Logging
Debug statements in key locations:
- Player death
- Enemy state changes
- Weapon firing
- Damage events

## 📊 Extending the Framework

### Adding New Enemy Types
1. Create enemy prefab
2. Add EnemyAI and EnemyHealth components
3. Configure in inspector
4. Add to WaveSpawner enemy array

### Adding New Weapons
1. Inherit from WeaponBase
2. Override PerformShot() for custom behavior
3. Create WeaponData ScriptableObject
4. Add to WeaponManager

### Adding New UI Elements
1. Create UI GameObject
2. Attach appropriate script
3. Wire up events
4. Update HUDManager if needed

## 🎓 Best Practices

1. **Always use object pooling** for frequently spawned objects
2. **Cache component references** in Awake() or Start()
3. **Use events** for cross-system communication
4. **Test in builds** not just in editor
5. **Profile performance** regularly
6. **Follow the existing code style**
7. **Document public APIs** with XML comments
8. **Use ScriptableObjects** for configuration data

## 🔍 Common Issues and Solutions

### Player Falls Through Floor
- Ensure CharacterController is properly configured
- Check collision layers
- Verify ground has collider

### Enemies Don't Move
- Bake NavMesh (Window > AI > Navigation)
- Check NavMeshAgent settings
- Ensure patrol points are assigned

### Weapons Don't Fire
- Check ammo count
- Verify firePoint is assigned
- Check weapon data configuration

### Audio Not Playing
- Ensure AudioManager has sounds configured
- Check volume settings
- Verify audio clips are assigned

### UI Not Responding
- Check Canvas scaler settings
- Verify button onClick events
- Ensure EventSystem exists in scene

## 📄 License

This framework is provided as-is for educational and commercial use.

## 🤝 Contributing

When contributing:
1. Follow existing code style
2. Add XML documentation
3. Test thoroughly
4. Update this README if needed

## 📞 Support

For issues or questions:
1. Check this documentation
2. Review code comments
3. Check Unity console for errors
4. Review component settings in inspector

---

**Happy Game Development! 🎮**
