using Godot;
using System;

public partial class Beam : Node2D
{
	private const int size = 128;
	private const int offset = 64;
	private AnimatedSprite2D startFrames;
	private AnimatedSprite2D bodyFrames;
	private BeamAreaController areaController;
	private bool isReady = true;
	public override void _Ready()
	{
		startFrames = GetNode<AnimatedSprite2D>("Start Frames");
		bodyFrames = GetNode<AnimatedSprite2D>("Body Frames");
		areaController = GetNode<BeamAreaController>("Area");
	}

	public override void _Process(double delta)
	{
	}

	public void FireBeam(int length, string type, Vector2 direction, Vector2 position)
	{
		if (isReady)
		{
			Visible = true;
			ReSizeFrames(length, type);
			areaController.ReSize(length, type);
			RotationDegrees = CalcRotation(direction);
			GlobalPosition = new Vector2(position.X + offset, position.Y + offset);
			
			isReady = false;
		}
	}

    private float CalcRotation(Vector2 direction)
    {
        if (direction == Vector2.Up)
		{
			return 90;
		}
		if (direction == Vector2.Right)
		{
			return 180;
		}
		if (direction == Vector2.Down)
		{
			return 270;
		}
		return 0;
    }

	public void Banish()
	{
		Position = new Vector2(-10000, -10000);
		Visible = false;
		Rotation = 0;
		isReady = true;
	}

	private void ReSizeFrames(int length, string type)
	{
		int sizeInd;
		switch (type)
		{
			case "medium":
				sizeInd = 1;
				break;
			case "large":
				sizeInd = 2;
				break;
			default:
				sizeInd = 0;
				break;
		}
		startFrames.Frame = sizeInd;
		bodyFrames.Frame = sizeInd;

		bodyFrames.Scale = new Vector2(length - 1,1);
		bodyFrames.Position = new Vector2(-size - ((length - 2) * (size / 2)), 0);
	}
}
