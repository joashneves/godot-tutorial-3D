using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    public int Speed { get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;
    private Vector3 _targetValocity = Vector3.Zero;

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
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }
        // ground velocity
        _targetValocity.X = direction.X * Speed;
        _targetValocity.Z = direction.Z * Speed;

        if (!IsOnFloor())
        {
            _targetValocity.Y -= FallAcceleration * (float)delta;
        }
        Velocity = _targetValocity;
        MoveAndSlide();
    }

}
