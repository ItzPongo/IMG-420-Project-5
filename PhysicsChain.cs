using Godot;
using System.Collections.Generic;

public partial class PhysicsChain : Node2D
{
	[Export] public int ChainSegments = 5;
	[Export] public float SegmentDistance = 30f;
	[Export] public PackedScene SegmentScene;
	
	private List<RigidBody2D> _segments = new List<RigidBody2D>();
	private List<Joint2D> _joints = new List<Joint2D>();
	private Node2D _anchorPoint;

	public override void _Ready()
	{
		CreateChain();
	}

	private void CreateChain()
	{
		Vector2 startPosition = GlobalPosition;
		Node2D previousBody = null;
		
		for (int i = 0; i < ChainSegments; i++)
		{
			RigidBody2D segment;
			
			// First segment is static (anchor point)
			if (i == 0)
			{
				StaticBody2D staticSegment = new StaticBody2D();
				staticSegment.Position = startPosition;
				AddChild(staticSegment);
				
				// Add collision shape
				CollisionShape2D collisionShape = new CollisionShape2D();
				RectangleShape2D shape = new RectangleShape2D();
				shape.Size = new Vector2(20, 20);
				collisionShape.Shape = shape;
				staticSegment.AddChild(collisionShape);
				
				// Add visual
				ColorRect visual = new ColorRect();
				visual.Size = new Vector2(20, 20);
				visual.Position = new Vector2(-10, -10);
				visual.Color = new Color(0.3f, 0.3f, 0.8f);
				staticSegment.AddChild(visual);
				
				// Store as anchor point, not in segments list
				_anchorPoint = staticSegment;
				previousBody = staticSegment;
				continue;
			}
			
			// Create regular rigid body segment
			if (SegmentScene != null)
			{
				segment = SegmentScene.Instantiate<RigidBody2D>();
			}
			else
			{
				segment = CreateDefaultSegment();
			}
			
			// Position the segment
			segment.Position = startPosition + new Vector2(0, i * SegmentDistance);
			AddChild(segment);
			_segments.Add(segment);
			
			// Set physics properties
			segment.Mass = 1.0f;
			segment.GravityScale = 1.0f;
			segment.LinearDamp = 0.5f;
			segment.AngularDamp = 0.5f;
			
			// Connect to previous segment with a joint
			if (previousBody != null)
			{
				PinJoint2D joint = new PinJoint2D();
				AddChild(joint);
				
				// Set joint properties
				joint.NodeA = previousBody.GetPath();
				joint.NodeB = segment.GetPath();
				
				// Position joint between segments
				joint.Position = startPosition + new Vector2(0, (i - 0.5f) * SegmentDistance);
				
				// Configure joint softness and damping for realistic behavior
				joint.Softness = 0.1f;  // Makes joint slightly flexible
				joint.Bias = 0.3f;      // How quickly joint corrects errors
				
				_joints.Add(joint);
			}
			
			previousBody = segment;
		}
	}
	
	private RigidBody2D CreateDefaultSegment()
	{
		RigidBody2D segment = new RigidBody2D();
		
		// Add collision shape
		CollisionShape2D collisionShape = new CollisionShape2D();
		RectangleShape2D shape = new RectangleShape2D();
		shape.Size = new Vector2(15, 25);
		collisionShape.Shape = shape;
		segment.AddChild(collisionShape);
		
		// Add visual representation
		ColorRect visual = new ColorRect();
		visual.Size = new Vector2(15, 25);
		visual.Position = new Vector2(-7.5f, -12.5f);
		visual.Color = new Color(0.8f, 0.2f, 0.2f);
		segment.AddChild(visual);
		
		return segment;
	}

	// Apply force to a specific segment
	public void ApplyForceToSegment(int segmentIndex, Vector2 force)
	{
		if (segmentIndex >= 0 && segmentIndex < _segments.Count)
		{
			_segments[segmentIndex].ApplyImpulse(force);
		}
	}
	
	// Apply force to last segment (useful for testing)
	public void ApplyForceToEnd(Vector2 force)
	{
		if (_segments.Count > 0)
		{
			_segments[_segments.Count - 1].ApplyImpulse(force);
		}
	}
	
	// For testing - can be called from _Process or input
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.Keycode == Key.Space)
			{
				// Apply random force when spacebar is pressed
				Vector2 randomForce = new Vector2(
					(float)GD.RandRange(-500, 500),
					(float)GD.RandRange(-500, 500)
				);
				ApplyForceToEnd(randomForce);
			}
		}
	}
}
