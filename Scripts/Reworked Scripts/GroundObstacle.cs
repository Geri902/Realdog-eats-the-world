using Godot;
using System;

public partial class GroundObstacle : CharacterBody2D
{
	public WorldDestruction gameController;
	private AnimatedSprite2D animation;
	private CollisionShape2D hitbox;
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("Animation");
		hitbox = GetNode<CollisionShape2D>("Shape");

		animation.AnimationFinished += Die;
	}

	public override void _Process(double delta)
	{
	}

	public void Die()
	{
		if (gameController is not null && gameController.obstacles.Contains(this))
		{
			gameController.obstacles.Remove(this);
		}
		QueueFree();
	}

	public void Hit()
	{
		hitbox.SetDeferred("disabled", true);
		animation.Play("default");

	}
	
}
