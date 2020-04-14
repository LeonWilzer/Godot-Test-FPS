using Godot;
using System;

public class Pistol_Point : WeaponPoint
{
    private PackedScene _bulletscene;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        damage = 5;
        idleAnimName = "Pistol_idle";
        fireAnimName = "Pistol_fire";
        unequipAnimation = "Pistol_unequip";
        equipAnimation = "Pistol_equip";
        isWeaponEnabled = false;
        _bulletscene = ResourceLoader.Load<PackedScene>("Bullet_Scene.tscn");
    }

    public override void FireWeapon(){
        var clone = (Bullet_Scene)_bulletscene.Instance();
        Node sceneRoot = GetTree().Root.GetChild(0);
        sceneRoot.AddChild(clone);

        clone.GlobalTransform = GlobalTransform;
        clone.Scale = new Vector3(4, 4, 4);
        clone._bulletDamage = damage;
    }
}