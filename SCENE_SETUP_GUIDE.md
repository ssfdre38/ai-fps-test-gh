# Unity Scene Setup Instructions

This guide will help you set up a complete FPS game scene in Unity using the provided framework.

## Scene Hierarchy Example

```
Level01
├── Lighting
├── GameManager
├── AudioManager
├── ObjectPooler
├── WaveSpawner
├── Environment
│   ├── Ground
│   ├── Walls
│   └── Cover Objects
├── Player
│   ├── Camera
│   └── WeaponHolder
│       ├── Pistol
│       ├── AssaultRifle
│       ├── Shotgun
│       └── SniperRifle
├── SpawnPoints
│   ├── SpawnPoint1
│   ├── SpawnPoint2
│   └── SpawnPoint3
├── PatrolPoints
│   ├── PatrolPoint1
│   ├── PatrolPoint2
│   └── PatrolPoint3
└── Canvas
    ├── HUD
    ├── PauseMenu
    └── SettingsMenu
```

## Step-by-Step Setup

### 1. Create Base Scene

1. Create new scene: File > New Scene
2. Save as "Level01" in Assets/Scenes/
3. Add Directional Light (if not present)
4. Configure lighting settings

### 2. Environment Setup

#### Ground
```
1. Create Plane (3D Object > Plane)
2. Scale to desired size (e.g., 10, 1, 10)
3. Add Box Collider
4. Tag as "Ground"
5. Layer: Ground
6. Add material/texture
```

#### Walls and Cover
```
1. Create Cubes for walls
2. Add Box Colliders
3. Tag as appropriate (Wall, Cover)
4. Layer: Default or Cover
```

### 3. Game Managers Setup

#### GameManager
```
1. Create Empty GameObject: "GameManager"
2. Add Component: Scripts > Systems > GameManager
3. Configure in Inspector:
   - Current Game State: Playing
   - Player Spawn Point: (create and assign)
   - Player Prefab: (assign your player prefab)
```

#### AudioManager
```
1. Create Empty GameObject: "AudioManager"
2. Add Component: Scripts > Systems > AudioManager
3. Add sounds in Inspector:
   - Name: "WeaponFire"
   - Clip: (assign audio clip)
   - Audio Type: SFX
   - Volume: 0.7
   - Is3D: false
4. Repeat for all sounds needed
```

#### ObjectPooler
```
1. Create Empty GameObject: "ObjectPooler"
2. Add Component: Scripts > Systems > ObjectPooler
3. Configure pools:
   - Tag: "Bullet"
   - Prefab: (bullet prefab)
   - Size: 50
4. Add more pools as needed
```

### 4. Player Setup

#### Create Player GameObject
```
1. Create Empty GameObject: "Player"
2. Tag: Player
3. Layer: Player
4. Position: (0, 1, 0) or spawn point
```

#### Add CharacterController
```
1. Add Component: Character Controller
2. Configure:
   - Height: 2
   - Radius: 0.5
   - Center: (0, 1, 0)
```

#### Add Player Scripts
```
1. Add Component: Scripts > Player > FPSController
   - Walk Speed: 5
   - Sprint Speed: 8
   - Crouch Speed: 2.5
   - Jump Force: 8
   - Gravity: 20

2. Add Component: Scripts > Player > PlayerHealth
   - Max Health: 100
   - Max Armor: 100
   - Enable Health Regeneration: true
   - Regen Delay: 5
   - Regen Rate: 5

3. Add Component: Scripts > Player > PlayerAudio
4. Add Component: Audio Source (for footsteps)
```

#### Create Camera Child
```
1. Create Child GameObject: "Camera"
2. Tag: MainCamera
3. Add Component: Camera
4. Add Component: Audio Listener
5. Add Component: Scripts > Player > CameraController
   - Mouse Sensitivity: 2
   - Max Look Angle: 80
6. Position: (0, 1.6, 0)
```

#### Create WeaponHolder Child
```
1. Create Child GameObject under Player: "WeaponHolder"
2. Position: (0.5, 1.4, 0.5) - adjust as needed
3. Add Component: Scripts > Weapons > WeaponManager
```

#### Add Weapons to WeaponHolder
```
For each weapon:
1. Create Child GameObject: "Pistol"
2. Add Component: Scripts > Weapons > Pistol
3. Configure:
   - Weapon Data: (assign ScriptableObject)
   - Fire Point: (create and assign child transform)
   - Player Camera: (assign main camera)
4. Add weapon model as child
5. Repeat for AssaultRifle, Shotgun, SniperRifle
```

### 5. Enemy Setup

#### Create Enemy Prefab
```
1. Create Empty GameObject: "Enemy"
2. Tag: Enemy
3. Layer: Enemy
4. Add Component: Nav Mesh Agent
   - Speed: 3.5
   - Angular Speed: 120
   - Acceleration: 8
   - Stopping Distance: 2
   - Auto Braking: true

5. Add Component: Scripts > AI > EnemyAI
   - Enemy Type: Basic
   - Detection Range: 20
   - Attack Range: 10
   - Field Of View: 90
   - Damage: 10
   - Fire Rate: 1
   - Target Mask: (select Player layer)
   - Obstacle Mask: (select Default layer)
   - Patrol Points: (assign transforms)
   - Cover Health Threshold: 30
   - Cover Search Radius: 15
   - Cover Mask: (select Cover layer)

6. Add Component: Scripts > AI > EnemyHealth
   - Max Health: 100
   - Enable Ragdoll: true
   - Despawn Delay: 10

7. Add enemy model with Animator
8. Add Rigidbodies to limbs for ragdoll
9. Save as Prefab in Assets/Prefabs/Enemies/
```

### 6. Spawn Points Setup

```
1. Create Empty GameObject: "SpawnPoints"
2. Create child Empty GameObjects: "SpawnPoint1", "SpawnPoint2", etc.
3. Position them around the level
4. Use arrow gizmo to show forward direction
```

### 7. Patrol Points Setup

```
1. Create Empty GameObject: "PatrolPoints"
2. Create child Empty GameObjects: "PatrolPoint1", "PatrolPoint2", etc.
3. Position them in a patrol route
4. Assign to Enemy AI patrol points array
```

### 8. Wave Spawner Setup

```
1. Create Empty GameObject: "WaveSpawner"
2. Add Component: Scripts > Systems > WaveSpawner
3. Configure:
   - Starting Wave: 1
   - Time Between Waves: 10
   - Base Enemies Per Wave: 5
   - Difficulty Scaling: 1.2
   - Enemy Prefabs: (assign enemy prefabs)
   - Spawn Points: (assign spawn point transforms)
   - Spawn Delay: 0.5
   - Max Enemies Alive: 20
   - Auto Start Waves: true
```

### 9. UI Setup

#### Create Canvas
```
1. Create UI > Canvas
2. Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080
   - Match: 0.5
```

#### Add Event System
```
- Should be created automatically
- If not: Create UI > Event System
```

#### Create HUD
```
1. Create Child Empty GameObject: "HUD"
2. Add Component: Scripts > UI > HUDManager
3. Create UI elements:
   - Health Bar (Image with fill)
   - Armor Bar (Image with fill)
   - Ammo Text (TextMeshPro)
   - Crosshair (Image at center)
   - Score Text (TextMeshPro)
   - Wave Text (TextMeshPro)
4. Assign references in HUDManager
```

#### Create Pause Menu
```
1. Create Child GameObject: "PauseMenu"
2. Add Component: Scripts > UI > PauseMenu
3. Create panel with buttons:
   - Resume
   - Options
   - Main Menu
   - Quit
4. Assign references in PauseMenu
5. Set inactive by default
```

#### Create Settings Menu
```
1. Create Child GameObject: "SettingsMenu"
2. Add Component: Scripts > UI > SettingsMenu
3. Create settings UI:
   - Volume sliders (Master, Music, SFX)
   - Graphics dropdowns (Quality, Resolution)
   - Control sliders (Sensitivity)
4. Assign references in SettingsMenu
5. Set inactive by default
```

### 10. NavMesh Baking

```
1. Select all ground and walkable surfaces
2. Window > AI > Navigation
3. Object tab:
   - Check "Navigation Static"
   - Navigation Area: Walkable
4. Bake tab:
   - Agent Radius: 0.5
   - Agent Height: 2
   - Max Slope: 45
   - Step Height: 0.4
5. Click "Bake"
```

### 11. Lighting Setup

```
1. Window > Rendering > Lighting
2. Environment tab:
   - Skybox Material: (choose or create)
   - Sun Source: (assign Directional Light)
3. Mixed Lighting:
   - Baked Global Illumination: (optional)
4. Generate Lighting (if using baked)
```

### 12. Layer Collision Matrix

```
1. Edit > Project Settings > Physics
2. Configure Layer Collision Matrix:
   - Player vs Enemy: Checked
   - Player vs Ground: Checked
   - Enemy vs Ground: Checked
   - Player vs Player: Unchecked
   - Enemy vs Enemy: Unchecked
```

### 13. Input Configuration

```
1. Edit > Project Settings > Input Manager
2. Verify axes exist:
   - Horizontal (A/D, Left/Right)
   - Vertical (W/S, Up/Down)
   - Fire1 (Left Mouse)
   - Fire2 (Right Mouse)
   - Jump (Space)
   - Mouse X
   - Mouse Y
```

### 14. Create Weapon Data ScriptableObjects

```
1. Right-click in Project > Create > FPS Game > Weapon Data
2. Name: "PistolData"
3. Configure:
   - Weapon Name: "Pistol"
   - Weapon Type: Pistol
   - Damage: 25
   - Fire Mode: SemiAuto
   - Magazine Size: 12
   - Max Reserve Ammo: 60
   - Reload Time: 1.5
4. Repeat for other weapons
5. Assign to weapon components
```

### 15. Build Settings

```
1. File > Build Settings
2. Add scenes:
   - MainMenu (index 0)
   - Level01 (index 1)
3. Configure platform settings
```

## Testing Checklist

- [ ] Player can move (WASD)
- [ ] Player can sprint (Shift)
- [ ] Player can jump (Space)
- [ ] Player can crouch (Ctrl)
- [ ] Camera looks around with mouse
- [ ] Weapons can switch (1-4 keys)
- [ ] Weapons can fire (Left Mouse)
- [ ] Weapons can reload (R)
- [ ] Enemies spawn in waves
- [ ] Enemies patrol between points
- [ ] Enemies chase player when detected
- [ ] Enemies attack when in range
- [ ] Enemies take cover when low health
- [ ] UI displays health, ammo, score
- [ ] Pause menu works (Esc)
- [ ] Settings save and load
- [ ] Audio plays correctly
- [ ] NavMesh is properly baked

## Tips

1. **Test frequently** - Test each system as you add it
2. **Use prefabs** - Create prefabs for reusable objects
3. **Save often** - Save your scene regularly
4. **Version control** - Use Git to track changes
5. **Optimize** - Profile performance regularly
6. **Documentation** - Keep notes on your setup

## Common Issues

### Enemies not moving
- Check NavMesh is baked
- Verify NavMeshAgent settings
- Ensure patrol points are assigned

### Player falls through floor
- Check CharacterController is present
- Verify colliders on ground
- Check layer collision matrix

### Weapons not firing
- Verify weapon data is assigned
- Check fire point transform exists
- Ensure camera reference is set

### UI not visible
- Check Canvas render mode
- Verify Canvas Scaler settings
- Ensure EventSystem exists

---

**Scene setup complete! Your FPS game is ready to play!** 🎮
