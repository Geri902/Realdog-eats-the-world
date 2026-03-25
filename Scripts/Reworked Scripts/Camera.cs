using Godot;
using System;

public partial class Camera : Camera2D
{
	public WorldDestruction gameController;
	private Timer shakeTimer;
	public RandomNumberGenerator rnd = new RandomNumberGenerator();
	private const int maxShakes = 7;
	private int currentShakes = 0;
	private float shakeMagnitude = 6.0f;
	private bool isShaking = false;
	public override void _Ready()
	{
		rnd.Randomize();
		shakeTimer = GetNode<Timer>("Shake");

		shakeTimer.Timeout += Shake;
	}

	
	public override void _Process(double delta)
	{
		
	}

	public void startShaking()
	{
		if (isShaking == false)
		{
			Shake();
		}
	}

	private void Shake()
	{
		if (currentShakes < maxShakes)
		{
			float x = rnd.RandfRange(-shakeMagnitude, shakeMagnitude);
			float y = rnd.RandfRange(-shakeMagnitude, shakeMagnitude);
			Position = new Vector2(x,y);

			currentShakes++;

			shakeTimer.Start();
		}
		else
		{
			Position = Vector2.Zero;
			currentShakes = 0;
			isShaking = false;
		}
	}

	public void Setup(Vector2 startPos, float duration)
	{
		Zoom = new Vector2(500, 500);
		GlobalPosition = startPos;

		Tween tween = CreateTween();

		tween.TweenProperty(this, "zoom", new Vector2(50, 50), duration/2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, "zoom", new Vector2(0.5f, 0.5f), duration/2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
		tween.Parallel().TweenProperty(this, "global_position", Vector2.Zero, duration/2).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);


		tween.Finished += AfterTween;
	}

	private void AfterTween()
	{
		gameController.StartGame();
	}
}
