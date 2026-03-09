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
	[Export]
	private PackedScene groundObstacleScene;
	public List<GroundObstacle> obstacles = new List<GroundObstacle>();
	private List<ReworkedFood> foods = new List<ReworkedFood>();
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private DogController dogController;
	private Timer stepTimer;
	private Timer areaHitTimer;
	private Timer obstacleTimer;
	private TileMapLayer borderTiles;
	private Beam beam;
	private AnimatedSprite2D chargeMeter;
	private Label coolDownLabel;
	private Vector2 currentDirection;
	private Vector2 nextDirection;
	private bool willDash = false;
	private bool willFire = false;
	private bool didFire = false;
	private const int maxFireCharge = 3;
	private int currentCharge = 0;
	private const int coolDownDuration = 5;
	private int coolDownCurrent = 0;

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
		obstacleTimer = GetNode<Timer>("Obstacle");
		borderTiles = GetNode<TileMapLayer>("Borders");
		beam = GetNode<Beam>("Beam");
		chargeMeter = GetNode<AnimatedSprite2D>("Charge");
		coolDownLabel = GetNode<Label>("CoolDown");
		
		stepTimer.Timeout += HandleStep;
		areaHitTimer.Timeout += SpawnHit;
		obstacleTimer.Timeout += SpawnObstacle;

		SetBoundries(borderTiles.GetUsedCells());

		dogController.SetUp(rnd, this);
		nextDirection = dogController.Spawn();
		SpawnFood();
		currentDirection = nextDirection;

		stepTimer.Start();
		areaHitTimer.Start();
		obstacleTimer.Start();

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

	private void Fire(int charge)
	{
		string size;

		switch (charge)
		{
			case 2:
				size = "medium";
				break;
			case 3:
				size = "large";
				break;
			default:
				size = "small";
				break;
		}

		beam.FireBeam(BeamLength(), size, currentDirection, dogController.parts[0].GlobalPosition);
		didFire = true;
	}

	private bool CanFire()
	{
		if (BeamLength() > 1)
		{
			return true;
		}
		return false;
	}

	private int BeamLength()
	{
		int length = 1;
		Vector2 current = dogController.parts[0].GlobalPosition / size;
		while (current.X < boundry["maxX"] && current.X > boundry["minX"] && current.Y < boundry["maxY"] && current.Y > boundry["minY"])
		{
			current += currentDirection;
			length++;
		}
		return length;
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

	private void ChangeCoolDownText()
	{
		string output;

		if (coolDownCurrent != 0)
		{
			output = coolDownCurrent.ToString();
		}
		else
		{
			output = "";
		}

		coolDownLabel.Text = output;
	}

	private bool HandleFiring()
	{
		if (didFire)
		{
			beam.Banish();
			didFire = false;
		}

		if (coolDownCurrent == 0)
		{
			if (Input.IsActionPressed("Fire"))
			{
				willFire = true;
				currentCharge += 1;
				chargeMeter.Frame = currentCharge;

				if (currentCharge > maxFireCharge)
				{
					dogController.Die();
					GameOver();
				}
			}
			else
			{
				if (willFire)
				{
					Fire(currentCharge);
					willFire = false;
					didFire = true;
					coolDownCurrent = coolDownDuration;
					currentCharge = 0;
					chargeMeter.Frame = 0;

					ChangeCoolDownText();

					return true;
				}
			}
		}
		else
		{
			coolDownCurrent -= 1;
			ChangeCoolDownText();
		}


		return false;
	}

	private void HandleStep()
	{
		if (HandleFiring())
		{
			return;
		}

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
		food.SetUp(rnd, this);
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
		currentCharge = 0;
		coolDownCurrent = 0;
		willDash = false;
		willFire = false;
		didFire = false;

		stepTimer.Stop();
		obstacleTimer.Stop();
		areaHitTimer.Stop();
	}

	private void SetBoundries(Godot.Collections.Array<Vector2I> walls)
    {
        boundry["minX"] = walls.Min(l => l.X) + 1;
        boundry["maxX"] = walls.Max(l => l.X) - 1;
        boundry["minY"] = walls.Min(l => l.Y) + 1;
        boundry["maxY"] = walls.Max(l => l.Y) - 1;
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

	private void SpawnObstacle()
	{
		GroundObstacle obstacle = groundObstacleScene.Instantiate<GroundObstacle>();
		obstacle.gameController = this;
		obstacles.Add(obstacle);
		obstacle.GlobalPosition = GetRandomFreePosition();
		AddChild(obstacle);

	}

    public List<Vector2> ObstaclePositions()
    {
        List<Vector2> obstaclePositions = new List<Vector2>();

		foreach (GroundObstacle obstacle in obstacles)
		{
			obstaclePositions.Add(obstacle.GlobalPosition);
		}

		return obstaclePositions;
    }

	public Vector2 GetRandomFreePosition()
	{
		List<Vector2> occupied = [.. ObstaclePositions(), .. dogController.GetDogPositions()];

		if (dogController.parts.Count > 0)
		{
			Vector2 nextPos = dogController.parts[0].GlobalPosition + currentDirection;
			occupied.Add(nextPos);
		}

		foreach (ReworkedFood food in foods)
		{
			occupied.Add(food.GlobalPosition);
		}

		if (occupied.Count < (Math.Abs(boundry["minX"]) + boundry["maxX"] + 1) * (Math.Abs(boundry["minY"]) + boundry["maxY"] + 1))
		{
			int x;
			int y;
			do
			{
				x = rnd.RandiRange(boundry["minX"],boundry["maxX"]) * size;
				y = rnd.RandiRange(boundry["minY"],boundry["maxY"]) * size;
			} while (occupied.Any(i=>i.IsEqualApprox(new Vector2(x, y))));

			return new Vector2(x, y);
		}
		return new Vector2(-1000, -1000);
	}
}
