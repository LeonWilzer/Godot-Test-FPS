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

public static class HitTest
{

    private static int _baseBulletBoost = 9;

    // Called when the node enters the scene tree for the first time.
    public static void BulletHit(object physical, int Damage, Transform BulletGlobalTrans)
    {

        if (physical is RigidBody)
        {
            var _directionVect = BulletGlobalTrans.basis.z.Normalized() * _baseBulletBoost;
            RigidBody rigidbody = (RigidBody)physical;
            rigidbody.ApplyImpulse((BulletGlobalTrans.origin - rigidbody.GlobalTransform.origin).Normalized(), _directionVect * Damage);
            GD.Print("RigidBody hit");
        }
        else if (physical is StaticBody)
            GD.Print("StaticBody hit");
        else if (physical is KinematicBody)
            GD.Print("KinematicBody hit");
    }
}