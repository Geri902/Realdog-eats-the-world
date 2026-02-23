using Godot;
using System;

public partial class WorldDestruction : Node2D
{
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private DogController dogController;
	private Timer stepTimer;
	private Vector2 currentDirection;
	private Vector2 nextDirection;
	public override void _Ready()
	{
		rnd.Randomize();

		dogController = GetNode<DogController>("DogController");
		stepTimer = GetNode<Timer>("Step");

		stepTimer.Timeout += HandleStep;

		dogController.SetUp(rnd);
		nextDirection = dogController.Spawn();
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
}
