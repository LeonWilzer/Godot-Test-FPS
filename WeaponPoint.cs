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
