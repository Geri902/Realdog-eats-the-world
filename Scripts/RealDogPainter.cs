using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class RealDogPainter : TileMapLayer
{
	private const int tileSetInd = 0;
	private static Dictionary<string,Vector2I> partFromName = new Dictionary<string, Vector2I>()
	{
		{"Tail", new Vector2I(0,0)},
		{"Bend", new Vector2I(0,1)},
		{"Body", new Vector2I(0,2)},
		{"Head", new Vector2I(0,3)},
	};

	private List<Vector2I> partPlaces = new List<Vector2I>();

	//testing
	List<Vector2I> testShape = new List<Vector2I>
	{
		new Vector2I(0, 0),
		new Vector2I(1, 0),
		new Vector2I(2, 0),
		new Vector2I(3, 0),
		new Vector2I(0, 1),
		new Vector2I(0, 2),
		new Vector2I(0, 3),
		new Vector2I(0, 4),
		new Vector2I(1, 4),
		new Vector2I(2, 4),
		new Vector2I(3, 4),
		new Vector2I(2, 2),
		new Vector2I(3, 2),
		new Vector2I(3, 3),
	};

	private int partCount = 0;
	public override void _Ready()
	{
		partPlaces = testShape;
		DrawDog();
	}

	public override void _Process(double delta)
	{
	}

	public void AddPart(Vector2I where)
	{
		partPlaces.Add(where);
		partCount++;
	}

	public void DrawDog()
	{
		int lastPartInd = partCount - 1;

		Clear();
		DrawPart(0,"Head");
		for (int i = 1; i < lastPartInd; i++)
		{
			string what = "Body";
			if (IsBend(i))
			{
				what = "Bend";
			}
			DrawPart(i,what);
		}
		DrawPart(lastPartInd,"Tail");

	}

	private void DrawPart(int which, string what)
	{
		Vector2I part = partPlaces[which];
		SetCell(
			part,
			sourceId: tileSetInd, 
			atlasCoords: partFromName[what],
			alternativeTile: 0
		);
		RotatePart(part, which, what);
	}

	private bool IsBend(int which)
	{
		Vector2I current = partPlaces[which];
		Vector2I diff1 = partPlaces[which - 1] - current;
		Vector2I diff2 = partPlaces[which + 1] - current;

		return diff1.X == diff2.X || diff1.Y == diff2.Y;
	}
	private int CalcDeg(int which, string what)
	{
		Vector2I current = partPlaces[which];
		Vector2I prev;
		Vector2I next;
		Vector2I diff1;
		Vector2I diff2;

		int deg = 0;

		switch (what)
		{
			case "Head":
				next = partPlaces[which + 1];
				diff1 = next - current;

				if (diff1 == Vector2I.Down)
				{
					deg = 90;
				}
				else if (diff1 == Vector2I.Left)
				{
					deg = 180;
				}
				else if (diff1 == Vector2I.Up)
				{
					deg = 270;
				}
				break;
			case "Tail":
				prev = partPlaces[which - 1];
				diff1 = prev - current;

				if (diff1 == Vector2I.Up)
				{
					deg = 90;
				}
				else if (diff1 == Vector2I.Right)
				{
					deg = 180;
				}
				else if (diff1 == Vector2I.Down)
				{
					deg = 270;
				}
				break;	
			default:
				prev = partPlaces[which - 1];
				next = partPlaces[which + 1];
				diff1 = next - current;
				diff2 = prev - current;

				if (IsBend(which))
				{
					if ((diff2 == Vector2I.Left && diff1 == Vector2I.Up) || (diff1 == Vector2I.Left && diff2 == Vector2I.Up))
					{
						deg = 90;
					}
					else if ((diff2 == Vector2I.Right && diff1 == Vector2I.Up) || (diff1 == Vector2I.Right && diff2 == Vector2I.Up))
					{
						deg = 180;
					}
					else if ((diff2 == Vector2I.Right && diff1 == Vector2I.Down) || (diff1 == Vector2I.Right && diff2 == Vector2I.Down))
					{
						deg = 270;
					}
				}
				else
				{
					if (diff1 == Vector2I.Left){
						deg = 180;
					}

					else if (diff1 == Vector2I.Up){
						deg = 270;
					}

					else if (diff1 == Vector2I.Down){
						deg = 90;
					}
				}
				break;
		}

		return deg;
	}

	private void RotatePart(Vector2I part, int which, string what)
	{
		int rotation = 0;
		switch (CalcDeg(which, what))
		{
			case 90:
				rotation = (int)TileSetAtlasSource.TransformTranspose | (int)TileSetAtlasSource.TransformFlipH;
				break;
			case 180:
				rotation = (int)TileSetAtlasSource.TransformFlipH | (int)TileSetAtlasSource.TransformFlipV;
				break;
			case 270:
				rotation = (int)TileSetAtlasSource.TransformTranspose | (int)TileSetAtlasSource.TransformFlipV;
				break;
			default:
				break;
		}
		SetCell(
			part,
			sourceId: tileSetInd, 
			atlasCoords: partFromName[what],
			alternativeTile: rotation
		);
	}
}
