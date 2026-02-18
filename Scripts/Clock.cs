using Godot;
using System;

public partial class Clock : Label
{
	public Timer timer;
	public int timeInSeconds = 0;
	public override void _Ready()
	{
		timer = GetNode<Timer>("ClockTimer");
		timer.Timeout += HandleTick;
	}

	private void HandleTick()
	{
		timeInSeconds += 1;
		Text = GetTimeText();
		
	}

	public void RestartClock()
	{
		timeInSeconds = 0;
		Text = GetTimeText();
		timer.Start();
	}

	public string GetTimeText()
	{
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

		return output;
	}

	public override void _Process(double delta)
	{
	}
}
