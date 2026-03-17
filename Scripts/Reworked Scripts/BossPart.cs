using Godot;
using System;

public partial class BossPart : CharacterBody2D
{
	private CollisionShape2D hitbox;
	public Boss main = null;
    public override void _Ready()
    {
        hitbox = GetNode<CollisionShape2D>("Hitbox");
    }

	public void Enable()
	{
		hitbox.SetDeferred("disabled", false);
		Visible = true;
	}
	public void Eaten()
	{
		if (main is not null)
		{
			if (main.parts.Count < 2)
			{
				main.gameController.BossDefeated();
				main.QueueFree();
				return;
			}
		}
		main.parts.Remove(this);
		QueueFree();
	}
}
