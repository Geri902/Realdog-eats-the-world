using Godot;
using System;
using System.Collections.Generic;

public partial class ReworkedFood : CharacterBody2D
{
	private Dictionary<int, (string name, int variations)> LevelData = new Dictionary<int, (string name, int variations)>
	{
		{1, (name: "NegPal", variations: 3)},
		{2, (name: "JohnPal", variations: 2)},
	};
	private const int size = 128;
	private float maxOffset = 10f;
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private WorldDestruction gameController;
	private AnimatedSprite2D sprite;
	private Timer shiverTimer;
	private Area2D fearArea;
	private float shiverSpead = 0.05f;
	public int panicRange = 3;
	public bool isPanicing = false;
	private int currentLevel = 1;
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
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

	public void SetUp(RandomNumberGenerator rnd, WorldDestruction gameController, int currentLevel)
	{
		this.rnd = rnd;	
		this.gameController = gameController;
		if (LevelData.ContainsKey(currentLevel))
		{
			this.currentLevel = currentLevel;
		}
	}

	private Vector2 GetRandomOffset()
	{
		float x = rnd.RandfRange(-maxOffset ,maxOffset);
		float y = rnd.RandfRange(-maxOffset ,maxOffset);

		return new Vector2(x,y);
	}

	private void RandomiseSprite()
	{
		sprite.Animation = LevelData[currentLevel].name;
		sprite.Frame = rnd.RandiRange(0, LevelData[currentLevel].variations - 1);
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

	public void MoveFood()
	{
		Vector2 prevPos = GlobalPosition;
		do
		{
			GlobalPosition = gameController.GetRandomFreePosition();
		} while (GlobalPosition.IsEqualApprox(prevPos));
		RandomiseSprite();
	}
}
