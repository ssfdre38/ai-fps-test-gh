# API Reference

Quick reference for the most commonly used classes and methods in the Unity FPS Framework.

## Player System

### FPSController

Controls player movement and input handling.

```csharp
public class FPSController : MonoBehaviour
```

**Properties:**
- `bool IsSprinting` - Whether player is sprinting
- `bool IsCrouching` - Whether player is crouching
- `bool IsGrounded` - Whether player is on ground

**Methods:**
- `Vector3 GetVelocity()` - Returns current movement velocity
- `void SetControlsEnabled(bool enabled)` - Enable/disable player controls

---

### PlayerHealth

Manages player health, damage, and regeneration.

```csharp
public class PlayerHealth : MonoBehaviour
```

**Properties:**
- `bool IsDead` - Whether player is dead
- `float HealthPercentage` - Current health as 0-1 value
- `float ArmorPercentage` - Current armor as 0-1 value

**Methods:**
- `void TakeDamage(float damage, DamageType type, Vector3 direction)` - Apply damage
- `void Heal(float amount)` - Restore health
- `void AddArmor(float amount)` - Add armor
- `void Respawn()` - Respawn the player

**Events:**
- `event HealthChangedHandler OnHealthChanged` - Fired when health changes
- `event ArmorChangedHandler OnArmorChanged` - Fired when armor changes
- `event DamageReceivedHandler OnDamageReceived` - Fired when damage taken
- `event DeathHandler OnDeath` - Fired on player death

---

### CameraController

Controls camera rotation and weapon sway.

```csharp
public class CameraController : MonoBehaviour
```

**Properties:**
- `float MouseSensitivity` - Mouse sensitivity (0.1-10)

**Methods:**
- `void ApplyCameraShake(float intensity, float duration)` - Shake camera
- `void ResetRotation()` - Reset camera to zero rotation
- `void SetControlsEnabled(bool enabled)` - Enable/disable camera controls

---

## Weapon System

### WeaponBase

Abstract base class for all weapons.

```csharp
public abstract class WeaponBase : MonoBehaviour
```

**Properties:**
- `WeaponData WeaponData` - Weapon configuration data
- `bool IsReloading` - Whether weapon is reloading
- `int CurrentAmmo` - Current magazine ammo
- `int ReserveAmmo` - Reserve ammo count

**Methods:**
- `bool Fire()` - Attempt to fire weapon
- `void Reload()` - Reload weapon
- `void SetAiming(bool aiming)` - Set ADS state
- `void AddAmmo(int amount)` - Add reserve ammo
- `void OnEquip()` - Called when equipped
- `void OnUnequip()` - Called when unequipped

**Events:**
- `event AmmoChangedHandler OnAmmoChanged` - Fired when ammo changes
- `event WeaponFiredHandler OnWeaponFired` - Fired when weapon fires

---

### WeaponManager

Manages weapon switching and input.

```csharp
public class WeaponManager : MonoBehaviour
```

**Properties:**
- `WeaponBase CurrentWeapon` - Currently equipped weapon
- `int CurrentWeaponIndex` - Current weapon index

**Methods:**
- `void SwitchToWeapon(int index)` - Switch to weapon by index
- `void SwitchToNextWeapon()` - Switch to next weapon
- `void SwitchToPreviousWeapon()` - Switch to previous weapon
- `bool Fire()` - Fire current weapon
- `void Reload()` - Reload current weapon
- `void SetAiming(bool aiming)` - Set aiming for current weapon
- `void AddWeapon(WeaponBase weapon)` - Add weapon to inventory
- `void AddAmmoToWeapon(int index, int amount)` - Add ammo to weapon

**Events:**
- `event WeaponChangedHandler OnWeaponChanged` - Fired when weapon changes

---

## AI System

### EnemyAI

Main AI controller with state machine.

```csharp
public class EnemyAI : MonoBehaviour
```

**Properties:**
- `NavMeshAgent NavAgent` - NavMesh agent component
- `Transform Target` - Current target
- `AIState CurrentStateType` - Current AI state
- `Transform[] PatrolPoints` - Patrol waypoints
- `float AttackRange` - Attack range distance
- `bool IsInCover` - Whether AI is in cover

**Methods:**
- `void ChangeState(AIState newState)` - Change to new state
- `bool CanSeeTarget()` - Check if target is visible
- `bool IsTargetInAttackRange()` - Check if target in range
- `void FireAtTarget()` - Fire weapon at target
- `void OnTakeDamage(float damage)` - Called when damaged
- `bool FindCoverPosition()` - Find suitable cover
- `void SetInCover(bool inCover)` - Set cover state

**Events:**
- `event StateChangedHandler OnStateChanged` - Fired when state changes

---

### EnemyHealth

Manages enemy health and death.

```csharp
public class EnemyHealth : MonoBehaviour
```

**Properties:**
- `bool IsDead` - Whether enemy is dead
- `float HealthPercentage` - Health as 0-1 value

**Methods:**
- `void TakeDamage(float damage, DamageType type)` - Apply damage
- `void Heal(float amount)` - Restore health
- `void ApplyExplosionForce(float force, Vector3 position, float radius)` - Apply explosion

**Events:**
- `event HealthChangedHandler OnHealthChanged` - Fired when health changes
- `event DamagedHandler OnDamaged` - Fired when damaged
- `event DeathHandler OnDeath` - Fired on death

---

## Game Systems

### GameManager

Main game state manager (Singleton).

```csharp
public class GameManager : Singleton<GameManager>
```

**Properties:**
- `GameState CurrentGameState` - Current game state
- `int CurrentScore` - Current score
- `int CurrentWave` - Current wave number
- `float ScoreMultiplier` - Score multiplier
- `GameObject PlayerInstance` - Player instance

**Methods:**
- `void ChangeGameState(GameState state)` - Change game state
- `void PauseGame()` - Pause the game
- `void ResumeGame()` - Resume the game
- `void AddScore(int points)` - Add to score
- `void SetScoreMultiplier(float multiplier)` - Set score multiplier
- `void IncrementWave()` - Increment wave counter
- `void GameOver()` - Trigger game over
- `void Victory()` - Trigger victory
- `void RestartLevel()` - Restart current level
- `void LoadMainMenu()` - Load main menu
- `void LoadLevel(string levelName)` - Load specific level
- `void QuitGame()` - Quit application
- `void RespawnPlayer()` - Respawn player

**Events:**
- `event GameStateChangedHandler OnGameStateChanged`
- `event ScoreChangedHandler OnScoreChanged`
- `event WaveChangedHandler OnWaveChanged`

**Usage:**
```csharp
GameManager.Instance.AddScore(100);
GameManager.Instance.PauseGame();
```

---

### AudioManager

Manages all game audio (Singleton).

```csharp
public class AudioManager : Singleton<AudioManager>
```

**Properties:**
- `float MasterVolume` - Master volume (0-1)
- `float MusicVolume` - Music volume (0-1)
- `float SFXVolume` - SFX volume (0-1)

**Methods:**
- `void Play(string soundName)` - Play 2D sound
- `void Play3D(string soundName, Vector3 position)` - Play 3D sound at position
- `void Stop(string soundName)` - Stop sound
- `void Pause(string soundName)` - Pause sound
- `void Resume(string soundName)` - Resume sound
- `bool IsPlaying(string soundName)` - Check if playing
- `void AddSound(string name, AudioClip clip, AudioType type, ...)` - Add sound at runtime
- `void StopAll()` - Stop all sounds
- `void PauseAll()` - Pause all sounds
- `void ResumeAll()` - Resume all sounds

**Usage:**
```csharp
AudioManager.Instance.Play("WeaponFire");
AudioManager.Instance.Play3D("Explosion", transform.position);
```

---

### WaveSpawner

Manages enemy wave spawning.

```csharp
public class WaveSpawner : MonoBehaviour
```

**Properties:**
- `int CurrentWave` - Current wave number
- `int EnemiesAlive` - Number of enemies alive

**Methods:**
- `void StartNextWave()` - Start next wave
- `void TriggerNextWave()` - Manually trigger wave
- `void StopSpawning()` - Stop spawning and clear enemies

**Events:**
- `event WaveStartedHandler OnWaveStarted`
- `event WaveCompletedHandler OnWaveCompleted`
- `event EnemySpawnedHandler OnEnemySpawned`

---

### ObjectPooler

Object pooling system for performance.

```csharp
public class ObjectPooler : MonoBehaviour
```

**Methods:**
- `GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rot)` - Spawn pooled object
- `void ReturnToPool(GameObject obj)` - Return object to pool
- `void CreatePool(string tag, GameObject prefab, int size)` - Create new pool

**Usage:**
```csharp
GameObject bullet = ObjectPooler.Instance.SpawnFromPool("Bullet", position, rotation);
```

---

## UI System

### HUDManager

Manages the heads-up display.

```csharp
public class HUDManager : MonoBehaviour
```

**Methods:**
- `void UpdateCrosshairSpread(float spread)` - Update crosshair
- `void AddKillFeed(string killer, string victim)` - Add kill feed entry

---

### MainMenu

Manages main menu.

```csharp
public class MainMenu : MonoBehaviour
```

**Methods:**
- `void OnStartGame()` - Start game
- `void OnOptions()` - Open options
- `void OnCredits()` - Open credits
- `void OnQuit()` - Quit game
- `void OnBack()` - Return to main menu
- `void LoadLevel(string levelName)` - Load level

---

### PauseMenu

Manages pause menu.

```csharp
public class PauseMenu : MonoBehaviour
```

**Methods:**
- `void Pause()` - Pause game
- `void Resume()` - Resume game
- `void ShowOptions()` - Show options
- `void HideOptions()` - Hide options
- `void LoadMainMenu()` - Return to main menu
- `void QuitGame()` - Quit game

---

### SettingsMenu

Manages game settings.

```csharp
public class SettingsMenu : MonoBehaviour
```

**Methods:**
- `void ApplySettings()` - Apply all settings
- `void Back()` - Return to previous menu
- `void ResetToDefault()` - Reset to defaults

---

## Utilities

### Singleton<T>

Generic singleton pattern for MonoBehaviours.

```csharp
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
```

**Properties:**
- `static T Instance` - Singleton instance

**Usage:**
```csharp
public class MyManager : Singleton<MyManager>
{
    // Your manager code
}

// Access from anywhere
MyManager.Instance.SomeMethod();
```

---

## Enumerations

### WeaponType
```csharp
enum WeaponType { Pistol, AssaultRifle, Shotgun, SniperRifle }
```

### FireMode
```csharp
enum FireMode { SemiAuto, Burst, FullAuto }
```

### DamageType
```csharp
enum DamageType { Bullet, Explosive, Fall, Melee }
```

### AIState
```csharp
enum AIState { Idle, Patrol, Chase, Attack, TakeCover, Dead }
```

### GameState
```csharp
enum GameState { MainMenu, Playing, Paused, GameOver, Victory }
```

### EnemyType
```csharp
enum EnemyType { Basic, Heavy, Fast, Sniper }
```

### AudioType
```csharp
enum AudioType { WeaponFire, Reload, Footstep, Impact, EnemyAlert, EnemyDeath, BackgroundMusic, UI }
```

### SurfaceType
```csharp
enum SurfaceType { Concrete, Metal, Wood, Grass, Water }
```

---

## Common Patterns

### Subscribing to Events

```csharp
void Start()
{
    PlayerHealth playerHealth = GetComponent<PlayerHealth>();
    playerHealth.OnHealthChanged += HandleHealthChanged;
}

void OnDestroy()
{
    // Always unsubscribe!
    playerHealth.OnHealthChanged -= HandleHealthChanged;
}

void HandleHealthChanged(float current, float max)
{
    // Handle health change
}
```

### Using Object Pooling

```csharp
// Spawn
GameObject bullet = ObjectPooler.Instance.SpawnFromPool("Bullet", position, rotation);

// Return after delay
Destroy(bullet, 5f); // Or manually return:
ObjectPooler.Instance.ReturnToPool(bullet);
```

### Creating Custom Weapon

```csharp
public class MyWeapon : WeaponBase
{
    protected override void PerformShot()
    {
        // Your custom firing logic
        base.PerformShot(); // Call base for standard behavior
    }
}
```

### Creating Custom AI State

```csharp
public class FleeState : AIStateBase
{
    public FleeState(EnemyAI ai) : base(ai) { }
    
    public override void OnEnter() { /* Setup */ }
    public override void OnUpdate() { /* Update logic */ }
    public override void OnExit() { /* Cleanup */ }
    public override AIState GetStateType() { return AIState.Idle; }
}
```

---

For more detailed information, see the full implementation guide and source code documentation.
