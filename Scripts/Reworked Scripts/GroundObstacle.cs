using Godot;
using System;

public partial class GroundObstacle : Area2D
{
	private AnimatedSprite2D animation;
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("Animation");
		animation.AnimationFinished += Die;

		Hit();
	}

	public override void _Process(double delta)
	{
	}

	private void Die()
	{
		QueueFree();
	}

	public void Hit()
	{
		animation.Play("default");
	}
	
}
