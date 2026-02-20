using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map : TileMapLayer
{
    private const int WallLayerInd = 0;
    private const int WarningLayerInd = 3;
    private const int AreaSize = 128;
    [Export]
    private PackedScene foodScene;
    [Export]
    private PackedScene flickererScene;
    private RandomNumberGenerator rnd;
    public Timer wallSpawnTimer;
    private Game game;
    private Dictionary<Vector2I, FoodScene> foods = new Dictionary<Vector2I, FoodScene>();
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
        wallSpawnTimer = GetNode<Timer>("Wall Spawn Timer");

        List<Vector2I> walls = GetUsedCells().ToList();

        foreach (Vector2I wall in walls)
        {
            spaces.Add(wall, new Wall());
            SetBoundry(wall);
        }
        FillSpaces();

        wallSpawnTimer.Timeout += PlaceWall;
    }

    public void ResetMap()
    {
        for (int y = boundry["minY"] + 1; y < boundry["maxY"]; y++)
        {
            for (int x = boundry["minX"] + 1; x < boundry["maxX"]; x++)
            {
                Vector2I pos = new Vector2I(x,y);
                spaces[pos] = null;
                EraseCell(pos);
            }
        }

        foreach (KeyValuePair<Vector2I,FoodScene> food in foods)
        {
            food.Value.QueueFree();
        }
        foods.Clear();

        Godot.Collections.Array<Vector2I> walls = GetUsedCells();
        Put("Wall", walls);
        wallSpawnTimer.Start();
    }

    public void SetMap(RandomNumberGenerator rnd, Game game)
    {
        this.rnd = rnd;
        this.game = game;
        
    }

    public string TryMove(Vector2I direction)
    {
        List<Vector2I> dogParts = game.GetDogParts();
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

                List<Vector2I> copy = new List<Vector2I>(dogParts);
                copy[copy.Count() - 1] = copy[0] + direction;
                /*
                    This might be a bit confusing, so I'm going to leave an explanation:
                    When we generate food we need to take into count the dog's next step, but the only things that "change" are the last and first position (we get +1 for the [first + direction] and the last is removed)
                    So I change the last to the future next position, the foreach doesn't care that the new head is at the position of the tail
                */

                MoveFood(target);
                ScareFoods(dogParts[0]);
                return "Eat";
            }
        }

        ScareFoods(dogParts[0]);
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

    public void MoveFood(Vector2I foodPosition)
    {
        if (!IsThereEnoughCells(1))
        {
            foods[foodPosition].QueueFree();
            return;
        }

        Vector2I position = GetRandomNonDogPosition();
        spaces[position] = new Food();

        FoodScene food = foods[foodPosition];
        foods.Remove(foodPosition);
        foods.Add(position, food);

        food.GlobalPosition = position * AreaSize;
        food.CalmDown();

    }

    public void GenerateFood()
    {
        Vector2I position = GetRandomNonDogPosition();
        spaces[position] = new Food();
        FoodScene food = foodScene.Instantiate<FoodScene>();
        food.GlobalPosition = position * AreaSize;
        AddChild(food);
        food.SetRnd(rnd);
        foods.Add(position, food);
    }

    private Vector2I GetRandomPosition()
    {
        List<KeyValuePair<Vector2I,Space>> emptySpaces = spaces.Where(x=>x.Value is null).ToList();

        int randIndex = rnd.RandiRange(0, emptySpaces.Count - 1);

        return emptySpaces[randIndex].Key;
    }

    private Vector2I GetRandomNonDogPosition()
    {
        Dictionary<Vector2I, Space> nonDog = new Dictionary<Vector2I, Space>(spaces);

        foreach (Vector2I pos in game.GetDogParts())
        {
            nonDog.Remove(pos);
        }

        List<KeyValuePair<Vector2I,Space>> emptySpaces = nonDog.Where(x=>x.Value is null).ToList();

        int randIndex = rnd.RandiRange(0, emptySpaces.Count - 1);

        return emptySpaces[randIndex].Key;
    }

    public bool IsNextFood(Vector2I headPosition)
    {
        Vector2I[] around = new Vector2I[4]{ Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right};

        foreach (Vector2I nextStep in around)
        {
            Vector2I target = headPosition + nextStep;
        if (spaces[target] is not null)
        {
            if (spaces[target].eddible)
            {
                return true;
            }
        }
        }
        return false;
    }

    private void ScareFoods(Vector2I head)
    {
        foreach (KeyValuePair<Vector2I, FoodScene> food in foods)
        {
            int panicRange = food.Value.panicRange;
            Vector2I foodPosition = food.Key;

            if (Mathf.Abs(foodPosition.X - head.X) <= panicRange && Mathf.Abs(foodPosition.Y - head.Y) <= panicRange)
            {
                if (!food.Value.isPanicing)
                {
                    food.Value.Panic();
                }
            }
            else
            {
                if (food.Value.isPanicing)
                {
                    food.Value.CalmDown();
                }
            }
        }
    }

    public void Put(string what, Godot.Collections.Array<Vector2I> where)
    {
        /*
            Type: setInd, terrainInd
            warning: 0, 1
            wall: 0, 0
        */
        switch (what)
        {
            case "Warning":
                SetCellsTerrainConnect(where, 0, 1, false);
                break;
            case "Wall":
                SetCellsTerrainConnect(where, 0, 0, false);
                foreach (Vector2I wall in where)
                {
                    spaces[wall] = new Wall();

                }
                List<Vector2I> dogParts = game.GetDogParts();
                if (dogParts.Any(where.Contains))
                {
                    game.Die();
                    return;
                }
                break;
            default:
                break;
        }
    }

    public void KillCells(Godot.Collections.Array<Vector2I> where)
    {
        foreach (Vector2I position in where)
        {
            EraseCell(position);
        }
    }

    private void PlaceWall()
    {
        //later can write logic to set multiple walls at once, like a 3x1 wall
        int wallCount = 1;

        if (!IsThereEnoughCells(wallCount))
        {
            return;
        }

        Godot.Collections.Array<Vector2I> walls = new Godot.Collections.Array<Vector2I>();

        for (int i = 0; i < wallCount; i++)
        {
            Vector2I position = GetRandomPosition();
            spaces[position] = new Warning();
            walls.Add(position);
        }

        Flickerer flickerer = flickererScene.Instantiate<Flickerer>();
        AddChild(flickerer);
        flickerer.StartFlicker(walls, this, "Wall", 3);
    }

    private bool IsThereEnoughCells(int count)
    {
        Dictionary<Vector2I, Space> nonDog = new Dictionary<Vector2I, Space>(spaces);

        foreach (Vector2I pos in game.GetDogParts())
        {
            nonDog.Remove(pos);
        }

        int emptyCount = nonDog.Count(x=>x.Value is null);

        return emptyCount >= count;
    }
}
