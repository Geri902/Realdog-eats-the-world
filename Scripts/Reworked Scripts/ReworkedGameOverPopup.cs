using Godot;
using System;

public partial class ReworkedGameOverPopup : ColorRect
{
	private Label stats;
	public Button newGameButton;
	public Button mainMenuButton;
	private Timer delay;
	public override void _Ready()
	{
		stats = GetNode<Label>("Stats");
		newGameButton = GetNode<Button>("New Game");
		mainMenuButton = GetNode<Button>("Main Menu");
		delay = GetNode<Timer>("Delay");

		delay.Timeout += DelayedReveal;
	}

	public override void _Process(double delta)
	{
		
	}

	private void DelayedReveal()
	{
		Visible = true;
	}

	public void GetGameOver(int level)
	{
		stats.Text = $"Layer reached: {level}";
		delay.Start();
	}
}
