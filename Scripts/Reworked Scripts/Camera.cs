using Godot;
using System;

public partial class Camera : Camera2D
{
	public WorldDestruction gameController;
	public override void _Ready()
	{
	}

	
	public override void _Process(double delta)
	{
		
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
