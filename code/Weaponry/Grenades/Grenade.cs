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
}