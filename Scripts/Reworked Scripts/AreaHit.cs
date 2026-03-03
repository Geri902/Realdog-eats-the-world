using Godot;
using System;

public partial class AreaHit : Area2D
{
	private const int size = 128;
	private const int WarningSetId = 0;
	private const int WarningTerrainId = 1;
	private Timer countdownTimer;
	private Timer visibilityTimer;
	private AnimatedSprite2D animation;
	private TileMapLayer warningTiles;
	private CollisionShape2D shape;
	private int area;
	private float vis = 1;
	private float visStep = 0.10f;
	private double visDelay = 0.05f;
	private int flickerCount = 3;
	private bool inc = false;
	public override void _Ready()
	{
		countdownTimer = GetNode<Timer>("Countdown");
		visibilityTimer = GetNode<Timer>("Visibility Timer");
		animation = GetNode<AnimatedSprite2D>("Animation");
		warningTiles = GetNode<TileMapLayer>("Warning Tiles");
		shape = GetNode<CollisionShape2D>("Shape");

		countdownTimer.Timeout += Hit;
		visibilityTimer.Timeout += ChangeVisibility;
		animation.AnimationFinished += Finish;
	}

	public override void _Process(double delta)
	{
	}

	public void StartUp(string size, double countdown, Vector2 position)
	{
		GlobalPosition = position;
		switch (size)
		{
			case "1x1":
				area = 1;
				break;
			case "3x3":
				area = 2;
				break;
			case "5x5":
				area = 3;
				break;
			default:
				break;
		}
		SetupArea();
		ReSize();

		visDelay = countdown / (flickerCount * 2) * visStep;

		countdownTimer.WaitTime = countdown;
		countdownTimer.Start();
		visibilityTimer.WaitTime = visDelay;
		visibilityTimer.Start();
	}

	private void ReSize()
	{
		shape.Scale *= area;
		animation.Scale *= area;
	}

	private void SetupArea()
	{
		warningTiles.Visible = true;
		Godot.Collections.Array<Vector2I> spaces = new Godot.Collections.Array<Vector2I>();

		for (int x = -area + 1; x < area; x++)
		{
			for (int y = -area + 1; y < area; y++)
			{
				spaces.Add(new Vector2I(x, y));
			}
		}

		warningTiles.SetCellsTerrainConnect(spaces, WarningSetId, WarningTerrainId);
	}

	private void Hit()
	{
		warningTiles.Visible = false;
		if (HasOverlappingBodies())
		{
			Godot.Collections.Array<Node2D> bodies = GetOverlappingBodies();

			foreach (Node2D body in bodies)
			{
				GD.Print("Hit:");
				if (body is ReworkedFood food)
				{
					food.MoveFood();
				}
				else if (body is Segment segment)
				{
					segment.Hit();
				}
			}
		}
		animation.Play("default");
	}

	private void Finish()
	{
		Visible = false;
		QueueFree();
	}

	private void ChangeVisibility()
	{
		if (inc)
		{
			if (vis + visStep > 1)
			{
				vis = 1;
				inc = false;
			}
			else
			{
				vis += visStep;
			}
		}
		else
		{
			if (vis - visStep < 0)
			{
				vis = 0;
				inc = true;
			}
			else
			{
				vis -= visStep;
			}
		}

		warningTiles.Modulate = new Color(1.0f, 1.0f, 1.0f, vis);
	}

}
