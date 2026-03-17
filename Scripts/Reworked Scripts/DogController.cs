using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DogController : Node2D
{
	private const int size = 128;
	[Export]
	private PackedScene segmentScene;
	public List<Segment> parts = new List<Segment>();
	public List<Segment> removedParts = new List<Segment>();
	private RandomNumberGenerator rnd;
	private BodyType headState = BodyType.HeadNormal;
	private WorldDestruction gameScene;
	private int dashLength = 3;
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

		foreach (Segment segment in removedParts)
		{
			segment.QueueFree();
		}
		removedParts.Clear();
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

		DrawAll();

		return nextPosition / -size;
	}

	private void AddPart(Vector2 position)
	{
		Segment part = segmentScene.Instantiate<Segment>();
		part.GlobalPosition = position;
		part.SetUp(this);
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

	public bool Move(Vector2 direction)
	{
		Vector2 prev = parts[0].Position;
		string response = parts[0].MoveSegment(direction * size);

		switch (response)
		{
			case "Eat":
				MoveRest(prev);
				parts[0].isFull = true;
				break;
			case "Reattach":
				gameScene.SpawnBoss();
				break;
			case "Moved":
				MoveRest(prev);
				break;
			case "Die":
				Die();
				return false;
			default:
				GD.Print(response);
				break;
		}

		DrawAll();
		return true;		
	}

	public void Die()
	{
		Explode();
		gameScene.isGameOver = true;
		gameScene.GameOver();
	}

	public void MoveRest(Vector2 previous, Segment reattatched = null)
	{
		Vector2 prev = previous;
		for (int i = 1; i < parts.Count; i++)
		{
			Vector2 current = parts[i].Position;
			parts[i].Position = prev;
			prev = current;
		}

		if (reattatched is not null)
		{
			Reattach(reattatched, prev);
		}
		Digest(prev);
	}

	private void Digest(Vector2 prev)
	{
		int lastInd = parts.Count - 1;

		if (parts[lastInd].isFull)
		{
			parts[lastInd].isFull = false;
			AddPart(prev);
			gameScene.SpawnBoss();
		}

		for (int i = lastInd; i > 0; i--)
		{
			if (parts[i - 1].isFull)
			{
				parts[i].isFull = true;
				parts[i - 1].isFull = false;
			}
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
		foreach (Segment part in removedParts)
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

	private void SplitAt(Segment at)
	{
		int ind = parts.IndexOf(at);
		int length = parts.Count - ind;
		List<Segment> split = parts.Slice(ind, length);

		foreach (Segment part in split)
		{
			part.isAttatched = false;
			part.isFull = false;
			part.ClearFullness();
		}

		parts.RemoveRange(ind, length);
		removedParts.AddRange(split);

		DrawAll();
	}

	public void HitAt(Segment at)
	{
		if (parts.IndexOf(at) < 2)
		{
			Die();
		}
		else
		{
			SplitAt(at);
			at.StartExplosion(0.05f);
		}
	}

	public bool Dash(Vector2 direction)
	{
		if (parts.Count > 2)
		{
			SplitAt(parts[2]);
			gameScene.SpawnBoss();

			for (int i = 0; i < dashLength; i++)
			{
				bool couldMove = Move(direction);
				if (!couldMove)
				{
					break;
				}
			}
			return true;
		}
		return false;
	}

    internal void Reattach(Segment segment, Vector2 prev)
    {
        parts.Add(segment);
		segment.GlobalPosition = prev;
		segment.isAttatched = true;
		removedParts.Remove(segment);
    }

    public void LoadBeam()
    {
        if (parts.Count < 3)
		{
			GD.Print("Bug: can't fire (not long enough)");
			return;
		}

		Segment toRemove = parts[parts.Count - 1];
		parts.Remove(toRemove);
		toRemove.QueueFree();
		DrawAll();
		gameScene.SpawnBoss();
    }
}
