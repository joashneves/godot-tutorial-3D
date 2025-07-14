using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    public int Speed { get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;
    [Export]
    public int JumpImpulse { get; set; } = 20;
    [Export]
    public int BounceImpulse { get; set; } = 16;
    private Vector3 _targetVelocity = Vector3.Zero;
    // Don't forget to rebuild the project so the editor knows about the new signal.

    // Emitted when the player was hit by a mob.
    [Signal]
    public delegate void HitEventHandler();

    // ...

    private void Die()
    {
        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnMobDetectorBodyEntered(Node3D body)
    {
        GD.Print("Morreu");
        Die();
    }
    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;
        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }


        if (direction != Vector3.Zero)
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }
        else
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
        }
        var pivot = GetNode<Node3D>("Pivot");
        pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
        // ground velocity
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        if (!IsOnFloor())
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        // Iterate through all collisions that occurred this frame.
        for (int index = 0; index < GetSlideCollisionCount(); index++)
        {
            // We get one of the collisions with the player.
            KinematicCollision3D collision = GetSlideCollision(index);

            // If the collision is with a mob.
            // With C# we leverage typing and pattern-matching
            // instead of checking for the group we created.
            if (collision.GetCollider() is Mob mob)
            {
                // We check that we are hitting it from above.
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    // If so, we squash it and bounce.
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;
                    // Prevent further duplicate calls.
                    break;
                }
            }
        }
        Velocity = _targetVelocity;
        MoveAndSlide();

    }
}
