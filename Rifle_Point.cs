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

public class Rifle_Point : WeaponPoint
{
    private FuncRef _callback;

    // Constructor
    public override void _Ready()
    {
        damage = 15;
        IdleAnimName = "Rifle_idle";
        FireAnimName = "Rifle_fire";
        unequipAnimation = "Rifle_unequip";
        equipAnimation = "Rifle_equip";
        IsWeaponEnabled = false;
        AmmoInWeapon = 50;
        SpareAmmo = 100;
        AmmoInMag = 50;
        CanReload = true;
        CanRefill = true;
        ReloadingAnimName = "Rifle_reload";
        gunFireSound = "Rifle_shot";
        GunCockSound = "Gun_cock";
        MaxAmmo = 250;
    }

    public override void FireWeapon()
    {
        RayCast Ray = GetNode<RayCast>("Ray_Cast");
        Ray.ForceRaycastUpdate();
        AmmoInWeapon--;
        
        if (Ray.IsColliding())
        {
            var _body = Ray.GetCollider();
            
            if (_body != Playernode && _body.HasMethod("BulletHit"))
            {
                _callback = GD.FuncRef(_body, "BulletHit");
                _callback.CallFunc(damage, GlobalTransform);
                //HitTest.BulletHit(_body, damage, Ray.GlobalTransform);
            }
        }

        Playernode.CreateSound(gunFireSound, GlobalTransform.origin);
    }
}
