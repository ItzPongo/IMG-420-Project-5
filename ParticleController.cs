using Godot;

public partial class ParticleController : GpuParticles2D
{
	private ShaderMaterial _shaderMaterial;
	private float _timeElapsed = 0f;

	public override void _Ready()
	{
		// Load custom shader
		Shader customShader = GD.Load<Shader>("res://custom_particle.gdshader");
		_shaderMaterial = new ShaderMaterial();
		_shaderMaterial.Shader = customShader;
		
		// Set shader parameters
		_shaderMaterial.SetShaderParameter("wave_intensity", 0.1f);
		_shaderMaterial.SetShaderParameter("wave_frequency", 10.0f);
		_shaderMaterial.SetShaderParameter("time_scale", 1.0f);
		_shaderMaterial.SetShaderParameter("color_start", new Color(1.0f, 0.5f, 0.0f, 1.0f));
		_shaderMaterial.SetShaderParameter("color_mid", new Color(1.0f, 0.8f, 0.2f, 1.0f));
		_shaderMaterial.SetShaderParameter("color_end", new Color(1.0f, 0.0f, 0.5f, 1.0f));
		
		// Apply shader material
		Material = _shaderMaterial;
		
		// Configure particle properties
		Amount = 100;
		Lifetime = 2.0f;
		Explosiveness = 0.0f;
		Randomness = 0.5f;
		FixedFps = 60;
		
		// Create and configure process material
		ParticleProcessMaterial processMaterial = new ParticleProcessMaterial();
		processMaterial.EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Sphere;
		processMaterial.EmissionSphereRadius = 50.0f;
		processMaterial.Direction = new Vector3(0, -1, 0);
		processMaterial.Spread = 45.0f;
		processMaterial.InitialVelocityMin = 50.0f;
		processMaterial.InitialVelocityMax = 100.0f;
		processMaterial.Gravity = new Vector3(0, 98, 0);
		processMaterial.ScaleMin = 8.0f;
		processMaterial.ScaleMax = 8.0f;
		
		ProcessMaterial = processMaterial;
		
		// Set texture (create a simple circle texture)
		Texture = CreateCircleTexture();
		
		Emitting = true;
	}

	public override void _Process(double delta)
	{
		_timeElapsed += (float)delta;
		
		// Animate shader parameters over time
		if (_shaderMaterial != null)
		{
			// Vary wave intensity
			float waveIntensity = 0.1f + Mathf.Sin(_timeElapsed * 0.5f) * 0.05f;
			_shaderMaterial.SetShaderParameter("wave_intensity", waveIntensity);
			
			// Vary time scale for interesting effects
			float timeScale = 1.0f + Mathf.Cos(_timeElapsed * 0.3f) * 0.5f;
			_shaderMaterial.SetShaderParameter("time_scale", timeScale);
		}
	}
	
	private Texture2D CreateCircleTexture()
	{
		int size = 32;
		Image image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
		
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				float dx = x - size / 2.0f;
				float dy = y - size / 2.0f;
				float distance = Mathf.Sqrt(dx * dx + dy * dy);
				float alpha = Mathf.Max(0, 1.0f - distance / (size / 2.0f));
				image.SetPixel(x, y, new Color(1, 1, 1, alpha));
			}
		}
		
		return ImageTexture.CreateFromImage(image);
	}
}
