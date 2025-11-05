shader_type canvas_item;

// Uniforms for customization
uniform float wave_intensity = 0.1;
uniform float wave_frequency = 10.0;
uniform float time_scale = 1.0;
uniform vec4 color_start : source_color = vec4(1.0, 0.5, 0.0, 1.0);
uniform vec4 color_end : source_color = vec4(1.0, 0.0, 0.5, 1.0);
uniform vec4 color_mid : source_color = vec4(1.0, 0.8, 0.2, 1.0);

void fragment() {
    // Get UV coordinates
    vec2 uv = UV;
    
    // Apply wave distortion based on time and position
    float wave = sin(uv.y * wave_frequency + TIME * time_scale) * wave_intensity;
    uv.x += wave;
    
    // Add secondary wave for more complex motion
    uv.y += cos(uv.x * wave_frequency * 0.5 + TIME * time_scale * 0.7) * wave_intensity * 0.5;
    
    // Create color gradient based on vertical position
    vec4 final_color;
    if (uv.y < 0.5) {
        // Blend between start and mid color
        final_color = mix(color_start, color_mid, uv.y * 2.0);
    } else {
        // Blend between mid and end color
        final_color = mix(color_mid, color_end, (uv.y - 0.5) * 2.0);
    }
    
    // Apply pulsing effect based on time
    float pulse = sin(TIME * 2.0) * 0.2 + 0.8;
    final_color.a *= pulse;
    
    // Apply texture color if exists
    vec4 tex_color = texture(TEXTURE, UV);
    final_color *= tex_color;
    
    COLOR = final_color;
}