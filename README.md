# Assignment 5 - Rendering and Physics

## Overview
This project demonstrates **custom shaders**, **physics simulations**, and **raycasting** using **Godot 4.x** and **C#**.  
Features include animated particles with wave effects, a swinging chain with realistic physics, and a laser that detects the player.

---

## What was added / changed
- Custom GLSL shader with wave effects and color gradients
- Particle system with animated effects
- Physics chain that swings realistically
- Laser security system with player detection
- Visual feedback (color changes, flashing)
- Player movement controller
- Custom chain segment scene for reusable physics objects

---

## Files & Structure
```
├── custom_particle.gdshader    # Shader file
├── ParticleController.cs       # Manages particles
├── PhysicsChain.cs            # Chain physics
├── LaserDetector.cs           # Laser system
├── Player.cs                  # Player movement
├── Main.tscn                  # Main scene
├── ChainSegment.tscn          # Reusable chain segment
└── README.md                  # Documentation
```

---

## Controls
- **WASD / Arrow Keys** - Move player
- **Spacebar** - Apply force to chain

---

## How Systems Work

### Part 1: Shader
The shader creates animated particles with:

**Wave Effect:**
```glsl
uv.x += sin(uv.y * 10.0 + TIME) * wave_intensity;
```
- Distorts particle texture over time
- Creates flowing motion

**Color Gradient:**
```glsl
final_color = mix(color_start, color_mid, uv.y * 2.0);
```
- Blends between 3 colors (orange → yellow → pink)
- Based on particle position

**Pulsing Animation:**
```glsl
float pulse = sin(TIME * 2.0) * 0.2 + 0.8;
```
- Particles fade in and out
- Creates breathing effect

---

### Part 2: Physics Chain

**Properties:**
- **Mass:** 1.0 per segment
- **Damping:** 0.5 (slows down motion)
- **Gravity:** 1.0 (normal)

**Why these values?**
- **Softness (0.1):** Makes chain flexible like rope
- **Bias (0.3):** Keeps chain stable but natural
- **Damping (0.5):** Stops endless swinging

**How it works:**
- First segment is static (anchor point)
- Each segment connects with PinJoint2D
- Spacebar applies random force to end
- Force travels through chain realistically
- Uses ChainSegment.tscn for consistent segment design (optional)

---

### Part 3: Laser System

**Setup:**
```csharp
_rayCast.TargetPosition = new Vector2(500, 0);
_rayCast.CollisionMask = 1;
```
- Shoots ray horizontally
- Detects objects on layer 1

**How it detects:**
1. Updates every frame
2. Checks for collisions
3. Verifies if player was hit
4. Changes color and flashes

**Visual feedback:**
- Green laser = normal
- Red laser + flashing = alarm
- Yellow circle = hit point

---

## Scene Structure
```
Main (Node2D)
├── ParticleSystem (Node2D)
│   └── GpuParticles2D [ParticleController.cs]
│
├── PhysicsDemo (Node2D) [PhysicsChain.cs]
│   ├── StaticBody2D (anchor)
│   └── ChainSegment instances (×5)
│
├── LaserSystem (Node2D) [LaserDetector.cs]
│   ├── RayCast2D
│   └── Line2D (beam)
│
└── Player (CharacterBody2D) [Player.cs]
    ├── CollisionShape2D
    └── ColorRect

ChainSegment.tscn (separate scene)
└── ChainSegment (RigidBody2D)
    ├── CollisionShape2D (RectangleShape2D 15×25)
    └── ColorRect (visual, red color)
```

---

## Troubleshooting

- **Compile errors:** Click Build button (hammer icon)
- **Scripts not working:** Make sure all scripts are saved
- **Image.Create error:** Use `Image.CreateEmpty()` instead

---
