using Godot;
using System;

public class Player : Area2D
{
	[Export]
	public int Speed = 400;

	[Signal]
	public delegate void Hit();

	private Vector2 _screenSize;

	private Vector2 _target;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		_screenSize = GetViewport().Size;

		this.Connect("body_entered", this, "OnPlayerBodyEntered");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventScreenTouch eventMouseButton && eventMouseButton.Pressed)
		{
			_target = (@event as InputEventScreenTouch).Position;
		}
	}

	public void Start(Vector2 pos)
	{
		Position = pos;
		_target = pos;

		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	public void OnPlayerBodyEntered(PhysicsBody2D body)
	{
		Hide();
		EmitSignal("Hit");
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		Vector2 velocity = new Vector2();
		if (Position.DistanceTo(_target) > 10)
		{
			velocity = _target - Position;
		}

		//if (Input.IsActionPressed("ui_right"))
		//{
		//    velocity.x += 1;
		//}

		//if (Input.IsActionPressed("ui_left"))
		//{
		//    velocity.x += -1;
		//}

		//if (Input.IsActionPressed("ui_down"))
		//{
		//    velocity.y += 1;
		//}

		//if (Input.IsActionPressed("ui_up"))
		//{
		//    velocity.y += -1;
		//}

		AnimatedSprite ani = GetNode<AnimatedSprite>("AnimatedSprite");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			ani.Play();
		}
		else
		{
			ani.Stop();
		}

		Position += velocity * delta;
		Position = new Vector2(Mathf.Clamp(Position.x, 0, _screenSize.x),
							   Mathf.Clamp(Position.y, 0, _screenSize.y));

		if (velocity.x != 0)
		{
			ani.Animation = "walk";
			ani.FlipV = false;
			ani.FlipH = velocity.x < 0;
		}
		else if(velocity.y != 0)
		{
			ani.Animation = "up";
			ani.FlipV = velocity.y > 0;
		}
	}
}
