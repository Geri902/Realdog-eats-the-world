using Godot;
using System;
using System.Collections.Generic;

public enum BodyType
{
	Straight = 0,
	Bend = 1,
	Tail = 2,
	HeadNormal = 3,
	HeadOpen = 4,
	AttackSmall = 5,
	AttackLarge = 6,
}

public partial class Segment : CharacterBody2D
{
	private const int size = 128;
	private AnimatedSprite2D bodyFrames;
	private AnimatedSprite2D explosionFrames;
	private Timer delayTimer;
	public bool isHead = false;
	public bool isFull = false;
	private DogController owner = null;
	public bool isAttatched = true;
	public override void _Ready()
	{
		bodyFrames = GetNode<AnimatedSprite2D>("Body Frames");
		explosionFrames = GetNode<AnimatedSprite2D>("Explosion Frames");
		delayTimer = GetNode<Timer>("Delay");

		delayTimer.Timeout += Explode;
		explosionFrames.AnimationFinished += Die;
	}

	private void Die()
	{
		if (owner.parts.Contains(this))
		{
			owner.parts.Remove(this);
		}
		else if (owner.removedParts.Contains(this))
		{
			owner.removedParts.Remove(this);
		}
		QueueFree();
	}

	public void SetUp(DogController owner)
	{
		this.owner = owner;
	}

	public override void _Process(double delta)
	{
	}
	private int CalcDeg(Vector2I currentI, Vector2I previousI, Vector2I nextI, BodyType what)
	{
		Vector2I diffNext;
		Vector2I diffPrev;

		int deg = 0;

		switch (what)
		{
			case BodyType.HeadNormal:
			case BodyType.HeadOpen:
			case BodyType.AttackSmall:
			case BodyType.AttackLarge:
				diffNext = nextI - currentI;

				if (diffNext == Vector2I.Down)
				{
					deg = 90;
				}
				else if (diffNext == Vector2I.Left)
				{
					deg = 180;
				}
				else if (diffNext == Vector2I.Up)
				{
					deg = 270;
				}
				break;
			case BodyType.Tail:
				diffPrev = previousI - currentI;

				if (diffPrev == Vector2I.Up)
				{
					deg = 90;
				}
				else if (diffPrev == Vector2I.Right)
				{
					deg = 180;
				}
				else if (diffPrev == Vector2I.Down)
				{
					deg = 270;
				}
				break;	
			default:
				diffNext = nextI - currentI;
				diffPrev = previousI - currentI;

				if (previousI.X != nextI.X && nextI.Y != previousI.Y)
				{
					if ((diffPrev == Vector2I.Left && diffNext == Vector2I.Up) || (diffNext == Vector2I.Left && diffPrev == Vector2I.Up))
					{
						deg = 90;
					}
					else if ((diffPrev == Vector2I.Right && diffNext == Vector2I.Up) || (diffNext == Vector2I.Right && diffPrev == Vector2I.Up))
					{
						deg = 180;
					}
					else if ((diffPrev == Vector2I.Right && diffNext == Vector2I.Down) || (diffNext == Vector2I.Right && diffPrev == Vector2I.Down))
					{
						deg = 270;
					}
				}
				else
				{
					if (diffNext == Vector2I.Left){
						deg = 180;
					}

					else if (diffNext == Vector2I.Up){
						deg = 270;
					}

					else if (diffNext == Vector2I.Down){
						deg = 90;
					}
				}
				break;
		}

		return deg;
	}

	public void DrawSegment(Vector2 previous, Vector2 next, BodyType what)
	{
		Vector2I currentI = new Vector2I((int)GlobalPosition.X / size, (int)GlobalPosition.Y / size);
		Vector2I previousI = new Vector2I((int)previous.X / size, (int)previous.Y / size);
		Vector2I nextI = new Vector2I((int)next.X / size, (int)next.Y / size);

		bodyFrames.RotationDegrees = CalcDeg(currentI, previousI, nextI, what);
		if (what == BodyType.Straight && previousI.X != nextI.X && nextI.Y != previousI.Y)
		{
			bodyFrames.Frame = (int)BodyType.Bend;
		}
		else
		{
			bodyFrames.Frame = (int)what;
		}

		if (isFull)
		{
			bodyFrames.Scale = new Vector2(1.25f, 1.25f);
			bodyFrames.Modulate = new Color(r: 0.5f, g: 0.5f, b: 0.5f);
		}
		else
		{
			bodyFrames.Scale = new Vector2(1, 1);
			bodyFrames.Modulate = new Color(r: 1.0f, g: 1.0f, b: 1.0f);
		}
	}

	public string MoveSegment(Vector2 direction)
	{
		KinematicCollision2D collision = MoveAndCollide(direction, true);

		if (collision is not null)
		{
			GD.Print("collision:");
			return HandleCollision(collision.GetCollider(), direction);
		}
		else
		{
			MoveAndCollide(direction);
			return "Moved";
		}


	}

	public string HandleCollision(GodotObject collider, Vector2 direction)
	{
		if (collider is TileMapLayer)
		{
			GD.Print("wall");
			return "Die";
		}
		if (collider is Segment segment)
		{
			if (segment.isAttatched)
			{
				GD.Print("Body");
				return "Die";
			}
			Vector2 prev = GlobalPosition;
			GlobalPosition += direction;
			owner.MoveRest(prev, segment);
			GD.Print("Reattatch");
			return "Reattach";

		}
		if (collider is ReworkedFood food)
		{
			GD.Print("Food");
			food.MoveFood();
			GlobalPosition += direction;
			return "Eat";
		}
		return "Can't handle";
	}
	
	public void StartExplosion(float delay)
	{
		delayTimer.WaitTime = delay;
		delayTimer.Start();
	}
	private void Explode()
	{
		explosionFrames.Play("default",1,false);
	}

	public void ClearFullness()
	{
		bodyFrames.Scale = new Vector2(1, 1);
		bodyFrames.Modulate = new Color(r: 1.0f, g: 1.0f, b: 1.0f);
	}

	public void Hit()
	{
		GD.Print("Owie");
		if (isAttatched)
		{
			owner.Die();
		}
		else
		{
			StartExplosion(0.01f);
		}
	}
}
