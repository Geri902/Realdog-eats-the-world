using Godot;
using System;
using System.Collections.Generic;

public partial class TankBossScene : Boss
{
	[Export]
	private PackedScene bulletScene;
	private Sprite2D turret = null;
	private Segment head = null;
	private float bulletSpeed = 700f;
	private bool grow = false;
	private Dictionary<Vector2, int> directionRotations = new Dictionary<Vector2, int>
	{
		{Vector2.Right,0},
		{Vector2.Down,90},
		{Vector2.Left,180},
		{Vector2.Up,270}
	};

    public override void _Ready()
    {
        base._Ready();
		head = gameController.GetHead();
		turret = mainFrames.GetNode<Sprite2D>("Turret");
    }

    public override void _Process(double delta)
    {
        if (grow)
		{
			turret.Scale += new Vector2(0.005f,0.005f);
		}
    }
    public override void Setup()
    {
		mainFrames.Frame = 1;

        delayTimer.Timeout += Shoot;
		
		stepTimer.Timeout += Step;
		stepTimer.Start();
    }

	private void Shoot()
	{
		Bullet bullet = bulletScene.Instantiate<Bullet>();
		AddChild(bullet);
		bullet.SetUp(turret.GlobalPosition, head.GlobalPosition, bulletSpeed, gameController);
		grow = false;
		turret.Scale = Vector2.One;
	}

    protected override void Step()
    {
        int chance = rnd.RandiRange(1,actChance);
		Vector2 direction = GetRandomDirection();
		
		if (true)
		{
			if (chance == 1)
			{
				delayTimer.Start();
				grow = true;
			}
			else
			{
				chance = rnd.RandiRange(1,movementChance);
	
				if (chance == 1)
				{
					mainFrames.RotationDegrees = directionRotations[direction];
					RotateTurret();
					Move(direction);
				}
			}
		}
    }

	public void RotateTurret()
	{
		if (head is not null)
		{
			turret.LookAt(head.GlobalPosition);
		}
	}

    public override void GameStepped()
    {
        if (turret is not null)
		{
			RotateTurret();
		}
    }


}
