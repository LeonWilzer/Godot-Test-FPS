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

public class StandardBullet : Spatial
{
    [Export]
    public float Gravity = -1;
    [Export]
    public int BulletSpeed = 250;
    public int BulletDamage { get; set; }
    private int _killTimer;
    private float timer;
    private bool _hitSomething;
    private Area _area;
    private FuncRef _callback;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BulletDamage = 15;
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

        // Rotate bullet around the x-axis s it's affected by Gravity. Warning, can overshoot and get into "orbit", make sure that speed is high and that gravity is low enough until a patch is found
        GlobalRotate(GlobalTransform.basis.x.Normalized(), -Gravity*delta);
        GlobalTranslate(forwardDir * BulletSpeed * delta);

        timer += delta;
        if (timer >= _killTimer)
            QueueFree();
    }

    public void Collided(PhysicsBody _body)
    {
        if (!_hitSomething && _body.HasMethod("BulletHit"))
        {
            _callback = GD.FuncRef(_body, "BulletHit");
            _callback.CallFunc(BulletDamage, GlobalTransform);
        }
        _hitSomething = true;
        QueueFree();
    }
}