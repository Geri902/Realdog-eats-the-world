using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export]
	private PackedScene gameScene;
	[Export]
	private PackedScene worldDestructionScene;
	[Export]
	private Label titleLabel;
	[Export]
	private Button normalButton;
	[Export]
	private Button worldDestructionButton;
	[Export]
	private VideoStreamPlayer introVideo;
	private Control panel;
	private Game game = null;
	private WorldDestruction destructionGame = null;
	private double videoLength = 0.0;
	private bool done = false;

	public override void _Ready()
	{
		normalButton.Pressed += StartNormal;
		worldDestructionButton.Pressed += StartWorldDestruction;
		panel = GetNode<Control>("Panel");

		introVideo.Finished += EnableUI;

		videoLength = Math.Floor(introVideo.GetStreamLength());
	}

	public override void _Process(double delta)
	{
		if(Input.IsAnythingPressed()){
			if (Input.IsActionJustPressed("Fire") && done == false)
			{
				introVideo.StreamPosition = videoLength;
				done = true;
			}
		}
	}

	private void EnableUI()
	{
		titleLabel.Visible = true;
		normalButton.Visible = true;
		worldDestructionButton.Visible = true;
		done = true;
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
				destructionGame.mainMenu = this;
				panel.Visible = false;
				GetTree().Root.AddChild(destructionGame);
			}
		}
	}

	public void BackToMenu()
	{
		if (game is not null)
		{
			game.QueueFree();
			game = null;
		}
		if (destructionGame is not null)
		{
			destructionGame.QueueFree();
			destructionGame = null;
		}
		panel.Visible = true;
	}
}
