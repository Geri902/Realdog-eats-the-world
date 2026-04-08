using Godot;
using System;

public partial class TankBossScene : Boss
{
	[Export]
	private PackedScene bulletScene;
	private Sprite2D turret = null;
	private Segment head = null;
	private float bulletSpeed = 500f;
	private bool grow = false;

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
			turret.Scale += new Vector2(0.001f,0.001f);
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
		bullet.SetTarget(turret.GlobalPosition, head.GlobalPosition, bulletSpeed);
		grow = false;
		turret.Scale = Vector2.One;
	}

    public override void Die()
    {
        base.Die();

		//add explosions + clean up head
    }

    protected override void Step()
    {
        int chance = rnd.RandiRange(1,actChance);
		Vector2 direction = GetRandomDirection(); //get cannon direction?
		
		if (true)
		//prev arrowmanager, de szerintem ide nem kell
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
					//need to add logic to rotate based on where it's moving
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
