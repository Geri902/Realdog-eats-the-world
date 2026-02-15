using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map : TileMapLayer
{
    private const int WallLayerInd = 0;
    private const int NegPalLayerInd = 1;
    private RandomNumberGenerator rnd;
    private Dictionary<Vector2I, Space> spaces = new Dictionary<Vector2I, Space>();
    private Dictionary<string, int> boundry = new Dictionary<string, int>()
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
        FillSpaces();
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

        if (spaces[target] is not null)
        {
            if (spaces[target] is Wall)
            {
                return "Die";
            }
            if (spaces[target].eddible)
            {
                spaces[target] = null;
                EraseCell(target);

                List<Vector2I> copy = new List<Vector2I>(dogParts);
                copy[copy.Count() - 1] = copy[0] + direction;
                /*
                    This might be a bit confusing, so I'm going to leave an explanation:
                    When we generate food we need to take into count the dog's next step, but the only things that "change" are the last and first position (we get +1 for the [first + direction] and the last is removed)
                    So I change the last to the future next position, the foreach doesn't care that the new head is at the position of the tail
                */

                GenerateFood(dogParts);
                return "Eat";
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

    private void FillSpaces()
    {
        for (int y = boundry["minY"] + 1; y < boundry["maxY"]; y++)
        {
            for (int x = boundry["minX"] + 1; x < boundry["maxX"]; x++)
            {
                Vector2I newPos = new Vector2I(x,y);
                if (!spaces.ContainsKey(newPos))
                {
                    spaces.Add(newPos,null);
                }
            }
        }
    }

    public void GenerateFood(List<Vector2I> dogParts)
    {
        Vector2I position = GetRandomNonDogPosition(dogParts);
        spaces[position] = new Food();
        SetCell(position, NegPalLayerInd, new Vector2I(0,0));
    }

    private Vector2I GetRandomPosition()
    {
        List<KeyValuePair<Vector2I,Space>> emptySpaces = spaces.Where(x=>x.Value is null).ToList();

        int randIndex = rnd.RandiRange(0, emptySpaces.Count - 1);

        return emptySpaces[randIndex].Key;
    }

    private Vector2I GetRandomNonDogPosition(List<Vector2I> dogParts)
    {
        Dictionary<Vector2I, Space> nonDog = new Dictionary<Vector2I, Space>(spaces);

        foreach (Vector2I pos in dogParts)
        {
            nonDog.Remove(pos);
        }

        List<KeyValuePair<Vector2I,Space>> emptySpaces = nonDog.Where(x=>x.Value is null).ToList();

        int randIndex = rnd.RandiRange(0, emptySpaces.Count - 1);

        return emptySpaces[randIndex].Key;
    }
}
