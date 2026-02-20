using Godot;

class Space
{
    public bool eddible;
    public Space()
    {
    }
}

class Wall : Space
{
    public Wall()
    {
        eddible = false;
    }
}

class Food : Space
{
    public Food()
    {
        eddible = true;
    }
}

class Warning : Space
{
    public Warning()
    {
        eddible = false;
    }
}