# Unity FPS Game Framework

A complete, production-ready First-Person Shooter game framework built with C# for Unity, featuring professional architecture and comprehensive gameplay systems.

## 🎮 Features

- **Complete Player System** - WASD movement, sprint, jump, crouch, head bob
- **Advanced Weapon System** - 4 weapon types with realistic mechanics (Pistol, Assault Rifle, Shotgun, Sniper)
- **Intelligent AI** - State machine-based enemies with patrol, chase, attack, and cover behaviors
- **Full UI Suite** - HUD, main menu, pause menu, settings with volume controls
- **Game Management** - Score tracking, wave system, game state management
- **Audio System** - Spatial 3D audio with volume controls and surface-based footsteps
- **Health System** - Damage types, armor, regeneration, and death handling
- **Object Pooling** - Performance-optimized object reuse system

## 📋 Requirements

- Unity 2021.3 LTS or newer
- TextMeshPro package
- Basic understanding of Unity and C#

## 🚀 Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/ssfdre38/ai-fps-test-gh.git
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Click "Add" and select the project folder
   - Open the project

3. **Read the Implementation Guide**
   - See [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) for detailed setup instructions

4. **Set up your first scene**
   - Follow the Scene Setup section in the implementation guide
   - Configure player, enemies, and UI
   - Bake NavMesh for AI

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Player/         # Player controller, health, audio, camera
│   ├── Weapons/        # Weapon system and implementations
│   ├── AI/             # Enemy AI with state machine
│   ├── Systems/        # Game manager, audio, spawner, pooling
│   ├── UI/             # HUD, menus, settings
│   └── Utilities/      # Singleton, enums, helpers
├── Scenes/             # Game scenes
├── Prefabs/            # Reusable game objects
└── ScriptableObjects/  # Configuration data
```

## 🎯 Key Components

### Player System
- **FPSController** - Movement with sprint, jump, crouch
- **PlayerHealth** - Health management with armor and regeneration
- **CameraController** - Mouse look with weapon sway
- **PlayerAudio** - Surface-based footstep sounds

### Weapon System
- **WeaponBase** - Abstract base class for all weapons
- **WeaponManager** - Weapon switching and management
- **WeaponData** - ScriptableObject for weapon configuration
- Individual weapon implementations (Pistol, Assault Rifle, Shotgun, Sniper)

### AI System
- **EnemyAI** - State machine controller with pathfinding
- **EnemyHealth** - Health and ragdoll death
- State implementations: Patrol, Chase, Attack, Cover

### Game Systems
- **GameManager** - Singleton managing game state
- **WaveSpawner** - Enemy wave spawning with difficulty scaling
- **AudioManager** - Centralized audio management
- **ObjectPooler** - Performance optimization

## 🛠️ Usage Examples

### Creating a New Weapon
```csharp
public class MachineGun : WeaponBase
{
    protected override void PerformShot()
    {
        // Custom firing logic
        base.PerformShot();
    }
}
```

### Spawning Enemies
```csharp
WaveSpawner spawner = FindObjectOfType<WaveSpawner>();
spawner.TriggerNextWave();
```

### Managing Game State
```csharp
GameManager.Instance.PauseGame();
GameManager.Instance.AddScore(100);
```

## 📖 Documentation

- [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) - Complete setup and usage guide
- All scripts include XML documentation
- Inspector tooltips for easy configuration

## 🎨 Design Patterns

- **Singleton** - For global managers
- **State Machine** - For AI behavior
- **Object Pool** - For performance
- **Observer** - For event-driven communication
- **Strategy** - For weapon variations

## ⚡ Performance Features

- Object pooling for bullets and effects
- Efficient component caching
- Distance-based AI updates
- Optimized raycasting

## 🔧 Configuration

All major systems are configurable through:
- ScriptableObjects (weapon data)
- Inspector properties (enemy AI, spawners)
- Settings menu (audio, graphics, controls)

## 📝 Code Quality

- XML documentation on all public methods
- Consistent naming conventions
- Proper encapsulation
- Event-driven architecture
- Clean separation of concerns

## 🐛 Troubleshooting

See the [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) for common issues and solutions.

## 📄 License

This project is open source and available for educational and commercial use.

## 🤝 Contributing

Contributions are welcome! Please:
1. Follow the existing code style
2. Add XML documentation
3. Test thoroughly
4. Update documentation

## 📞 Support

For detailed instructions, troubleshooting, and examples, see [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md).

---

**Built with Unity and C# - A complete, modular FPS framework ready for your game!** 🎮
