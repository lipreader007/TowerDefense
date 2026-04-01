# Tower Defense — CurlyBlue Lead Developer Audition

## How to Run

1. Open the project in **Unity 6000.0.23f1** or later (LTS preferred)
2. Open the scene: `Assets/Scenes/TowerDefense.unity`
3. Press **Play**

No build required. No additional packages beyond TextMeshPro (included in Unity).

---

## Project Structure

```
Assets/
└── Scripts/
    ├── Data/               # ScriptableObjects — pure config, zero logic
    │   ├── EnemyData.cs
    │   ├── TowerData.cs
    │   ├── GameConfig.cs
    │   └── WaveConfig.cs
    ├── Interfaces/         # Contracts — IDamageable, ITowerStrategy
    ├── Entities/           # MonoBehaviours — Enemy, Tower, Projectile, PlayerBase, Path
    ├── Towers/
    │   └── Strategies/     # AOETowerStrategy, MultiTargetTowerStrategy
    ├── Systems/            # GameManager, ScoreManager, WaveManager, InputManager
    ├── UI/                 # HUDView (pure view, no business logic)
    └── Utilities/          # EnemyHealthBar, RangeIndicator
```

---

## Technical Design Document

### Class Structure & Responsibility Breakdown

Responsibilities are divided into four layers:

**Data Layer (ScriptableObjects)**
- `EnemyData` — health, speed, damage, VFX prefab references, score value
- `TowerData` — range, fire rate, damage, tower type enum, AOE/multi-target settings
- `GameConfig` — global tuning values (base HP, spawn interval, wave cooldown)
- `WaveConfig` — per-wave spawn definitions (enemy type + count pairs)

No logic lives here. These are pure value containers a designer edits in the Inspector.

**Entity Layer (MonoBehaviours)**
- `Enemy` — movement along Path, current HP, damage reception, hit/death VFX, events
- `Tower` — targeting loop, fire rate timer, delegates attack to `ITowerStrategy`
- `Projectile` — homing movement, delegates damage application to a callback on hit
- `PlayerBase` — listens for enemies reaching it, tracks base HP, fires game-over event
- `Path` — owns the waypoint list; enemies query it for their next destination

**System Layer**
- `GameManager` — state machine (Playing → GameOver); singleton
- `ScoreManager` — subscribes to `Enemy.OnEnemyDied`, increments score
- `WaveManager` — coroutine-based spawner reading `WaveConfig` assets
- `InputManager` — raycast on mouse click → `IDamageable.TakeDamage`

**UI Layer**
- `HUDView` — subscribes to system events, updates TMP text and sliders; zero business logic

---

### Data vs Logic Separation

All tunable values live in ScriptableObjects. Gameplay code only reads from them:

```csharp
// All tuning lives in the SO, not in code:
_currentHealth = _data.MaxHealth;
transform.position = Vector3.MoveTowards(transform.position, target, _data.MoveSpeed * Time.deltaTime);
```

A designer duplicates `EnemyData_Basic.asset` → `EnemyData_Heavy.asset`, changes values,
and drags it onto a prefab. No programmer involvement.

---

### Strategy Pattern — Tower Attack Behaviour

`ITowerStrategy` decouples shot logic from the `Tower` MonoBehaviour:

```csharp
public interface ITowerStrategy
{
    void Execute(List<Enemy> enemiesInRange, Tower owner);
}
```

`Tower.cs` calls `_strategy.Execute(...)` — it never knows whether it's AOE or multi-target.
Adding a new tower type = new `.cs` file + new `TowerType` enum value. Nothing else changes.

**AOE Strategy**
Selects the highest-priority enemy (closest to base by `PathProgress`).
Fires one projectile; on impact, `Physics.OverlapSphere` damages all enemies in splash radius
with linear damage falloff from centre to edge.

**Multi-Target Strategy**
Selects top N enemies (N = `TowerData.MaxTargets`).
Fires one direct-damage projectile per target simultaneously.

---

### Event Architecture

Systems communicate through `static event Action<T>` delegates — not inspector wiring:

| Event | Raised by | Consumed by |
|---|---|---|
| `Enemy.OnEnemyDied` | `Enemy` | `ScoreManager` |
| `Enemy.OnEnemyReachedBase` | `Enemy` | `PlayerBase` |
| `PlayerBase.OnBaseDestroyed` | `PlayerBase` | `GameManager` |
| `PlayerBase.OnHealthChanged` | `PlayerBase` | `HUDView` |
| `ScoreManager.OnScoreChanged` | `ScoreManager` | `HUDView` |
| `GameManager.OnGameStateChanged` | `GameManager` | `HUDView` |

No entity holds a reference to a manager. The dependency graph flows one way.

---

### Assembly Definition Boundaries

Enforces dependency direction at compile time — a UI class cannot accidentally reference
a Systems class that references UI (circular dependency caught at compile time, not review time).

```
TowerDefense.Data           (no dependencies)
TowerDefense.Interfaces     (no dependencies)
TowerDefense.Towers         → Data, Interfaces
TowerDefense.Entities       → Data, Interfaces, Towers
TowerDefense.Systems        → Data, Interfaces, Entities
TowerDefense.UI             → Systems, Entities, Data
TowerDefense.Utilities      → Data, Entities
```

---

### Trade-offs Made Under Time Pressure

| Cut | Reason | What I'd add with more time |
|---|---|---|
| Object pooling | Instantiate/Destroy fine for demo scale | `UnityEngine.Pool.IObjectPool<T>` for enemies + projectiles |
| NavMesh pathfinding | Waypoint array is sufficient to show mechanics | NavMesh or A* for dynamic obstacle avoidance |
| Wave difficulty scaling | Static wave list, no scaling | Per-wave spawn interval overrides, difficulty curve |
| Audio | Time constraint; VFX hooks are in place | AudioManager with pooled sources, SO-driven clip configs |
| Unit tests | Architecture is test-friendly (no static logic) | NUnit tests for damage calc, strategy targeting, score |
| Tower placement | Pre-placed per spec | Grid-based ghost-preview placement system |
| Save / load | Not required for demo | Serialised save file via `JsonUtility` |
| Health bar reactivity | Polling approach on `EnemyHealthBar` | Reactive binding; expose `OnHealthChanged` event on `Enemy` |

---

### What I'd Do Differently With a Full Week

1. **Object pooling from day one.** Cheap to add early, painful to retrofit. Generic `Pool<T>` wired before spawn logic is written.
2. **Full WaveConfig ScriptableObjects.** Per-wave spawn curves, difficulty scaling, enemy mix — designer-owned with zero code changes.
3. **Event bus.** Replace direct `static event` subscriptions with a lightweight SO-based event channel (Ryan Hipple pattern) to eliminate cross-assembly coupling entirely.
4. **Additive scene architecture.** Bootstrap + Gameplay + UI in separate scenes; teams work in parallel without scene merge conflicts.
5. **CI pipeline.** GitHub Actions + Unity Test Runner on every PR. Asmdef boundaries already enforce the architecture; tests verify the logic.

---

## Shortcuts & Owned Trade-offs

- **No object pooling** — `Instantiate`/`Destroy` used for enemies and projectiles. Acceptable for demo enemy counts.
- **Waypoint pathfinding** — enemies follow a `Transform[]` array. Easy to swap for NavMesh later; the `Path` class is the only thing that changes.
- **Single scene** — all systems in one scene. `GameManager.RestartGame()` reloads the scene, which is simple and reliable for a demo.
- **No audio** — event hooks are in place; AudioManager slots in without touching existing code.
- **Health bar polling** — `EnemyHealthBar` polls rather than subscribing to an event. Fine for small counts; noted as a known rough edge.

---

## Scene Setup Checklist (for reviewer)

All of the following should be configured in `TowerDefense.unity`:

### GameObjects
- `[Bootstrap]` — holds `GameManager`, `ScoreManager`, `InputManager`
- `[WaveManager]` — holds `WaveManager` with wave list and enemy prefab map
- `Path` — holds `Path` component with waypoint Transforms as children
- `PlayerBase` — holds `PlayerBase` component, `GameConfig` assigned
- `Tower_AOE` — holds `Tower` with `TowerData_AOE` assigned
- `Tower_MultiTarget` — holds `Tower` with `TowerData_MultiTarget` assigned
- `Canvas` — World Space HUD with `HUDView` component

### ScriptableObject Assets (`Assets/Data/`)
- `GameConfig.asset`
- `EnemyData_Basic.asset` — low HP, low damage, higher score value
- `EnemyData_Heavy.asset` — high HP, high damage, lower speed
- `TowerData_AOE.asset` — type: AOE, splash settings filled
- `TowerData_MultiTarget.asset` — type: MultiTarget, MaxTargets = 3
- `WaveConfig_01.asset` — mix of basic and heavy enemies

### Layer Setup
- Create an `Enemy` layer in Project Settings → Tags & Layers
- Assign enemy prefab colliders to the `Enemy` layer
- Set `InputManager._enemyLayerMask` to the `Enemy` layer

---

*Total time spent: 2 hours 15 minutes*
