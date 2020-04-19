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

public class Knife : Weapon
{
    // Constructor
    public override void _Ready()
    {
        damage = 30;
        IdleAnimName = "Knife_idle";
        FireAnimName = "Knife_fire";
        unequipAnimation = "Knife_unequip";
        equipAnimation = "Knife_equip";
        IsWeaponEnabled = false;
        AmmoInWeapon = 1;
        CanReload = false;
        CanRefill = false;
        ReloadingAnimName = "null";
        gunFireSound = null;
        MaxAmmo = 1;
    }

    public override void FireWeapon()
    {
        Area _area = GetNode<Area>("Area");
        Godot.Collections.Array Bodies = _area.GetOverlappingBodies();

        foreach (PhysicsBody _body in Bodies)
        {
            if (!(_body is RigidBody))
                return;
            _callback = GD.FuncRef(_body, "BulletHit");
            _callback.CallFunc(damage, _area.GlobalTransform);
        }
    }
}