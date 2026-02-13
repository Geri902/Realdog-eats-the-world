using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private RealDogPainter realDogPainter;
	private Timer timer;
	private List<Vector2I> partPlaces = new List<Vector2I>();
	private Vector2I currentDirection;
	private int partCount = 0;
	public override void _Ready()
	{
		rnd.Randomize();
		realDogPainter = GetNode<RealDogPainter>("DogTiles");
		timer = GetNode<Timer>("Timer");

		timer.Timeout += HandleTimer;
		Spawn(Vector2I.Zero);
		timer.Start();
		AddPart();
		AddPart();
		AddPart();
		AddPart();
		AddPart();
	}
	public override void _Process(double delta)
	{
		if(Input.IsAnythingPressed()){
			if (Input.IsActionJustPressed("Up"))
			{
				ChangeDirection(Vector2I.Up);
			}
			else if (Input.IsActionJustPressed("Down"))
			{
				ChangeDirection(Vector2I.Down);
			}
			else if (Input.IsActionJustPressed("Left"))
			{
				ChangeDirection(Vector2I.Left);
			}
			else if (Input.IsActionJustPressed("Right"))
			{
				ChangeDirection(Vector2I.Right);
			}
		}
	}

	private bool CanTurn(Vector2I direction)
	{
		if (currentDirection == Vector2I.Up && direction == Vector2I.Down)
		{
			return false;
		}
		if (currentDirection == Vector2I.Down && direction == Vector2I.Up)
		{
			return false;
		}
		if (currentDirection == Vector2I.Left && direction == Vector2I.Right)
		{
			return false;
		}
		if (currentDirection == Vector2I.Right && direction == Vector2I.Left)
		{
			return false;
		}
		return true;
	}

	private void ChangeDirection(Vector2I direction)
	{
		if (CanTurn(direction))
		{
			currentDirection = direction;
		}
	}

	private void HandleTimer()
	{
		Move();
	}
	private void Move()
	{

		Vector2I prevPosition = partPlaces[0];
		partPlaces[0] += currentDirection;
		for (int i = 1; i < partPlaces.Count; i++)
		{
			Vector2I temp = partPlaces[i];
			partPlaces[i] = prevPosition;
			prevPosition = temp;
		}
		realDogPainter.DrawDog(partPlaces);
	}
	public void AddPart()
	{
		Vector2I pos = partPlaces[0];
		partPlaces[0] += currentDirection;
		partPlaces.Insert(1,pos);
	}

	private void Spawn(Vector2I spawnPoint)
	{
		partPlaces.Add(spawnPoint);
		int dir = rnd.RandiRange(0,3);

		switch (dir)
		{
			case 0:
				currentDirection = Vector2I.Down;
				break;
			case 1:
				currentDirection = Vector2I.Up;
				break;
			case 2:
				currentDirection = Vector2I.Right;
				break;
			default:
				currentDirection = Vector2I.Left;
				break;
		}

		AddPart();
	}
}
