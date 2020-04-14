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
