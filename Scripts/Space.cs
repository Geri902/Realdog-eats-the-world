using Godot;

class Space
{
    public bool eddible;
    public bool destructable = false;
    public Space()
    {
    }
}

class Wall : Space
{

    public Wall(bool destructable)
    {
        eddible = false;
        this.destructable = destructable;
    }
}

class Food : Space
{
    public Food()
    {
        eddible = true;
        destructable = true;
    }
}

class Warning : Space
{
    public Warning()
    {
        eddible = false;
    }
}