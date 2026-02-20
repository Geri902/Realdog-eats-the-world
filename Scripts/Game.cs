using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node2D
{
	[Export]
	public PackedScene explosionScene;
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
	public string gameMode = "Normal";
	public MainMenu mainMenu = null;
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

		map.SetMap(rnd, this);

		timer.Timeout += Step;
		Spawn(Vector2I.Zero);
		map.GenerateFood();

		Step();
		timer.Start();

		gameOverPopup.newGameButton.Pressed += RestartGame;
		gameOverPopup.mainMenuButton.Pressed += BackToMainMenu;

		if (gameMode == "World Destruction")
		{
			map.wallSpawnTimer.Start();
		}

		GD.Print(gameMode);
	}

	private void BackToMainMenu()
	{
		if (mainMenu is not null)
		{
			mainMenu.BackToMenu();
		}
	}

	private void RestartGame()
	{
		score = 0;
		scoreLabel.Text = $"Score: {score}";

		clock.RestartClock();
		timer.Start();

		realDogPainter.Clear();
		Spawn(Vector2I.Zero);
		map.ResetMap();
		map.GenerateFood();

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
			else if (Input.IsActionJustPressed("Fire"))
			{
				ChangeDirection(Vector2I.Zero);
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

	public void Die()
	{
		Explode();
		clock.timer.Stop();
		timer.Stop();
		map.wallSpawnTimer.Stop();
	}

	private void IncreaseScore()
	{
		int value = 10;
		if (gameMode == "World Destruction")
		{
			int pc = partPlaces.Count() - 1;
			value = 10 ^ (int)Math.Floor(pc / 4.0) * pc; 
			//this needs to be fixed
		}
		score += value;
		scoreLabel.Text = $"Score: {score}";
	}
	private void Eat()
	{
		AddPart();
		if (map.IsNextFood(partPlaces[0]))
		{
			realDogPainter.DrawDog(partPlaces, "Hungry");
		}
		else
		{
			realDogPainter.DrawDog(partPlaces, "Normal");
		}
		IncreaseScore();
	}

	private void Step()
	{
		Vector2I previousDirection = currentDirection;
		currentDirection = nextDirection;
		
		string option = map.TryMove(currentDirection);

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
			case "Fire":
				currentDirection = previousDirection;
				nextDirection = currentDirection;
				Fire();
				break;
			default:
				GD.Print("couldn't handle move");
				break;
		}
	}

	private void Fire()
	{
		//big beam broken
		Godot.Collections.Array<Vector2I> beam = map.GetBeamArea(false, currentDirection);
		//needs guards to not remove too much
		partPlaces.RemoveAt(partPlaces.Count() - 1);
		realDogPainter.Beam(false, beam, partPlaces);
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
		if (map.IsNextFood(partPlaces[0]))
		{
			realDogPainter.DrawDog(partPlaces, "Hungry");
		}
		else
		{
			realDogPainter.DrawDog(partPlaces, "Head");
		}
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

	public List<Vector2I> GetDogParts()
	{
		return partPlaces;
	}
}
