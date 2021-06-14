using Godot;
using System;

public class Mob : RigidBody2D
{
	[Export]
	public int MinSpeed = 150;

	[Export]
	public int MaxSpeed = 250;

	static private Random _random = new Random();

	public override void _Ready()
	{
		var ani = GetNode<AnimatedSprite>("AnimatedSprite");
		string[] mobTypes = ani.Frames.GetAnimationNames();
		ani.Animation = mobTypes[_random.Next(0, mobTypes.Length)];

		GetNode<VisibilityNotifier2D>("VisibilityNotifier2D").Connect("screen_exited", this, nameof(OnVisibilityNotifier2DScreenExited));
	}

	public void OnVisibilityNotifier2DScreenExited()
	{
		QueueFree();
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
