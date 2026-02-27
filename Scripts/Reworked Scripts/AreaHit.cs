using Godot;
using System;

public partial class AreaHit : Area2D
{
	private const int size = 128;
	private const int WarningSetId = 0;
	private const int WarningTerrainId = 1;
	private Timer countdownTimer;
	private AnimatedSprite2D animation;
	private TileMapLayer warningTiles;
	private CollisionShape2D shape;
	private int area;
	private bool active = false;
	public override void _Ready()
	{
		countdownTimer = GetNode<Timer>("Countdown");
		animation = GetNode<AnimatedSprite2D>("Animation");
		warningTiles = GetNode<TileMapLayer>("Warning Tiles");
		shape = GetNode<CollisionShape2D>("Shape");

		countdownTimer.Timeout += Hit;
		animation.AnimationFinished += Finish;

		StartUp("5x5", 1.0f, new Vector2(1000,500));
	}

	public override void _Process(double delta)
	{
		if (active)
		{
			//sin fv alapján warning tile visability
		}

		if (true)
		{
			//check 
		}
	}

	public void StartUp(string size, float countdown, Vector2 position)
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
		countdownTimer.WaitTime = countdown;
		countdownTimer.Start();
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
					GD.Print("Food");
					food.MoveFood();
				}
				else if (body is Segment segment)
				{
					GD.Print("Segment");
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


}
