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
	private Timer stepTimer;
	private Area2D overlap;
	private BossArrowManager arrowManager;
	private Timer delayTimer;
	public List<BossPart> parts = new List<BossPart>();
	public RandomNumberGenerator rnd = new RandomNumberGenerator();
	private int movementChance = 2; // movementChance = x means 1/x chance to move
	private int dashChance = 3; 
	private Vector2 actDirection;

    public override void _Ready()
    {
		rnd.Randomize();

        mainShape = GetNode<CollisionShape2D>("Main Hitbox");
		mainFrames = GetNode<AnimatedSprite2D>("Main Frames");
		deadParts = GetNode<Node2D>("Dead Parts");
		stepTimer = GetNode<Timer>("Step");
		overlap = GetNode<Area2D>("Overlap");
		arrowManager = GetNode<BossArrowManager>("Arrows");
		delayTimer = GetNode<Timer>("Delay");

		foreach (BossPart part in deadParts.GetChildren())
		{
			part.main = this;
			parts.Add(part);
		}

		SetupTimers("");
    }

	public void SetupTimers(string action)
	{
		switch (action)
		{
			case "Cannon":
				//delayTimer.Timeout += later boss action
				break;
			default:
				delayTimer.Timeout += Dash;
				break;
		}
		
		stepTimer.Timeout += Step;
		stepTimer.Start();
	}

	public void Die()
	{
		mainFrames.Visible = false;
		mainShape.SetDeferred("disabled", true);
		stepTimer.Stop();

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

	private void Step()
	{
		int chance = rnd.RandiRange(1,dashChance);
		Vector2 direction = GetRandomDirection();
		
		if (arrowManager.CanAct())
		{
			if (chance == 1)
			{
				arrowManager.SetArrow(direction);
				actDirection = direction;
				delayTimer.Start();
			}
			else
			{
				chance = rnd.RandiRange(1,movementChance);
	
				if (chance == 1)
				{
					Move(direction);
				}
			}
		}

	}

	private void Dash()
	{
		arrowManager.ResetArrows();
        KinematicCollision2D collision;
        do
		{
			collision = MoveAndCollide(actDirection * size);
		} while (collision is null && actDirection != Vector2.Zero);
		actDirection = Vector2.Zero;
	}

	private void Move(Vector2 direction)
	{
		MoveAndCollide(direction * size);

	}

	private Vector2 GetRandomDirection()
	{
		int randInd = rnd.RandiRange(0, dirs.Length - 1);

		return dirs[randInd];
	}
}
