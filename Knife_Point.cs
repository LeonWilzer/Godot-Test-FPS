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

public class Knife_Point : WeaponPoint
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        damage = 30;
        idleAnimName = "Knife_idle";
        fireAnimName = "Knife_fire";
        unequipAnimation = "Knife_unequip";
        equipAnimation = "Knife_equip";
        isWeaponEnabled = false;
    }

    public override void FireWeapon(){
        Area Area = GetNode<Area>("Area");
        var Bodies = Area.GetOverlappingBodies();

        foreach (PhysicsBody Body in Bodies){
            if (Body == playernode)
                return;
            HitTest.BulletHit(Body, damage, Area.GlobalTransform);
        }
    }
}