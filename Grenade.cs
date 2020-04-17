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

public class Grenade : RigidBody
{
    protected int _grenadeDamage;

    protected float _grenadeTime;
    protected float _grenadeTimer;

    protected float _explosionWaitTime;
    protected float _explosionWaitTimer;

    protected CollisionShape _rigidShape;
    protected MeshInstance _grenadeMesh;
    protected Area _blastArea;
    protected Particles _explosionParticles;
    protected AudioStream _explosionSound;
    protected PackedScene _simpleAudioPlayer;
    public Player PlayerBody {get; set;}
    protected FuncRef _callback;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        /*
        _grenadeDamage = 60;

        _grenadeTime = 2;
        _grenadeTimer = 0;

        _explosionWaitTime = 0.48f;
        _explosionWaitTimer = 0;

        _rigidShape = GetNode<CollisionShape>("Collision_Shape");
        _grenadeMesh = GetNode<MeshInstance>("Grenade");
        _blastArea = GetNode<Area>("Blast_Area");
        _explosionParticles = GetNode<Particles>("Explosion");
        _simpleAudioPlayer = ResourceLoader.Load<PackedScene>("res://Simple_Audio_Player.tscn");

        _explosionParticles.Emitting = false;
        _explosionParticles.OneShot = true;
        */
    }
/*
    public override void _Process(float delta)
    {
        if (_grenadeTimer < _grenadeTime)
        {
            _grenadeTimer += delta;
            return;
        }
        else
        {
            if (_explosionWaitTimer <= 0)
            {
                _explosionParticles.Emitting = true;

                _grenadeMesh.Visible = false;
                _rigidShape.Disabled = true;

                Mode = RigidBody.ModeEnum.Static;

                Godot.Collections.Array _bodies = _blastArea.GetOverlappingBodies();
                foreach (PhysicsBody _body in _bodies)
                    if (_body.HasMethod("BulletHit"))
                    {
                        _callback = GD.FuncRef(_body, "BulletHit");
                        _callback.CallFunc();
                    }

                SimpleAudioPlayer _audioClone = (SimpleAudioPlayer)_simpleAudioPlayer.Instance();
                Node _sceneRoot = GetTree().Root.GetChild(0);
		        _sceneRoot.AddChild(_audioClone);
		        _audioClone.PlaySoundLocal("Explosion", GlobalTransform.origin);

                if (_explosionWaitTimer < _explosionWaitTime)
                {
                    _explosionWaitTimer += delta;

                    if (_explosionWaitTimer >= _explosionWaitTime)
                        QueueFree();
                }
            }
        }
    }
*/
}