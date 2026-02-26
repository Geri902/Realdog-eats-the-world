using Godot;
using System;
using System.Collections.Generic;

public partial class DogController : Node2D
{
	private const int size = 128;
	[Export]
	private PackedScene segmentScene;
	private List<Segment> parts = new List<Segment>();
	private RandomNumberGenerator rnd;
	private BodyType headState = BodyType.HeadNormal;
	private WorldDestruction gameScene;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void SetUp(RandomNumberGenerator rnd, WorldDestruction gameScene)
	{
		this.rnd = rnd;
		this.gameScene = gameScene;
	}

	public void Reset()
	{
		foreach (Segment segment in parts)
		{
			segment.QueueFree();
		}
		parts.Clear();
	}

	public Vector2 Spawn()
	{
		AddPart(Vector2.Zero);
		parts[0].isHead = true;

		Vector2 nextPosition;
		int dir = rnd.RandiRange(0,3);

		switch (dir)
		{
			case 0:
				nextPosition = Vector2.Down * size;
				break;
			case 1:
				nextPosition = Vector2.Up * size;
				break;
			case 2:
				nextPosition = Vector2.Right * size;
				break;
			default:
				nextPosition = Vector2.Left * size;
				break;
		}

		AddPart(nextPosition);
		AddPart(nextPosition * 2);
		AddPart(nextPosition * 3);
		AddPart(nextPosition * 4);

		DrawAll();

		return nextPosition / -size;
	}

	private void AddPart(Vector2 position)
	{
		Segment part = segmentScene.Instantiate<Segment>();
		part.GlobalPosition = position;
		parts.Add(part);
		AddChild(part);
	}

	private void DrawAll()
	{
		int lastInd = parts.Count - 1;

		parts[0].DrawSegment(Vector2.Zero, parts[1].GlobalPosition, headState);
		for (int i = 1; i < lastInd; i++)
		{
			parts[i].DrawSegment(parts[i - 1].GlobalPosition, parts[i + 1].GlobalPosition, BodyType.Straight);
		}
		parts[lastInd].DrawSegment(parts[lastInd - 1].GlobalPosition, Vector2.Zero, BodyType.Tail);
	}

	public void Move(Vector2 direction)
	{
		Vector2 prev = parts[0].Position;
		string response = parts[0].MoveSegment(direction * size);

		switch (response)
		{
			case "Moved":
				MoveRest(prev);
				break;
			case "Die":
				Explode();
				gameScene.GameOver();	
				break;
			default:
				GD.Print(response);
				break;
		}

		DrawAll();		
	}

	private void MoveRest(Vector2 previous)
	{
		Vector2 prev = previous;
		for (int i = 1; i < parts.Count; i++)
		{
			Vector2 current = parts[i].Position;
			parts[i].Position = prev;
			prev = current;
		}
	}

	private void Explode()
	{
		float delay = 0.05f;
		float increment = 0.05f;
		foreach (Segment part in parts)
		{
			part.StartExplosion(delay);
			delay += increment;
		}
	}

	public List<Vector2> GetDogPositions()
	{
		List<Vector2> dogPositions = new List<Vector2>();

		foreach (Segment part in parts)
		{
			dogPositions.Add(part.GlobalPosition);
		}

		return dogPositions;
	}
}
