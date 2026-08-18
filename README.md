# Shoot-em-Up — "Navinha"

A 2D top-down shoot 'em up built in Unity, with three enemy archetypes, a drop-based power-up system and a three-phase final boss.

First project of the **Oficina Indie** course (André Young), built from the course material and open-ended direction rather than a step-by-step tutorial.

---

## Gameplay

The run is structured in three stages:

| Stage | Goal |
|---|---|
| 1 | Reach 400 points by destroying enemies |
| 2 | Survive 60 seconds |
| 3 | Defeat the final boss |

The mechanics are deliberately compact — the difficulty curve is what carries the experience.

### Enemies

Three archetypes, each demanding a different response from the player:

- **Melee** — no weapon, attacks by collision. Moves in a zig-zag pattern that makes it hard to hit, and deals heavy damage on contact.
- **Shooter** — the tankiest of the three. Fires a double laser, forcing the player to shoot and reposition quickly.
- **Multishooter** — four guns at 90° firing simultaneously, on a hull that rotates 360°. Ignoring it usually costs a life.

### Power-ups

Enemies have a chance to drop power-ups and health on death: **multishot**, **minigun**, **bomb**, **shield** and **move speed**.

### Final boss

Three phases, each activating a different weapon set:

1. **Simple shot** — two guns firing alternately
2. **Minigun** — two fast-firing guns with a randomized pattern
3. **Laser** — four guns driving four vertical beams

---

## Stack

| | |
|---|---|
| Engine | Unity 6000.0.48f1 |
| Language | C# |
| Rendering | Universal Render Pipeline (URP) |
| Input | Unity Input System |
| UI | TextMesh Pro |

---

## Architecture

`EntityStats` is the core of the design: a single shared component holding HP, speed, attack damage, attack speed, point value and shield state. Player, enemies and boss all reuse it, so damage, death and hit feedback (red-flash coroutine) are implemented once instead of duplicated per entity type.

```
Assets/Script/
├── EntityStats.cs        # Shared stats/damage component (player, enemies, boss)
├── SpawnManager.cs       # Enemy spawn logic and pacing
├── SimpleEnemy.cs        # Behaviour for the three enemy archetypes
├── FinalBossScript.cs    # Boss phase state machine
├── PowerUp.cs            # Drops and their effects
├── InventoryManager.cs   # Active power-up tracking
├── OptionsManager.cs     # Settings
├── PlayerScript.cs       # Player movement and firing
└── Misc/
    ├── LazerGun.cs           ├── Projectile_Player.cs
    ├── MiniGun_boss.cs       ├── Projectile_boss.cs
    ├── Projectile_Enemy.cs   ├── StartButton.cs
    └── explosion.cs
```

Scenes: `StartScene.unity` (menu) → `SampleScene.unity` (gameplay).

---

## Running locally

```bash
git clone https://github.com/EnzoGiacomini/Shoot-em-Up.git
```

Open the folder through **Unity Hub** with editor version `6000.0.48f1`, then load `Assets/Scenes/StartScene.unity` and press Play.

---

## Credits

Course direction by **André Young** (Oficina Indie). Visual effects from the **JMO Assets** package.
