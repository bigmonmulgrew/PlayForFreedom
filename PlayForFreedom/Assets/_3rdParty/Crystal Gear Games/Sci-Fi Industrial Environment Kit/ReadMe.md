# Sci-Fi Industrial Environment Kit
## Documentation & User Guide

Thank you for purchasing the **Sci-Fi Industrial Environment Kit**.

This package contains a collection of modular sci-fi environment assets designed to help you quickly create futuristic industrial facilities, space stations, corridors, hangars, and mechanical environments.

The kit has been created with optimisation and flexibility in mind, using a shared material workflow and a custom shader system that allows you to easily customise the appearance of the entire environment.

---

# Getting Started

1. Import the package into your Unity project.
2. Open the **Showcase Scene** to view all included prefabs and example scripts.
3. Drag the modular prefabs into your own scene to begin building.
4. Customise the included material using the shader controls.
5. Add the **SpinObject** component to any object that requires rotation, or use the **Rainbow Material Controller** as an example of runtime material customisation.

---

# Package Contents

The package includes:

- 101 Modular Sci-Fi Environment Pieces
- Optimised Modular Meshes
- Shared Trim Sheet Material Workflow
- Custom Environment Shader
- Force Field Shader Graph
- Emissive Lighting Controls
- Panel Colour Customisation System
- Customisable Force Field Effects
- SpinObject Utility Script
- Rainbow Material Controller
- Fully Assembled Showcase Scene

All assets are designed to work together seamlessly while maintaining a clean and efficient material setup.

---

# Modular Asset Workflow

All environment pieces use a shared material system.

Instead of every object having its own texture set, the kit uses a single master material that controls the appearance of every asset.

## Benefits

- Reduced material count
- Improved batching performance
- Easy global style changes
- Simple colour customisation
- Consistent visual appearance across the entire kit

Simply place the modular prefabs into your scene and they will automatically use the included material setup.

---

# Custom Sci-Fi Material Shader

The included shader provides complete creative control over the appearance of the environment.

## Surface Inputs

### Metallic Intensity

Controls how metallic the material appears.

**Higher Values**

- More reflective
- Stronger metallic appearance

**Lower Values**

- Less reflective
- More painted or plastic appearance

---

### Smoothness Intensity

Controls the surface roughness.

**Higher Values**

- Cleaner polished surfaces
- Stronger reflections

**Lower Values**

- Rougher industrial materials
- More weathered appearance

---

### Ambient Occlusion Intensity

Controls the strength of the baked ambient occlusion.

**Higher Values**

- Stronger shadows in creases and panel gaps

**Lower Values**

- Softer surface shading

---

### Lights Intensity

Controls the intensity of emissive lighting effects.

Increase this value to make illuminated elements appear brighter.

---

# Panel Customisation

The shader includes a dedicated panel masking system.

The panel mask separates specific areas of the mesh, allowing those regions to be recoloured without creating additional materials.

## Panel Intensity

Controls the brightness of the masked panel areas.

## Panel Colour

Allows you to recolour every masked panel.

Examples:

- Military Interiors
- Corporate Facilities
- Alien Technology
- Warning Panels
- Faction Variations

---

# Emissive Controls

The shader includes dedicated emissive controls.

## Light Colour

Changes the emissive colour.

Examples:

- Blue Sci-Fi Lighting
- Red Warning Systems
- Green Alien Technology
- Orange Industrial Lighting

---

## Emissive Intensity

Controls how bright emissive elements appear.

**Higher Values**

- Strong glow
- More intense lighting

**Lower Values**

- Subtle illuminated details

---

# Force Field Shader Graph

The package includes a custom **Force Field Shader Graph** designed to create futuristic energy barriers, shields, and holographic effects.

The shader provides a flexible setup allowing you to easily customise the appearance and behaviour of the force field without requiring additional textures or materials.

## Customisation Controls

### Pattern

Allows you to select and customise the internal pattern displayed within the force field.

Useful for creating effects such as:

- Energy shields
- Sci-fi barriers
- Holographic displays
- Containment fields

---

### Fresnel Power

Controls the strength of the edge glow effect.

**Higher Values**

- Stronger glowing edges
- More pronounced energy field appearance

**Lower Values**

- Softer edge lighting
- More transparent appearance

---

### Force Field Colour

Controls the main colour of the energy field.

Examples:

- Blue protective shields
- Red warning barriers
- Green alien technology
- Orange industrial energy fields

---

### Scrolling Speed

Controls the movement speed of the internal force field pattern.

**Higher Values**

- Faster energy movement
- More active appearance

**Lower Values**

- Slower, calmer energy movement

---

### Pattern Offset

Controls the direction and positioning of the scrolling pattern.

This allows you to adjust the flow of energy across the surface and create different movement styles.

---

### Pattern Size

Controls the scale of the internal force field pattern.

**Higher Values**

- Larger pattern details
- Softer energy variation

**Lower Values**

- Smaller pattern details
- More detailed energy effects

---

The Force Field Shader Graph can be used for:

- Energy shields
- Force fields
- Sci-fi doors
- Holographic displays
- Containment chambers
- Futuristic barriers

---

# Texture Maps

The material for the modular kit uses the following texture maps:

| Texture | Purpose |
|----------|----------|
| Albedo | Base colour information |
| Normal | Surface detail |
| Metallic/Roughness | Material response |
| Ambient Occlusion | Additional shading depth |
| Emissive | Illuminated areas |

---

# Showcase Scene

The package includes a fully assembled **Showcase Scene**, displaying all **101 prefabs**.

The scene demonstrates:

- Every modular asset
- Example environment layout
- Material customisation
- Force Field Shader effects
- Included utility scripts

---

## SpinObject Demonstration

A fan assembly demonstrates the **SpinObject** script.

The script can be added to any object requiring continuous rotation.

### Features

- Rotation Axis
- Rotation Speed
- Play In Editor Mode

When **Play In Editor** is enabled, objects rotate inside the Scene View without entering Play Mode.

Common uses include:

- Fans
- Turbines
- Machinery
- Rotating Displays
- Mechanical Components

---

## Rainbow Material Controller Demonstration

The Showcase Scene also demonstrates the **Rainbow Material Controller**.

This script continuously cycles through colours, showcasing the flexibility of the included shader.

It animates:

- Panel Colour
- Emissive Light Colour

This provides a quick demonstration of how easily the environment can be recoloured using a single shared material.

---

# Included Scripts & Shaders

## SpinObject

Continuously rotates an object.

### Features

- Adjustable Rotation Axis
- Adjustable Rotation Speed
- Optional Play In Editor Mode

Ideal for:

- Fans
- Machinery
- Turbines
- Display Objects
- Mechanical Components

---

## Rainbow Material Controller

Demonstrates runtime material customisation.

The script smoothly animates both the emissive lighting and panel colours, providing an example of how the shader can be driven through code.

This can be used as a starting point for:

- Alarm Systems
- Dynamic Lighting
- Faction Colours
- Animated Materials
- Environmental Effects

---

## Force Field Shader Graph

A custom shader graph designed to create dynamic sci-fi energy effects.

### Features

- Customisable energy pattern
- Adjustable Fresnel edge glow
- Colour controls
- Scrolling energy movement
- Pattern scale adjustment
- Pattern offset controls

Ideal for:

- Energy shields
- Force fields
- Holograms
- Sci-fi doors
- Containment systems
- Futuristic technology

---

# Recommended Usage

For the best results:

- Use the included material as your base material.
- Duplicate the material when creating entirely new themes.
- Use the panel colour and emissive controls to create visual variations.
- Use the Force Field Shader Graph for additional futuristic effects.
- Combine modular pieces to create large environments.

---

# Performance Notes

The kit has been designed with optimisation in mind.

Recommended practices:

- Use baked lighting where appropriate.
- Avoid creating unnecessary material duplicates.
- Maintain the shared material workflow whenever possible.

---

# Support

If you encounter any issues, please ensure:

- You are using a supported version of Unity.
- The required Render Pipeline has been configured correctly.
- The included shaders and materials have not been modified incorrectly.

---

# Thank You

Thank you for supporting this asset pack.

Enjoy building your next sci-fi environment!

---

**Sci-Fi Industrial Environment Kit**

Created by **Crystal Gear Games**  
Steven Holmes