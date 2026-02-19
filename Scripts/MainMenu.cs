using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export]
	private PackedScene gameScene;
	[Export]
	private Button normalButton;
	[Export]
	private Button worldDestructionButton;
	private Control panel;
	private Game game = null;
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
		if (game is null)
		{
			game = gameScene.Instantiate<Game>();
			game.gameMode = type;
			game.mainMenu = this;
			panel.Visible = false;
			GetTree().Root.AddChild(game);
		}
	}

	public void BackToMenu()
	{
		game.QueueFree();
		game = null;
		panel.Visible = true;
	}
}
