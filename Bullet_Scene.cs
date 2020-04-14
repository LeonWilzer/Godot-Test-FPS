/*
This file is part of Godot-Test-FPS

Godot-Test-FPS is the source code for a game.
Copyright (C) 2020 Leon Wilzer

Godot-Test-FPS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or any later version.

Godot-Test-FPS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

Contact: leon.wilzer@protonmail.com
*/

using Godot;
using System;

public class Bullet_Scene : Spatial
{
    [Export]
    public float Gravity = -5;
    [Export]
    public int _bulletSpeed = 100;
    public int _bulletDamage { get; set; }
    private int _killTimer;
    private float timer;
    private bool _hitSomething;
    private Area _area;
    private float rot;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _bulletDamage = 15;
        _killTimer = 4;
        timer = 0;
        _hitSomething = false;
        _area = GetNode<Area>("Area");
        _area.Connect("body_entered", this, "Collided");
    }
    public override void _PhysicsProcess(float delta)
    {
        // Get direction of the Bullet
        Vector3 forwardDir = GlobalTransform.basis.z;

        // Rotate bullet around the x-axis s it's affected by Gravity. Warning, can overshoot and get into "orbit", make sure that speed and gravity are high enough until a patch is found
        GlobalRotate(GlobalTransform.basis.x.Normalized(), -Gravity*delta);
        //
        GlobalTranslate(forwardDir * _bulletSpeed * delta);

        timer += delta;
        if (timer >= _killTimer)
            QueueFree();
    }

    public void Collided(RigidBody _body)
    {
        if (!_hitSomething)
             HitTest.BulletHit(_body, _bulletDamage, GlobalTransform);
        _hitSomething = true;
        QueueFree();
    }

     public void Collided(KinematicBody _body)
     {
        if (!_hitSomething)
             HitTest.BulletHit(_body, _bulletDamage, GlobalTransform);
        _hitSomething = true;
        QueueFree();
    }

    public void Collided(StaticBody _body)
    {
        if (!_hitSomething)
            HitTest.BulletHit(_body, _bulletDamage, GlobalTransform);
        _hitSomething = true;
        QueueFree();
    }
}