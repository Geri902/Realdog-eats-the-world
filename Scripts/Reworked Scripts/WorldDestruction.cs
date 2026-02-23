using Godot;
using System;

public partial class WorldDestruction : Node2D
{
	private RandomNumberGenerator rnd = new RandomNumberGenerator();
	private DogController dogController;
	public override void _Ready()
	{
		rnd.Randomize();

		dogController = GetNode<DogController>("DogController");

		dogController.SetUp(rnd);
		dogController.Spawn();
	}

	public override void _Process(double delta)
	{
	}
}
