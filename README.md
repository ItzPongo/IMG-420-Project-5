# Assignment 5 - Rendering and Physics

## Overview
This project implements **custom canvas shaders**, **advanced physics simulations**, and **raycasting detection** using **Godot 4.x** and **C# (.NET)**.  
It features a particle system with shader effects, a physics-based chain system with joints, and a laser security system with player detection.

---

## Basic Requirements
1. **Custom Canvas Item Shader with Particles (3 points)**
   - Custom shader applied to particle system with ShaderMaterial
   - Wave distortion effect using UV coordinates and TIME
   - Color gradient with three-color blending system
   - Time-based animation for pulsing and dynamic parameter updates

2. **Advanced Rigid Body Physics with Joints (3.5 points)**
   - Multiple RigidBody2D segments (minimum 5) connected in chain
   - PinJoint2D connections with configured softness and bias
   - Realistic physics behavior with gravity, mass, and damping
   - Force application system for player interaction

3. **Raycasting Laser with Player Detection (3.5 points)**
   - RayCast2D continuously detecting collisions
   - Line2D visualization of laser beam path
   - Player detection with hierarchy checking
   - Visual alarm system with color changes and flashing effects

---

## What was added / changed
- Implemented custom GLSL shader with wave distortion and tri-color gradient system
- Created procedural particle texture generation in C# using Image.CreateEmpty()
- Built dynamic physics chain with configurable segments and joint properties
- Implemented raycast-based laser security system with alarm triggers
- Added visual feedback systems including flashing effects and color transitions
- Organized all scripts and shaders into clean project structure
- Added comprehensive error handling and player verification logic

---

## Files & Structure
```
├── custom_particle.gdshader    # Custom shader with wave effects
├── ParticleController.cs       # Particle system manager
├── PhysicsChain.cs            # Chain physics implementation
├── LaserDetector.cs           # Raycast laser security system
├── Player.cs                  # Player movement controller
├── Main.tscn                  # Main scene with all systems
└── README.md                  # This file
```

---

## Gameplay Summary
- **Movement Controls:**  
  - WASD or Arrow keys for movement  
  - Spacebar to apply random force to physics chain
  
- **Systems:**  
  - Particle system with animated shader effects in top-left area
  - Physics chain hanging from anchor point (swings with spacebar)
  - Laser security beam that detects player crossing
  
- **Visual Feedback:**  
  - Particles display wave distortion and color gradients
  - Chain segments react realistically to forces
  - Laser turns red and flashes when player detected

---

## How Systems Work

### Part 1: Custom Shader
The shader (`custom_particle.gdshader`) creates dynamic particle effects through:

#### Wave Distortion
```glsl
float wave = sin(uv.y * wave_frequency + TIME * time_scale) * wave_intensity;
uv.x += wave;
uv.y += cos(uv.x * wave_frequency * 0.5 + TIME * time_scale * 0.7) * wave_intensity * 0.5;
```
- Primary horizontal wave using sine function
- Secondary vertical wave for complex organic motion
- TIME variable ensures continuous animation
- Adjustable frequency and intensity for customization

#### Color Gradient System
```glsl
if (uv.y < 0.5) {
    final_color = mix(color_start, color_mid, uv.y * 2.0);
} else {
    final_color = mix(color_mid, color_end, (uv.y - 0.5) * 2.0);
}
```
- Three-color gradient (start → mid → end)
- Blends based on vertical UV position
- Creates fire-like or aurora effects
- Default: Orange → Yellow → Pink

#### Time-Based Animation
```glsl
float pulse = sin(TIME * 2.0) * 0.2 + 0.8;
final_color.a *= pulse;
```
- Pulsing opacity effect
- Breathing animation at 2 Hz frequency
- Alpha oscillates between 0.6 and 1.0

#### Dynamic Parameters (C#)
```csharp
float waveIntensity = 0.1f + Mathf.Sin(_timeElapsed * 0.5f) * 0.05f;
_shaderMaterial.SetShaderParameter("wave_intensity", waveIntensity);
```
- Wave intensity oscillates (0.05 to 0.15)
- Time scale varies for speed variations
- Real-time parameter updates during gameplay

---

### Part 2: Physics Chain System

#### Configuration
```csharp
[Export] public int ChainSegments = 5;
[Export] public float SegmentDistance = 30f;
```

#### Physics Properties
- **Mass:** 1.0 kg per segment
- **Linear Damping:** 0.5 (reduces velocity over time)
- **Angular Damping:** 0.5 (reduces rotation speed)
- **Gravity Scale:** 1.0 (normal gravity)

#### Joint Configuration (PinJoint2D)
```csharp
joint.Softness = 0.1f;  // Slightly flexible joints
joint.Bias = 0.3f;      // Error correction speed
```

**Why these values?**
- **Softness (0.1):** Creates rope-like flexibility rather than rigid metal chain
  - Lower = more rigid, Higher = more elastic
- **Bias (0.3):** Balances stability with realistic motion
  - Too high = jittery behavior, Too low = stretchy/unrealistic
- **Damping (0.5):** Prevents endless oscillation while maintaining momentum
- **First Segment Static:** Acts as anchor point (ceiling attachment)

#### Force Application
```csharp
public void ApplyForceToEnd(Vector2 force)
{
    if (_segments.Count > 0)
    {
        _segments[_segments.Count - 1].ApplyImpulse(force);
    }
}
```
- Spacebar applies random impulse to end segment
- Demonstrates realistic physics propagation through chain
- Force travels from segment to segment via joints

---

### Part 3: Raycast Detection System

#### RayCast2D Setup
```csharp
_rayCast.Enabled = true;
_rayCast.TargetPosition = new Vector2(LaserLength, 0);
_rayCast.CollisionMask = 1;
```
- Casts horizontal ray from origin
- Length defined by LaserLength export (default 500px)
- Detects objects on collision layer 1

#### Detection Process

**1. Ray Update (every physics frame):**
```csharp
_rayCast.ForceRaycastUpdate();
bool isColliding = _rayCast.IsColliding();
```

**2. Collision Handling:**
```csharp
Vector2 endPoint = _rayCast.GetCollisionPoint();
var collider = _rayCast.GetCollider();
```

**3. Player Verification:**
```csharp
private bool IsPlayerHit(GodotObject collider)
{
    if (collider == _player) return true;
    // Traverse parent hierarchy for nested nodes
}
```
- Checks if collider is player or player's child
- Handles complex player node structures
- Prevents false positives from other objects

**4. Visual Feedback:**
- **Line2D:** Draws from origin to collision point (or full length if no hit)
- **Color Transition:** Green (normal) → Red (alarm)
- **Hit Indicator:** Yellow circle at exact collision point
- **Flash Effect:** Pulsing red overlay using sine wave
  ```csharp
  float alpha = (Mathf.Sin(_flashTime * 10.0f) + 1.0f) * 0.3f;
  _alarmFlash.Color = new Color(1, 0, 0, alpha);
  ```

#### Alarm System
- **Timer:** 0.5s periodic triggers while alarm active
- **State Management:** Prevents spam, clean transitions
- **Debug Output:** Console messages for testing ("ALARM! Player detected!")
- **Reset:** Automatically returns to normal when player leaves beam

---

## Scene Structure
```
Main (Node2D)
├── ParticleSystem (Node2D)
│   └── GpuParticles2D [ParticleController.cs]
│
├── PhysicsDemo (Node2D) [PhysicsChain.cs]
│   ├── StaticBody2D (anchor)
│   ├── RigidBody2D (segment 1)
│   ├── RigidBody2D (segment 2)
│   ├── RigidBody2D (segment 3)
│   ├── RigidBody2D (segment 4)
│   └── PinJoint2D (×4 joints)
│
├── LaserSystem (Node2D) [LaserDetector.cs]
│   ├── RayCast2D
│   ├── Line2D (laser beam visualization)
│   ├── Timer (alarm timer)
│   └── ColorRect (flash effect)
│
├── Player (CharacterBody2D) [Player.cs]
│   ├── CollisionShape2D
│   └── ColorRect (visual)
│
└── Camera2D (optional)
```

---

## Troubleshooting / Common Fixes

### Shader Issues
- **Particles don't show wave effect:** Verify shader path is `res://custom_particle.gdshader`
- **No color gradient:** Check Material property is set on GpuParticles2D
- **Build errors with literals:** Add `f` suffix to float values (e.g., `0.1f`)

### Physics Chain Issues
- **Chain doesn't appear:** Verify PhysicsChain script attached to PhysicsDemo node
- **No movement when pressing Space:** Check Input section has `ui_*` actions or script uses Key.Space
- **Segments fall through:** Ensure collision layers match between segments
- **Type conversion errors:** Changed `previousBody` to `Node2D` type to handle both StaticBody2D and RigidBody2D

### Laser Detection Issues
- **Laser doesn't detect player:** 
  - Set Player collision layer to 1
  - Set LaserDetector collision mask to 1
  - Assign PlayerPath in Inspector
- **Laser doesn't show:** Verify Line2D is child of LaserSystem
- **No alarm trigger:** Check console output for "ALARM! Player detected!" messages

### Troubleshooting / common fixes

Missing assemblies / compile errors: Open the .csproj and set Sdk="Godot.NET.Sdk/<your-version>" to match your Godot Mono binary.

---

