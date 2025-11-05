using Godot;

public partial class LaserDetector : Node2D
{
	[Export] public float LaserLength = 500f;
	[Export] public Color LaserColorNormal = Colors.Green;
	[Export] public Color LaserColorAlert = Colors.Red;
	[Export] public NodePath PlayerPath;
	
	private RayCast2D _rayCast;
	private Line2D _laserBeam;
	private Node2D _player;
	private bool _isAlarmActive = false;
	private Timer _alarmTimer;
	private ColorRect _alarmFlash;
	private float _flashTime = 0f;

	public override void _Ready()
	{
		SetupRaycast();
		SetupVisuals();
		SetupAlarm();
		
		// Get player reference
		if (PlayerPath != null && !PlayerPath.IsEmpty)
		{
			_player = GetNode<Node2D>(PlayerPath);
		}
		else
		{
			// Try to find player in scene
			_player = GetTree().Root.FindChild("Player", true, false) as Node2D;
		}
	}

	private void SetupRaycast()
	{
		// Create RayCast2D
		_rayCast = new RayCast2D();
		AddChild(_rayCast);
		
		// Set raycast properties
		_rayCast.Enabled = true;
		_rayCast.TargetPosition = new Vector2(LaserLength, 0);
		
		// Set collision mask to detect player (layer 1 by default)
		_rayCast.CollisionMask = 1;
		
		// Enable hit from inside
		_rayCast.HitFromInside = true;
	}

	private void SetupVisuals()
	{
		// Create Line2D for laser visualization
		_laserBeam = new Line2D();
		AddChild(_laserBeam);
		
		_laserBeam.Width = 3.0f;
		_laserBeam.DefaultColor = LaserColorNormal;
		_laserBeam.AddPoint(Vector2.Zero);
		_laserBeam.AddPoint(new Vector2(LaserLength, 0));
		
		// Add glow effect
		_laserBeam.BeginCapMode = Line2D.LineCapMode.Round;
		_laserBeam.EndCapMode = Line2D.LineCapMode.Round;
	}
	
	private void SetupAlarm()
	{
		// Create alarm timer
		_alarmTimer = new Timer();
		AddChild(_alarmTimer);
		_alarmTimer.WaitTime = 0.5f;
		_alarmTimer.OneShot = false;
		_alarmTimer.Timeout += OnAlarmTimeout;
		
		// Create visual flash effect
		_alarmFlash = new ColorRect();
		AddChild(_alarmFlash);
		_alarmFlash.Size = new Vector2(100, 100);
		_alarmFlash.Position = new Vector2(-50, -50);
		_alarmFlash.Color = new Color(1, 0, 0, 0);
		_alarmFlash.ZIndex = 10;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Force raycast to update
		_rayCast.ForceRaycastUpdate();
		
		// Check collision
		bool isColliding = _rayCast.IsColliding();
		Vector2 endPoint;
		
		if (isColliding)
		{
			// Get collision point
			endPoint = _rayCast.GetCollisionPoint();
			var collider = _rayCast.GetCollider();
			
			// Check if we hit the player
			if (_player != null && IsPlayerHit(collider))
			{
				if (!_isAlarmActive)
				{
					TriggerAlarm();
				}
			}
			else
			{
				if (_isAlarmActive)
				{
					ResetAlarm();
				}
			}
		}
		else
		{
			// No collision, use full length
			endPoint = GlobalPosition + new Vector2(LaserLength, 0);
			
			if (_isAlarmActive)
			{
				ResetAlarm();
			}
		}
		
		// Update laser beam
		UpdateLaserBeam(endPoint);
		
		// Update flash effect
		if (_isAlarmActive)
		{
			_flashTime += (float)delta;
			float alpha = (Mathf.Sin(_flashTime * 10.0f) + 1.0f) * 0.3f;
			_alarmFlash.Color = new Color(1, 0, 0, alpha);
		}
	}
	
	private bool IsPlayerHit(GodotObject collider)
	{
		if (collider == _player)
			return true;
			
		// Check if collider is a child of player
		if (collider is Node node)
		{
			Node parent = node.GetParent();
			while (parent != null)
			{
				if (parent == _player)
					return true;
				parent = parent.GetParent();
			}
		}
		
		return false;
	}

	private void UpdateLaserBeam(Vector2 endPoint)
	{
		if (_laserBeam != null && _laserBeam.GetPointCount() >= 2)
		{
			// Convert to local coordinates
			Vector2 localEnd = ToLocal(endPoint);
			_laserBeam.SetPointPosition(1, localEnd);
		}
	}

	private void TriggerAlarm()
	{
		_isAlarmActive = true;
		_laserBeam.DefaultColor = LaserColorAlert;
		_alarmTimer.Start();
		_flashTime = 0f;
		
		GD.Print("ALARM! Player detected!");
		
		// Optional: Play alarm sound
		// _alarmSound.Play();
	}

	private void ResetAlarm()
	{
		_isAlarmActive = false;
		_laserBeam.DefaultColor = LaserColorNormal;
		_alarmTimer.Stop();
		_alarmFlash.Color = new Color(1, 0, 0, 0);
		
		GD.Print("Alarm reset.");
	}
	
	private void OnAlarmTimeout()
	{
		// Periodic alarm action (could trigger sound, animation, etc.)
		if (_isAlarmActive)
		{
			GD.Print("Alarm still active!");
		}
	}
	
	// Debug drawing
	public override void _Draw()
	{
		if (_rayCast != null && _rayCast.IsColliding())
		{
			Vector2 collisionPoint = ToLocal(_rayCast.GetCollisionPoint());
			DrawCircle(collisionPoint, 5, Colors.Yellow);
		}
	}
	
	public override void _Process(double delta)
	{
		QueueRedraw();
	}
}
