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

public abstract class WeaponPoint : Spatial
{
    protected int damage;

    public string idleAnimName { get; protected set; }
    public string fireAnimName { get; protected set; }

    protected string unequipAnimation;
    protected string equipAnimation;

    public bool isWeaponEnabled { get; protected set; }

    public Player playernode { protected get;  set; }

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
        {
            playernode.AnimationPlayer.SetAnimation(equipAnimation);
        }
        return false;
    }
}
