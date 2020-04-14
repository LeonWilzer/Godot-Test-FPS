using Godot;
using System;

public static class HitTest
{

    private static int _baseBulletBoost = 9;

    // Called when the node enters the scene tree for the first time.
    public static void BulletHit(object physical, int Damage, Transform BulletGlobalTrans)
    {

        if (physical is RigidBody)
        {
            var _directionVect = BulletGlobalTrans.basis.z.Normalized() * _baseBulletBoost;
            RigidBody rigidbody = (RigidBody)physical;
            rigidbody.ApplyImpulse((BulletGlobalTrans.origin - rigidbody.GlobalTransform.origin).Normalized(), _directionVect * Damage);
            GD.Print("RigidBody hit");
        }
        else if (physical is StaticBody)
            GD.Print("StaticBody hit");
        else if (physical is KinematicBody)
            GD.Print("KinematicBody hit");
    }
}