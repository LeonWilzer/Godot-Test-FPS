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

public class Rifle_Point : WeaponPoint
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        damage = 15;
        idleAnimName = "Rifle_idle";
        fireAnimName = "Rifle_fire";
        unequipAnimation = "Rifle_unequip";
        equipAnimation = "Rifle_equip";
        isWeaponEnabled = false;
    }

    public override void FireWeapon(){
        RayCast Ray = GetNode<RayCast>("Ray_Cast");
        Ray.ForceRaycastUpdate();

        if (Ray.IsColliding()){
            var body = Ray.GetCollider();
            
            if (body != playernode)
                HitTest.BulletHit(body, damage, Ray.GlobalTransform);
        }
    }
}
