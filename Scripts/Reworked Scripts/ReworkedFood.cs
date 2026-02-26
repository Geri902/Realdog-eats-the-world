using Godot;
using System;
using System.Collections.Generic;

public partial class ReworkedFood : CharacterBody2D
{
	private const int size = 128;
	private float maxOffset = 10f;
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private Dictionary<string, int> boundry;
	private DogController dogController;
	private Sprite2D sprite;
	private Timer shiverTimer;
	private Area2D fearArea;
	private float shiverSpead = 0.05f;
	public int panicRange = 3;
	public bool isPanicing = false;
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite");
		shiverTimer = GetNode<Timer>("Timer");
		fearArea = GetNode<Area2D>("Fear Area");

		shiverTimer.WaitTime = shiverSpead;
		shiverTimer.Timeout += Shiver;

		fearArea.BodyEntered += OnEnter;
		fearArea.BodyExited += OnExit;

	}

	public override void _Process(double delta)
	{
	}

	public void SetUp(RandomNumberGenerator rnd, Dictionary<string, int> boundry, DogController dogController)
	{
		this.rnd = rnd;	
		this.boundry = boundry;
		this.dogController = dogController;
	}

	private Vector2 GetRandomOffset()
	{
		float x = rnd.RandfRange(-maxOffset ,maxOffset);
		float y = rnd.RandfRange(-maxOffset ,maxOffset);

		return new Vector2(x,y);
	}

	private void Shiver()
	{
		sprite.Offset = GetRandomOffset();
	}
	public void Panic()
	{
		shiverTimer.Start();
		isPanicing = true;
		Shiver();
	}

	public void CalmDown()
	{
		shiverTimer.Stop();
		isPanicing = false;
		sprite.Offset = Vector2.Zero;
	}

	private void OnEnter(Node2D body)
	{
		if (body is Segment segment)
		{
			if (segment.isHead)
			{
				Panic();
			}
		}
	}

	private void OnExit(Node2D body)
	{
		if (body is Segment segment)
		{
			if (segment.isHead)
			{
				CalmDown();
			}
		}
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

	public void MoveFood()
	{
		Vector2 prevPos = GlobalPosition;
		do
		{
			GlobalPosition = GetRandomNonDogPosition();
		} while (GlobalPosition.IsEqualApprox(prevPos));
	}
}
