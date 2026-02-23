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

public partial class Segment : Area2D
{
	private const int size = 128;
	private AnimatedSprite2D frames;
	public bool isHead = false;
	public override void _Ready()
	{
		frames = GetNode<AnimatedSprite2D>("Frames");
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
		
		frames.RotationDegrees = CalcDeg(currentI, previousI, nextI, what);
		if (what == BodyType.Straight && previousI.X != nextI.X && nextI.Y != previousI.Y)
		{
			frames.Frame = (int)BodyType.Bend;
		}
		frames.Frame = (int)what;
	}
}
