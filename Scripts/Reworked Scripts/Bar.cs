using Godot;
using System;

public partial class Bar : AnimatedSprite2D
{
	private AnimatedSprite2D content;
	public override void _Ready()
	{
		content = GetChild<AnimatedSprite2D>(0);
	}

	public void SetBar(float width)
	{
		content.Scale = new Vector2(width, 1);
	}
}
