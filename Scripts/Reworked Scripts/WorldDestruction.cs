using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WorldDestruction : Node2D
{
	private const int size = 128;
	[Export]
	private PackedScene foodScene;
	[Export]
	private PackedScene areaHitScene;
	private List<ReworkedFood> foods = new List<ReworkedFood>();
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private DogController dogController;
	private Timer stepTimer;
	private Timer areaHitTimer;
	private TileMapLayer borderTiles;
	private Vector2 currentDirection;
	private Vector2 nextDirection;
	private bool willDash = false;
	private Dictionary<string, int> boundry = new Dictionary<string, int>()
    {
        {"minX",0},
        {"maxX",0},
        {"minY",0},
        {"maxY",0},
        
    };
	public override void _Ready()
	{
		rnd.Randomize();

		dogController = GetNode<DogController>("DogController");
		stepTimer = GetNode<Timer>("Step");
		areaHitTimer = GetNode<Timer>("Area Hit");
		borderTiles = GetNode<TileMapLayer>("Borders");

		stepTimer.Timeout += HandleStep;
		areaHitTimer.Timeout += SpawnHit;

		SetBoundries(borderTiles.GetUsedCells());

		dogController.SetUp(rnd, this);
		nextDirection = dogController.Spawn();
		SpawnFood();
		currentDirection = nextDirection;
		stepTimer.Start();
		areaHitTimer.Start();

	}

	public override void _Process(double delta)
	{
		if(Input.IsAnythingPressed()){
			if (Input.IsActionJustPressed("Up"))
			{
				ChangeDirection(Vector2.Up);
			}
			else if (Input.IsActionJustPressed("Down"))
			{
				ChangeDirection(Vector2.Down);
			}
			else if (Input.IsActionJustPressed("Left"))
			{
				ChangeDirection(Vector2.Left);
			}
			else if (Input.IsActionJustPressed("Right"))
			{
				ChangeDirection(Vector2.Right);
			}
			else if (Input.IsActionJustPressed("Fire"))
			{
				GD.Print("Fire");
			}
			else if(Input.IsActionJustPressed("Dash"))
			{
				if (willDash)
				{
					willDash = false;
				}
				else
				{
					willDash = true;
				}
			}
		}
	}

	private bool CanTurn(Vector2 direction)
	{
		if (currentDirection == Vector2.Up && direction == Vector2.Down)
		{
			return false;
		}
		if (currentDirection == Vector2.Down && direction == Vector2.Up)
		{
			return false;
		}
		if (currentDirection == Vector2.Left && direction == Vector2.Right)
		{
			return false;
		}
		if (currentDirection == Vector2.Right && direction == Vector2.Left)
		{
			return false;
		}
		return true;
	}

	private void ChangeDirection(Vector2 direction)
	{
		if (CanTurn(direction))
		{
			nextDirection = direction;
			willDash = false;
		}
	}

	private void HandleStep()
	{
		if (willDash)
		{
			bool did = dogController.Dash(currentDirection);
			if (did)
			{
				stepTimer.Start();
			}
			else
			{
				currentDirection = nextDirection;
				dogController.Move(currentDirection);
			}
			willDash = false;
		}
		else
		{
			currentDirection = nextDirection;
			dogController.Move(currentDirection);
		}
	}

	private void SpawnFood()
	{
		ReworkedFood food = foodScene.Instantiate<ReworkedFood>();
		food.SetUp(rnd, boundry, dogController);
		food.MoveFood();
		AddChild(food);
		foods.Add(food);
	}

	private void ResetFood()
	{
		foods.Clear();
		SpawnFood();
	}

	public void GameOver()
	{
		stepTimer.Stop();
		areaHitTimer.Stop();
	}

	private void SetBoundries(Godot.Collections.Array<Vector2I> walls)
    {
        boundry["minX"] = walls.Min(l => l.X) + 1;
        boundry["maxX"] = walls.Max(l => l.X) - 1;
        boundry["minY"] = walls.Min(l => l.Y) + 1;
        boundry["maxY"] = walls.Max(l => l.Y) - 1;

		GD.Print($"X: [{boundry["minX"]},{boundry["maxX"]}]\nY: [{boundry["minY"]},{boundry["maxY"]}]");
    }

	private Vector2 GetRandomAreaCenter(int areaSize)
	{
		int x = rnd.RandiRange(boundry["minX"] + areaSize,boundry["maxX"] - areaSize) * size;
		int y = rnd.RandiRange(boundry["minY"] + areaSize,boundry["maxY"] - areaSize) * size;

		return new Vector2(x, y);
	}

	private void SpawnHit()
	{
		int areaSize = rnd.RandiRange(1,3);
		string area = "";

		switch (areaSize)
		{
			case 1:
				area = "1x1";
				break;
			case 2:
				area = "3x3";
				break;
			case 3:
				area = "5x5";
				break;
			default:
				break;
		}

		AreaHit hit = areaHitScene.Instantiate<AreaHit>();
		AddChild(hit);
		Vector2 position = GetRandomAreaCenter(areaSize);
		float countdown = 4.0f;

		hit.StartUp(area, stepTimer.WaitTime * countdown, position);
	}
}
