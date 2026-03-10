using Godot;
using System;
using System.Collections.Generic;

public partial class Boss : CharacterBody2D
{
	private CollisionShape2D mainShape;
	private AnimatedSprite2D mainFrames;
	private Node2D deadParts;
	public List<BossPart> parts = new List<BossPart>();

    public override void _Ready()
    {
        mainShape = GetNode<CollisionShape2D>("Main Hitbox");
		mainFrames = GetNode<AnimatedSprite2D>("Main Frames");
		deadParts = GetNode<Node2D>("Dead Parts");

		foreach (BossPart part in deadParts.GetChildren())
		{
			part.main = this;
			parts.Add(part);
		}
    }

	public void Die()
	{
		GD.Print("Died");
		mainFrames.Visible = false;
		mainShape.Disabled = true;

		foreach (BossPart part in parts)
		{
			part.Enable();
		}
	}
}
