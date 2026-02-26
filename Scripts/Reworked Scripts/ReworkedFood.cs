using Godot;
using System;

public partial class ReworkedFood : StaticBody2D
{
	private float maxOffset = 10f;
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
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

	public void SetUp(RandomNumberGenerator rnd)
	{
		this.rnd = rnd;	
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
}
