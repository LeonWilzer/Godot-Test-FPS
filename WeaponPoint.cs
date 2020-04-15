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

public abstract class WeaponPoint : Spatial
{
    // Weapon properties
    protected int damage;

    // Asset names
    public string idleAnimName { get; protected set; }
    public string fireAnimName { get; protected set; }
    protected string unequipAnimation;
    protected string equipAnimation;
    public string reloadingAnimName { get; protected set; }
    public string gunCockSound { get; protected set; }
    protected string gunFireSound;

    // Variables
    public bool isWeaponEnabled { get; protected set; }
    public int ammoInWeapon { get; protected set; }
    public int spareAmmo { get; protected set; }
    protected int ammoInMag;
    public bool canReload { get; protected set; }
    protected bool canRefill;

    // Nodes
    public Player playernode { protected get;  set; }

    // Constructor
    public override void _Ready()
    {
        gunCockSound = "Gun_cock";
    }

    public abstract void FireWeapon();
    public bool UnequipWeapon()
    {
        if ( playernode.AnimationPlayer.CurrentState == idleAnimName)
            if (playernode.AnimationPlayer.CurrentState != unequipAnimation)
                playernode.AnimationPlayer.SetAnimation(unequipAnimation);

    
        if (playernode.AnimationPlayer.CurrentState == "Idle_unarmed")
        {
            isWeaponEnabled = false;
            return true;
        }
        return false;
    }

    public bool EquipWeapon()
    {
        if ( playernode.AnimationPlayer.CurrentState == idleAnimName)
        {
            isWeaponEnabled = true;
            return true;
        }

        if (playernode.AnimationPlayer.CurrentState == "Idle_unarmed")
            playernode.AnimationPlayer.SetAnimation(equipAnimation);
        return false;
    }

    public bool ReloadWeapon()
    {
        bool canReload = false;

        if (playernode.AnimationPlayer.CurrentState == idleAnimName)
            canReload = true;
        if (spareAmmo <= 0 || ammoInWeapon == ammoInMag)
            canReload = false;
            
        if (canReload)
        {
            int _ammoNeeded = ammoInMag - ammoInWeapon;

            if (spareAmmo >= _ammoNeeded)
            {
                spareAmmo -= _ammoNeeded;
                ammoInWeapon = ammoInMag;
            }
            else
            {
                ammoInWeapon += spareAmmo;
                spareAmmo = 0;
            }

            playernode.AnimationPlayer.SetAnimation(reloadingAnimName);
            //playernode.CreateSound(gunCockSound, playernode.Camera.GlobalTransform.origin);
            return true;
        }
        return false;
    }
}