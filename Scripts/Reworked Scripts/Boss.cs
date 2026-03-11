using Godot;
using System;
using System.Collections.Generic;

public partial class Boss : CharacterBody2D
{
	private const int size = 128;
	private Vector2[] dirs = {Vector2.Up, Vector2.Down, Vector2.Left, Vector2.Right};
	private CollisionShape2D mainShape;
	private AnimatedSprite2D mainFrames;
	private Node2D deadParts;
	private Timer moveTimer;
	private Area2D overlap;
	public List<BossPart> parts = new List<BossPart>();
	public RandomNumberGenerator rnd = new RandomNumberGenerator();
	private int movementChance = 2; // movementChance = x means 1/x chance to move

    public override void _Ready()
    {
		rnd.Randomize();

        mainShape = GetNode<CollisionShape2D>("Main Hitbox");
		mainFrames = GetNode<AnimatedSprite2D>("Main Frames");
		deadParts = GetNode<Node2D>("Dead Parts");
		moveTimer = GetNode<Timer>("Move Timer");
		overlap = GetNode<Area2D>("Overlap");

		foreach (BossPart part in deadParts.GetChildren())
		{
			part.main = this;
			parts.Add(part);
		}

		moveTimer.Timeout += Move;
		moveTimer.Start();
    }

	public void Die()
	{
		GD.Print("Died");
		mainFrames.Visible = false;
		mainShape.SetDeferred("disabled", true);
		moveTimer.Stop();

		Godot.Collections.Array<Node2D> bodies = overlap.GetOverlappingBodies();

		foreach (Node2D body in bodies)
		{
			if (body is GroundObstacle obstacle)
			{
				obstacle.Hit();
			}
		}

		foreach (BossPart part in parts)
		{
			part.Enable();
		}
	}

	private void Move()
	{
		int chance = rnd.RandiRange(1,movementChance);

		if (chance == 1)
		{
			Vector2 direction = GetRandomDirection();
			MoveAndCollide(direction * size, false);
		}
	}

	private Vector2 GetRandomDirection()
	{
		int randInd = rnd.RandiRange(0, dirs.Length - 1);

		return dirs[randInd];
	}
}
