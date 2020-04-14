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