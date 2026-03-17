using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export]
	private PackedScene gameScene;
	[Export]
	private PackedScene worldDestructionScene;
	[Export]
	private Button normalButton;
	[Export]
	private Button worldDestructionButton;
	private Control panel;
	private Game game = null;
	private WorldDestruction destructionGame = null;
	public override void _Ready()
	{
		normalButton.Pressed += StartNormal;
		worldDestructionButton.Pressed += StartWorldDestruction;
		panel = GetNode<Control>("Panel");
	}

	public override void _Process(double delta)
	{
	}

	private void StartNormal()
	{
		ChangeScene("Normal");
	}

	private void StartWorldDestruction()
	{
		ChangeScene("World Destruction");
	}

	private void ChangeScene(string type)
	{
		if (type == "Normal")
		{
			if (game is null)
			{
				game = gameScene.Instantiate<Game>();
				game.gameMode = type;
				game.mainMenu = this;
				panel.Visible = false;
				GetTree().Root.AddChild(game);
			}
		}
		else
		{
			if (destructionGame is null)
			{
				destructionGame = worldDestructionScene.Instantiate<WorldDestruction>();
				//destructionGame.mainMenu = this;
				panel.Visible = false;
				GetTree().Root.AddChild(destructionGame);
			}
		}
	}

	public void BackToMenu()
	{
		game.QueueFree();
		game = null;
		panel.Visible = true;
	}
}
