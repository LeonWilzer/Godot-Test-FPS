/*
This file is part of Godot-Test-FPS

Godot-Test-FPS is the source code for a game.
Copyright (C) 2020 Leon Wilzer <leon.wilzer@protonmail.com>

Godot-Test-FPS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or any later version.

Godot-Test-FPS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

using Godot;
using System;

public class Target : StaticBody
{
    private int _targetHealth;
    private int _currentHealth;

    private Spatial _brokenTargetHolder;

    // The collision shape for the target.
    // NOTE: this is for the whole target, not the pieces of the target.
    private CollisionShape _targetCollisionShape;

    private float _targetRespawnTime;
    private float _targetRespawnTimer;

    public PackedScene _destroyedTarget;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _targetHealth = 40;
        _currentHealth = 40;
        _targetRespawnTime = 14;
        _targetRespawnTime = 0;
        _brokenTargetHolder = GetParent().GetNode<Spatial>("Broken_Target_Holder");
        _targetCollisionShape = GetNode<CollisionShape>("Collision_Shape");
        _destroyedTarget = ResourceLoader.Load<PackedScene>("res://Broken_Target.tscn");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_targetRespawnTimer > 0)
        {
            _targetRespawnTimer -= delta;

            if (_targetRespawnTimer <= 0)
            {
                foreach (RigidBody _child in _brokenTargetHolder.GetChildren())
                    _child.QueueFree();

                _targetCollisionShape.Disabled = false;
                Visible = true;
                _currentHealth = _targetHealth;
            }
        }
    }

    public void BulletHit(int _damage, Transform _bulletTransform)
    {
        GD.Print("TargetHit");
        _currentHealth -= _damage;
        if (_currentHealth <= 0)
        {
            Node _clone = _destroyedTarget.Instance();
            _brokenTargetHolder.AddChild(_clone);

            foreach ( RigidBody _rigid in _clone.GetChildren())
            {
                Vector3 _centerInRigidSpace = _brokenTargetHolder.GlobalTransform.origin - _rigid.GlobalTransform.origin;
                var _direction = (_rigid.Transform.origin - _centerInRigidSpace).Normalized();
                // Apply the impulse with some additional force (I find 12 works nicely).
                _rigid.ApplyImpulse(_centerInRigidSpace, _direction * 12 * _damage);
            }

            _targetRespawnTimer = _targetRespawnTime;

            _targetCollisionShape.Disabled = true;
            Visible = false;
        }
    }

}
