using Godot;
using System;
using System.Collections.Generic;

public partial class Flickerer : Node2D
{
	public Godot.Collections.Array<Vector2I> positions;
	public Map map;
	public string placing;
	public int flickerCount;
	private int currentFlicker = 0;
	public Timer flicker;
	public override void _Ready()
	{
		flicker = GetNode<Timer>("Flicker");

		flicker.Timeout += Tick;
	}

	public override void _Process(double delta)
	{
		
	}

	public void StartFlicker(Godot.Collections.Array<Vector2I> positions, Map map, string placing, int flickerCount)
	{
		this.positions = positions;
		this.map = map;
		this.placing = placing;
		this.flickerCount = flickerCount * 2;

		flicker.Start();
	}

	private void Tick()
	{
		if (currentFlicker < flickerCount)
		{
			if (currentFlicker % 2 == 0)
			{
				map.Put("Warning", positions);
			}
			else
			{
				map.KillCells(positions);
			}
			currentFlicker++;
		}
		else
		{
			Finish();
		}
	}

	private void Finish()
	{
		map.Put(placing, positions);
		flicker.Stop();
		QueueFree();
	}
}
