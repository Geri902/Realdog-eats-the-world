using Godot;
using System;

public partial class Clock : Label
{
	private Timer timer;
	public int timeInSeconds = 0;
	public override void _Ready()
	{
		timer = GetNode<Timer>("ClockTimer");
		timer.Timeout += HandleTick;
	}

	private void HandleTick()
	{
		timeInSeconds += 1;
		int seconds = timeInSeconds % 60;
		int minutes = timeInSeconds / 60;
		
		string output = "";

		if (minutes < 10)
		{
			output += 0;
		}
		output += minutes + ":"; 

		if (seconds < 10)
		{
			output += 0;
		}
		output += seconds;

		Text = output;
		
	}

	public override void _Process(double delta)
	{
	}
}
