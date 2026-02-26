using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WorldDestruction : Node2D
{
	private const int size = 128;
	[Export]
	private PackedScene foodScene;
	private List<ReworkedFood> foods = new List<ReworkedFood>();
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private DogController dogController;
	private Timer stepTimer;
	private TileMapLayer borderTiles;
	private Vector2 currentDirection;
	private Vector2 nextDirection;
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
		borderTiles = GetNode<TileMapLayer>("Borders");

		stepTimer.Timeout += HandleStep;

		SetBoundries(borderTiles.GetUsedCells());

		dogController.SetUp(rnd, this);
		nextDirection = dogController.Spawn();
		SpawnFood();
		currentDirection = nextDirection;
		stepTimer.Start();

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
				ChangeDirection(Vector2.Zero);
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
		}
	}

	private void HandleStep()
	{
		currentDirection = nextDirection;
		dogController.Move(currentDirection);
	}

	private void SpawnFood()
	{
		ReworkedFood food = foodScene.Instantiate<ReworkedFood>();
		food.GlobalPosition = GetRandomNonDogPosition();
		food.SetUp(rnd);
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
	}

	private void SetBoundries(Godot.Collections.Array<Vector2I> walls)
    {
        boundry["minX"] = walls.Min(l => l.X) + 1;
        boundry["maxX"] = walls.Max(l => l.X) - 1;
        boundry["minY"] = walls.Min(l => l.Y) + 1;
        boundry["maxY"] = walls.Max(l => l.Y) - 1;

		GD.Print($"X: [{boundry["minX"]},{boundry["maxX"]}]\nY: [{boundry["minY"]},{boundry["maxY"]}]");
    }

	private Vector2 GetRandomNonDogPosition()
	{
		List<Vector2> dogPositions = dogController.GetDogPositions();

		int x;
		int y;
		do
		{
			x = rnd.RandiRange(boundry["minX"],boundry["maxX"]) * size;
			y = rnd.RandiRange(boundry["minY"],boundry["maxY"]) * size;
		} while (dogPositions.Contains(new Vector2(x,y)));


		GD.Print($"moved fruit to: {x}:{y}");
		return new Vector2(x, y);
	}
}
