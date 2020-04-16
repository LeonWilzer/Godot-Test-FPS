using Godot;
using System;

public class AmmoPickup : Spatial
{
    private int _kitSize = 1;
    [Export(PropertyHint.Enum, "small, big")]
    public int KitSize
    {
        get { return _kitSize; }
        set {
            if (_isReady)
            {
                KitSizeChangeValues(_kitSize, false);
                _kitSize = value;
                KitSizeChangeValues(_kitSize, true);
            }
            else
                _kitSize = value;
        }
    }
    private int[] _AmmoAmounts;
    private float _respawnTime;
    private float _respawnTimer;
    private bool _isReady;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // 0 = small; 1 = big
        _AmmoAmounts = new int[] { 1, 4 };
        _respawnTime = 3;
        _respawnTimer = 0;
        GetNode<Area>("Holder/Ammo_Pickup_Trigger").Connect("body_entered", this, "TriggerBodyEntered");

        _isReady = true;

        KitSizeChangeValues(0, false);
        KitSizeChangeValues(1, false);
        KitSizeChangeValues(KitSize, true);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_respawnTimer > 0)
        {
            _respawnTimer -= delta;

            if (_respawnTimer <= 0)
                KitSizeChangeValues(KitSize, true);
        }
    }

    public void KitSizeChangeValues(int size, bool enable)
    {
        if (size == 1)
        {
            GetNode<CollisionShape>("Holder/Ammo_Pickup_Trigger/Shape_Kit").Disabled = !enable;
            GetNode<Spatial>("Holder/Ammo_Kit").Visible = enable;
        }
        else if (size == 0)
        {
            GetNode<CollisionShape>("Holder/Ammo_Pickup_Trigger/Shape_Kit_Small").Disabled = !enable;
            GetNode<Spatial>("Holder/Ammo_Kit_Small").Visible = enable;
        }
    }

    public void TriggerBodyEntered(Player _body)
    {
        if (_body is Player)
        {
            _body.AddAmmo(_AmmoAmounts[KitSize]);
            _respawnTimer = _respawnTime;
            KitSizeChangeValues(KitSize, false);
        }
    }
}