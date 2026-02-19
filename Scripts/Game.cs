using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node2D
{
	[Export]
	private PackedScene explosionScene;
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private RealDogPainter realDogPainter;
	private Timer timer;
	private List<Vector2I> partPlaces = new List<Vector2I>();
	private Vector2I currentDirection;
	private Vector2I nextDirection;
	private Map map;
	private Label scoreLabel;
	private Clock clock;
	private GameOver gameOverPopup;
	private int score = 0;
	public override void _Ready()
	{
		rnd.Randomize();
		realDogPainter = GetNode<RealDogPainter>("DogTiles");
		timer = GetNode<Timer>("StepTimer");
		map = GetNode<Map>("MapTiles");
		scoreLabel = GetNode<Label>("Score");
		clock = GetNode<Clock>("Clock");
		gameOverPopup = GetNode<GameOver>("Game Over Popup");

		scoreLabel.Text = $"Score: {score}";

		map.SetRnd(rnd);

		timer.Timeout += Step;
		Spawn(Vector2I.Zero);
		map.GenerateFood(partPlaces);

		Step();
		timer.Start();

		gameOverPopup.newGameButton.Pressed += RestartGame;
	}

	private void RestartGame()
	{
		score = 0;
		scoreLabel.Text = $"Score: {score}";

		clock.RestartClock();
		timer.Start();

		Spawn(Vector2I.Zero);
		map.ResetMap();
		map.GenerateFood(partPlaces);

		gameOverPopup.Visible = false;
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
			nextDirection = direction;
		}
	}

	private void Explode()
	{
		double delay = 0.05;
		double increment = 0.05;
		foreach (Vector2I part in partPlaces)
		{
			Explosion explosion = explosionScene.Instantiate<Explosion>();
			AddChild(explosion);
			explosion.Position = part * 128;
			explosion.Start(delay, realDogPainter, part);
			delay += increment;
		}

		gameOverPopup.GetGameOver(score, clock.GetTimeText());
		partPlaces.Clear();
	}

	private void Die()
	{
		GD.Print("Die");
		Explode();
		clock.timer.Stop();
		timer.Stop();
	}

	private void IncreaseScore()
	{
		int value = 10;
		//later can add logic that gives score based on tile, length or other variables
		score += value;
		scoreLabel.Text = $"Score: {score}";
	}
	private void Eat()
	{
		AddPart();
		realDogPainter.DrawDog(partPlaces, map.IsNextFood(partPlaces[0]));
		IncreaseScore();
		GD.Print("Eat");
	}

	private void Step()
	{
		currentDirection = nextDirection;
		
		string option = map.TryMove(partPlaces, currentDirection);

		switch (option)
		{
			case "Move":
				Move();
				break;
			case "Die":
				Die();
				break;
			case "Eat":
				Eat();
				break;
			default:
				GD.Print("couldn't handle move");
				break;
		}
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
		realDogPainter.DrawDog(partPlaces, map.IsNextFood(partPlaces[0]));
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

		nextDirection = currentDirection;

		AddPart();
	}
}
