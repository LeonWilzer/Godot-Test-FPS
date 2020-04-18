using Godot;
using System;

public class TurretBody : StaticBody
{   public void BulletHit(int _damage, Transform _bulletHitPos)
    {
        GetNode<Turret>("../../").BulletHit(_damage, _bulletHitPos);
    }
}
