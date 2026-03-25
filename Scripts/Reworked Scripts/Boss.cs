using Godot;
using GodotPlugins.Game;
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
	private Bar healthBar;
	public List<BossPart> parts = new List<BossPart>();
	public RandomNumberGenerator rnd = new RandomNumberGenerator();
	public WorldDestruction gameController = null;
	private int movementChance = 1; // movementChance = x means 1/x chance to move
	private int dashChance = 4; 
	private Vector2 actDirection;
	private int maxHP;
	private int currentHP;
	private bool isDead = false;

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
		healthBar = GetNode<Bar>("Bar");

		foreach (BossPart part in deadParts.GetChildren())
		{
			part.main = this;
			parts.Add(part);
		}
    }

	public void SetHealth(int health)
	{
		maxHP = health;
		currentHP = health;
	}

	public List<Vector2> PartPositions()
	{
		List<Vector2> partPositions = new List<Vector2>();

		foreach (BossPart part in parts)
		{
			partPositions.Add(part.GlobalPosition);
		}

		return partPositions;
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

	public void Stop()
	{
		stepTimer.Stop();
		arrowManager.ResetArrows();
		delayTimer.Stop();
	}
	public void Die()
	{
		arrowManager.ResetArrows();
		isDead = true;
		gameController.isBossDead = true;
		gameController.SetBossLabel();
		mainFrames.Visible = false;
		healthBar.Visible = false;
		mainShape.SetDeferred("disabled", true);
		stepTimer.Stop();

		gameController.ShakeCamera(15.0f, 30);

		Godot.Collections.Array<Node2D> bodies = overlap.GetOverlappingBodies();

		foreach (Node2D body in bodies)
		{
			if (body is GroundObstacle obstacle)
			{
				obstacle.Hit();
			}
			if (body is ReworkedFood food)
			{
				food.MoveFood();
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
		if (isDead == false)
		{
			arrowManager.ResetArrows();
	        bool canContinue = true;
	        do
			{
				KinematicCollision2D collision = MoveAndCollide(actDirection * size, true);
	
				if (collision is not null)
				{
					GodotObject hit = collision.GetCollider();
					if (hit is not null)
					{
						if (hit is Segment segment)
						{
							canContinue = false;
							segment.Hit();
						}
						else if (hit is TileMapLayer)
						{
							canContinue = false;
						}
					}
				}
				if (canContinue)
				{
					MoveAndCollide(actDirection * size);
				}
	
			} while (canContinue && actDirection != Vector2.Zero);
			gameController.ShakeCamera(10.0f);
			actDirection = Vector2.Zero;
		}
	}

	private void Move(Vector2 direction)
	{
		if (isDead == false)
		{
			MoveAndCollide(direction * size);
		}

	}

	private Vector2 GetRandomDirection()
	{
		int randInd = rnd.RandiRange(0, dirs.Length - 1);

		return dirs[randInd];
	}

	public void Hit(int damadge)
	{
		currentHP -= damadge;
		healthBar.SetBar((float)currentHP / (float)maxHP);
		if (currentHP <= 0)
		{
			Die();
		}
	}
}
