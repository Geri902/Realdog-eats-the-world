using Godot;
using System;

public partial class Explosion : AnimatedSprite2D
{
	private Timer delay;
	private RealDogPainter dogPainter;
	private Vector2I at;
	private bool killed = false;
	public override void _Ready()
	{
		delay = GetNode<Timer>("Delay");
		delay.Timeout += Explode;
		SpriteFrames.SetAnimationLoop("default", false);
		AnimationFinished += Die;
		
	}

	public override void _Process(double delta)
	{
		if (Frame > 11 && killed == false)
		{
			dogPainter.KillCell(at);
		}
	}

	public void Start(double amount, RealDogPainter painter, Vector2I position)
	{
		at = position;
		dogPainter = painter;
		delay.WaitTime = amount;
		delay.Start();
	}

	private void Explode()
	{
		Play("default",1,false);
	}

	private void Die()
	{
		
		QueueFree();
	}
}
