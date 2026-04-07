using Godot;
using System;

public partial class TankBossScene : Boss
{
    public override void Setup()
    {
		mainFrames.Frame = 1;

        delayTimer.Timeout += Shoot;
		
		stepTimer.Timeout += Step;
		stepTimer.Start();
    }

	private void Shoot()
	{
		
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
				//arrowManager.SetArrow(direction);
				actDirection = direction;
				delayTimer.Start();
			}
			else
			{
				chance = rnd.RandiRange(1,movementChance);
	
				if (chance == 1)
				{
					//need to add rotate logic
					Move(direction);
				}
			}
		}
    }


}
