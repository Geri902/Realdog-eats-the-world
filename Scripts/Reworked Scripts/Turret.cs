using Godot;
using System;

public partial class Turret : Sprite2D
{
	
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		//replace with player coords (maybe don't even need script for self?)
		Vector2 mousePos = GetGlobalMousePosition();

		LookAt(mousePos);
	}
}
