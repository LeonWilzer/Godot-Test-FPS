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

public class Turret : Spatial
{
    [Export]
    public bool _useRaycast = true;

    private int _TurretDamageBullet;
    private int _TurretDamageRaycast;

    private float _flashTime = 0.1f;
    private float _flashTimer = 0;

    private float _fireTime = 0.8f;
    private float _fireTimer = 0;

    private Spatial _nodeTurretHead;
    private RayCast _nodeRaycast;
    private MeshInstance _nodeFlashOne;
    private MeshInstance _nodeFlashTwo;

    private int _ammoInTurret;
    private int _ammoInFullTurret;
    private float _ammoReloadTime;
    private float _ammoReloadTimer;

    private Godot.Object _currentTarget;
    KinematicBody _currentKinematic;

    private bool _isActive;

    private int _playerHeight;

    private Particles _smokeParticles;

    private int _turretHealth;
    private int _maxTurretHealth;

    private float _destroyedTime;
    private float _destroyedTimer;

    private PackedScene _bulletScene;

    private FuncRef _callback;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _TurretDamageBullet = 20;
        _TurretDamageRaycast = 5;

        _flashTime = 0.1f;
        _flashTimer = 0;

        _fireTime = 0.8f;
        _fireTimer = 0;

        _ammoInTurret = 20;
        _ammoInFullTurret = 20;
        _ammoReloadTime = 4;
        _ammoReloadTimer = 0;

        _isActive = false;

        _playerHeight = 3;

        _maxTurretHealth = 60;
        _turretHealth = _maxTurretHealth;

        _destroyedTime = 20;
        _destroyedTimer = 0;

        _bulletScene = ResourceLoader.Load<PackedScene>("Bullet_Scene.tscn");

        GetNode<Area>("Vision_Area").Connect("body_entered", this, "BodyEnteredVision");
        GetNode<Area>("Vision_Area").Connect("body_exited", this, "BodyExitedVision");

        _nodeTurretHead = GetNode<Spatial>("Head");
        _nodeRaycast = GetNode<RayCast>("Head/Ray_Cast");
        _nodeFlashOne = GetNode<MeshInstance>("Head/Flash");
        _nodeFlashTwo = GetNode<MeshInstance>("Head/Flash_2");

        _nodeRaycast.AddException(this);
        _nodeRaycast.AddException(GetNode<StaticBody>("Base/Static_Body"));
        _nodeRaycast.AddException(GetNode<StaticBody>("Head/Static_Body"));
        _nodeRaycast.AddException(GetNode<Area>("Vision_Area"));

        _nodeFlashOne.Visible = false;
        _nodeFlashTwo.Visible = false;

        _smokeParticles = GetNode<Particles>("Smoke");
        _smokeParticles.Emitting = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_isActive)
        {
            if (_flashTimer > 0)
            {
                _flashTimer -= delta;

                if (_flashTimer <= 0)
                {
                    _nodeFlashOne.Visible = false;
                    _nodeFlashTwo.Visible = false;
                }
            }

            if (_currentTarget != null)
            {
                _nodeTurretHead.LookAt(_currentKinematic.GlobalTransform.origin + new Vector3(0, _playerHeight, 0), new Vector3(0,1,0));

                if (_turretHealth > 0)
                {
                    if (_ammoInTurret > 0)
                        if (_fireTimer > 0)
                            _fireTimer -= delta;
                        else
                            FireBullet();
                    else
                        if (_ammoReloadTimer > 0)
                            _ammoReloadTimer -= delta;
                        else
                            _ammoInTurret = _ammoInFullTurret;
                }
            }
        }

        if (_turretHealth <= 0)
            if (_destroyedTimer > 0)
                _destroyedTimer -= delta;
            else
            {
                _turretHealth = _maxTurretHealth;
                _smokeParticles.Emitting = false;
            }
    }

    public void FireBullet()
    {
        if (_useRaycast)
        {
            _nodeRaycast.LookAt(_currentKinematic.GlobalTransform.origin + new Vector3(0, _playerHeight, 0), new Vector3(0,1,0));

            _nodeRaycast.ForceRaycastUpdate();

            if (_nodeRaycast.IsColliding())
            {
                Godot.Object _body = _nodeRaycast.GetCollider();
                if (_body.HasMethod("BulletHit"))
                {
                    _callback = GD.FuncRef(_body, "BulletHit");
                    _callback.CallFunc(_TurretDamageRaycast, _nodeRaycast.GetCollisionPoint());
                }
                _ammoInTurret--;
            }
        }
        else
        {
            BulletScene _clone = (BulletScene)_bulletScene.Instance();
            Node _sceneRoot = GetTree().Root.GetChild(0);
            _sceneRoot.AddChild(_clone);

            _clone.GlobalTransform = GetNode<Spatial>("Head/Barrel_End").GlobalTransform;
            _clone.Scale = new Vector3(8,8,8);
            _clone._bulletDamage = _TurretDamageBullet;
            _clone._bulletSpeed = 50;

            _ammoInTurret--;
        }

        _nodeFlashOne.Visible = true;
        _nodeFlashTwo.Visible = true;

        _flashTimer = _flashTime;
        _fireTimer = _fireTime;

        if (_ammoInTurret <= 0)
            _ammoReloadTimer = _ammoReloadTime;
    }

    public void BodyEnteredVision(Godot.Object _body)
    {
        if (_currentTarget == null && _body is KinematicBody)
        {
            _currentTarget = (Godot.Object)_body;
            _currentKinematic = (KinematicBody)_body;
            _isActive = true;
        }
    }

    public void BodyExitedVision(Godot.Object _body)
    {
        if (_currentTarget != null && _body == _currentTarget)
        {
            _currentTarget = null;
            _currentKinematic = null;
            _isActive = false;

            _flashTimer = 0;
            _fireTimer = 0;
            _nodeFlashTwo.Visible = false;
            _nodeFlashOne.Visible = false;
        }
    }

    public void BulletHit(int _damage, Transform _bulletHitPos)
    {
        _turretHealth -= _damage;
        if (_turretHealth <= 0)
        {
            _smokeParticles.Emitting = true;
            _destroyedTimer = _destroyedTime;
        }
    }
}
