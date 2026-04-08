using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
	public float speed = 0.0f;
	public Vector2 direction = Vector2.Zero;

	public void SetTarget(Vector2 start, Vector2 target, float speed)
	{
		GlobalPosition = start;
		direction = (target - start).Normalized();
		LookAt(target);
		this.speed = speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (direction != Vector2.Zero)
		{
			Velocity = direction * speed;
        	KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

			if (collision != null)
			{
				GodotObject collider = collision.GetCollider();

				if (collider is Segment segment)
				{
					segment.Hit();
				}
				if (collider is TileMapLayer)
				{
					Die();
				}
				if (collider is GroundObstacle obstacle)
				{
					obstacle.Die();
				}
				
			}
		}
	}

	private void Die()
	{
		QueueFree();
	}
}
