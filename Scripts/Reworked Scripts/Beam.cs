using Godot;
using System;

public partial class Beam : Node2D
{
	private const int size = 128;
	private AnimatedSprite2D startFrames;
	private AnimatedSprite2D bodyFrames;
	private BeamAreaController areaController;
	public override void _Ready()
	{
		startFrames = GetNode<AnimatedSprite2D>("Start Frames");
		bodyFrames = GetNode<AnimatedSprite2D>("Body Frames");
		areaController = GetNode<BeamAreaController>("Area");
	}

	public override void _Process(double delta)
	{
	}

	private void ReSize(int length, string type, int rotation, Vector2 position)
	{
		ReSizeFrames(length, type);
		areaController.ReSize(length, type);
		Rotation = rotation;
		Position = position;
	}

	private void Banish()
	{
		Position = new Vector2(-1000, -1000);
		Visible = false;
		Rotation = 0;
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
