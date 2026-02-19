using Godot;
using System;

public partial class FoodScene : Node2D
{
	private float maxOffset = 10f;
	private RandomNumberGenerator rnd;
	private Sprite2D sprite;
	private Timer shiverTimer;
	private float shiverSpead = 0.05f;
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite");
		shiverTimer = GetNode<Timer>("Timer");

		shiverTimer.WaitTime = shiverSpead;
		shiverTimer.Timeout += Shiver;

	}

	public override void _Process(double delta)
	{
	}

	public void SetRnd(RandomNumberGenerator rnd)
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
	private void Panic()
	{
		shiverTimer.Start();
		Shiver();
	}

	public void CalmDown()
	{
		shiverTimer.Stop();
		sprite.Offset = Vector2.Zero;
	}
}
