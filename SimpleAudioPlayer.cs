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

public class SimpleAudioPlayer : Spatial
{
    private AudioStream _audioPistolShot;
    private AudioStream _audioGunCock;
    private AudioStream _audioRifleShot;
    private AudioStream _explosionSound;

    private AudioStreamPlayer _audioNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _audioPistolShot = ResourceLoader.Load<AudioStream>("res://assets/audio/sounds/weapons/gun_revolver_pistol_shot_04.wav");
        _audioGunCock = ResourceLoader.Load<AudioStream>("res://assets/audio/sounds/weapons/GUN_MP5K_Cocked_Full_Action_02.wav");
        _audioRifleShot = ResourceLoader.Load<AudioStream>("res://assets/audio/sounds/weapons/gun_rifle_sniper_shot_01.wav");
        _explosionSound = ResourceLoader.Load<AudioStream>("res://assets/audio/sounds/weapons/explosion_large_no_tail_03.wav");
        _audioNode = GetNode<AudioStreamPlayer>("Audio_Stream_Player");
        _audioNode.Connect("finished", this, "DestroySelf");
        _audioNode.Stop();
    }

    public void PlaySound(string _soundName, Vector3 position = new Vector3())
    {
        if (_audioPistolShot == null || _audioRifleShot == null || _audioGunCock == null)
        {
            GD.Print("Audio not set!");
            QueueFree();
            return;
        }

        switch (_soundName)
        {
            case "Pistol_shot":
                _audioNode.Stream = _audioPistolShot;
                break;
            case "Rifle_shot":
                _audioNode.Stream = _audioRifleShot;
                break;
            case "Gun_cock":
                _audioNode.Stream = _audioGunCock;
                break;
            case "Explosion":
                _audioNode.Stream = _explosionSound;
                break;
            default:
                GD.Print("UNKOWN STREAM");
                QueueFree();
                return;
        }

        /*
        if ( _audioNode is AudioStreamPlayer3D && position != null )
            _audioNode.GlobalTransform = new Transform(GlobalTransform.basis, position);
        */
        _audioNode.Play();
    }

    public void DestroySelf()
    {
        _audioNode.Stop();
        QueueFree();
    }
}