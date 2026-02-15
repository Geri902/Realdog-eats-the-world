using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map : TileMapLayer
{
    private RandomNumberGenerator rnd;
    private Dictionary<Vector2I, Space> spaces = new Dictionary<Vector2I, Space>();
    public Dictionary<string, int> boundry = new Dictionary<string, int>()
    {
        {"minX",0},
        {"maxX",0},
        {"minY",0},
        {"maxY",0},
        
    };

    public override void _Ready()
    {
        List<Vector2I> walls = GetUsedCells().ToList();

        foreach (Vector2I wall in walls)
        {
            spaces.Add(wall, new Wall());
            SetBoundry(wall);
        }
        CorrectBoundries();
    }

    public void SetRnd(RandomNumberGenerator rnd)
    {
        this.rnd = rnd;
        
    }

    public string TryMove(List<Vector2I> dogParts, Vector2I direction)
    {
        Vector2I target = dogParts[0] + direction;

        foreach (Vector2I segment in dogParts)
        {
            if (segment == target)
            {
                return "Die";
            }
        }

        foreach (var space in spaces)
        {
            if (space.Key == target)
            {
                if (space.Value is Wall)
                {
                    return "Die";
                }
                if (space.Value.eddible)
                {
                    return "Eat";
                }
                break;
            }
        }

        return "Move";
    }

    private void SetBoundry(Vector2I wall)
    {
        int x = wall.X;
        int y = wall.Y;

        if (boundry["minX"] > x)
        {
            boundry["minX"] = x;
        }
        else if (boundry["maxX"] < x)
        {
            boundry["maxX"] = x;
        }

        if (boundry["minY"] > y)
        {
            boundry["minY"] = y;
        }
        else if (boundry["maxY"] < y)
        {
            boundry["maxY"] = y;
        }
    }

    private void CorrectBoundries()
    {
        if (boundry["minX"] < 0)
        {
            boundry["minX"] += 1;
        }
        else
        {
            boundry["minX"] -= 1;
        }

        if (boundry["maxX"] < 0)
        {
            boundry["maxX"] += 1;
        }
        else
        {
            boundry["maxX"] -= 1;
        }

        if (boundry["minY"] < 0)
        {
            boundry["minY"] += 1;
        }
        else
        {
            boundry["minY"] -= 1;
        }

        if (boundry["maxY"] < 0)
        {
            boundry["maxY"] += 1;
        }
        else
        {
            boundry["maxY"] -= 1;
        }
    }
}
