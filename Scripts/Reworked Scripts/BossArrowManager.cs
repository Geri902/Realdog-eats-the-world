using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BossArrowManager : Node2D
{
	private AnimatedSprite2D up;
	private AnimatedSprite2D down;
	private AnimatedSprite2D left;
	private AnimatedSprite2D right;
	private Dictionary<Vector2, AnimatedSprite2D> arrows = new Dictionary<Vector2, AnimatedSprite2D>();
	public override void _Ready()
	{
		up = GetNode<AnimatedSprite2D>("Up");
		down = GetNode<AnimatedSprite2D>("Down");
		left = GetNode<AnimatedSprite2D>("Left");
		right = GetNode<AnimatedSprite2D>("Right");

		arrows.Add(Vector2.Up, up);
		arrows.Add(Vector2.Down, down);
		arrows.Add(Vector2.Left, left);
		arrows.Add(Vector2.Right, right);
	}

	public void SetArrow(Vector2 direction)
	{
		arrows[direction].Visible = true;
	}

	public void ResetArrows()
	{
		foreach (KeyValuePair<Vector2,AnimatedSprite2D> kvp in arrows)
		{
			kvp.Value.Visible = false;
		}
	}

	public bool CanAct()
	{
		return arrows.All(x=>x.Value.Visible == false);
	}

}
