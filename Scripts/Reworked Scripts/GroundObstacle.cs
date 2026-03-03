using Godot;
using System;

public partial class GroundObstacle : Area2D
{
	public WorldDestruction gameController;
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
		if (gameController.obstacles.Contains(this))
		{
			gameController.obstacles.Remove(this);
		}
		QueueFree();
	}

	public void Hit()
	{
		animation.Play("default");
	}
	
}
