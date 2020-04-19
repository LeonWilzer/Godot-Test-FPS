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

public abstract class Weapon : Spatial
{
    // Weapon properties
    protected int damage;
    public int AmmoInMag { get; protected set; }
    public int MaxAmmo { get; protected set; }


    // Asset names
    public string IdleAnimName { get; protected set; }
    public string FireAnimName { get; protected set; }
    protected string unequipAnimation;
    protected string equipAnimation;
    public string ReloadingAnimName { get; protected set; }
    public string GunCockSound { get; protected set; }
    protected string gunFireSound;

    // Variables
    public bool IsWeaponEnabled { get; protected set; }
    public int AmmoInWeapon { get; protected set; }
    public int SpareAmmo { get; set; }
    public bool CanReload { get; protected set; }
    public bool CanRefill { get; protected set; }
    protected FuncRef _callback;

    // Nodes
    public Player Playernode { protected get;  set; }

    // Constructor
    public override void _Ready()
    {
        GunCockSound = "Gun_cock";
    }

    public abstract void FireWeapon();
    public bool UnequipWeapon()
    {
        if ( Playernode.AnimationPlayer.CurrentState == IdleAnimName)
            if (Playernode.AnimationPlayer.CurrentState != unequipAnimation)
                Playernode.AnimationPlayer.SetAnimation(unequipAnimation);

    
        if (Playernode.AnimationPlayer.CurrentState == "Idle_unarmed")
        {
            IsWeaponEnabled = false;
            return true;
        }
        return false;
    }

    public bool EquipWeapon()
    {
        if ( Playernode.AnimationPlayer.CurrentState == IdleAnimName)
        {
            IsWeaponEnabled = true;
            return true;
        }

        if (Playernode.AnimationPlayer.CurrentState == "Idle_unarmed")
            Playernode.AnimationPlayer.SetAnimation(equipAnimation);
        return false;
    }

    public bool ReloadWeapon()
    {
        bool CanReload = false;

        if (Playernode.AnimationPlayer.CurrentState == IdleAnimName)
            CanReload = true;
        if (SpareAmmo <= 0 || AmmoInWeapon == AmmoInMag)
            CanReload = false;
            
        if (CanReload)
        {
            int _ammoNeeded = AmmoInMag - AmmoInWeapon;

            if (SpareAmmo >= _ammoNeeded)
            {
                SpareAmmo -= _ammoNeeded;
                AmmoInWeapon = AmmoInMag;
            }
            else
            {
                AmmoInWeapon += SpareAmmo;
                SpareAmmo = 0;
            }

            Playernode.AnimationPlayer.SetAnimation(ReloadingAnimName);
            //Playernode.CreateSound(GunCockSound, Playernode.Camera.GlobalTransform.origin);
            return true;
        }
        return false;
    }
}