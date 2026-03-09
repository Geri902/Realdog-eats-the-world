using Godot;
using System;

public partial class BeamAreaController : Area2D
{
	private const int size = 128;
	private CollisionShape2D hitbox;
	public override void _Ready()
	{
		hitbox = GetNode<CollisionShape2D>("Shape");
		BodyEntered += HandleHit;
	}

	public override void _Process(double delta)
	{
	}

	public void ReSize(int length, string type)
	{
		int y = 1;

		switch (type)
		{
			case "medium":
				y = 3;
				break;
			case "large":
				y = 5;
				break;
			default:
				break;
		}

		hitbox.Scale = new Vector2(length,y);
		hitbox.Position = new Vector2(0 - ((length - 1) * (size / 2)), 0);
	}

	public void HandleHit(Node2D body)
	{
		if (body is ReworkedFood food)
		{
			food.MoveFood();
		}
		else if(body is GroundObstacle obstacle)
		{
			obstacle.Hit();
		}
	}
}
