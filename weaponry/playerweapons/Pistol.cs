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

public class Pistol : Weapon
{
    private PackedScene _bulletScene;

    // Constructor
    public override void _Ready()
    {
        damage = 5;
        IdleAnimName = "Pistol_idle";
        FireAnimName = "Pistol_fire";
        unequipAnimation = "Pistol_unequip";
        equipAnimation = "Pistol_equip";
        AmmoInWeapon = 10;
        SpareAmmo = 20;
        AmmoInMag = 10;
        IsWeaponEnabled = false;
        CanReload = true;
        CanRefill = true;
        ReloadingAnimName = "Pistol_reload";
        _bulletScene = ResourceLoader.Load<PackedScene>("res://weaponry/bullets/standard/StandardBullet.tscn");
        gunFireSound = "Pistol_shot";
        GunCockSound = "Gun_cock";
        MaxAmmo = 50;
    }

    public override void FireWeapon()
    {
        StandardBullet _clone = (StandardBullet)_bulletScene.Instance();
        Node _sceneRoot = GetTree().Root.GetChild(0);
        _sceneRoot.AddChild(_clone);

        _clone.GlobalTransform = GlobalTransform;
        _clone.Scale = new Vector3(4, 4, 4);
        _clone.BulletDamage = damage;
        AmmoInWeapon--;
        Playernode.CreateSound(gunFireSound);

    }
}