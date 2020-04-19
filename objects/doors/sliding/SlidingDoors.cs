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

public class SlidingDoors : KinematicBody
{
    private Area _area;
    private bool _opening;
    [Export]
    public float DoorSpeed = 2;
    private Transform _translation;
    [Export]
    public float MaxOpen = 10;
    private Transform _maxTotalTransform;
    private Transform _originalTransform;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _area = GetNode<Area>("Area");
        _area.Connect("body_entered", this, "BodyEnteredArea");
        _area.Connect("body_exited", this, "BodyExitedArea");
        _maxTotalTransform = GlobalTransform.Translated(new Vector3(MaxOpen,0,0));
        _originalTransform = GlobalTransform;
        _translation = GlobalTransform;
        _opening = false;
    }

    public void BodyEnteredArea(Node _body)
    {
        if (_body is Player && _body != this)
            _opening = true;
    }
    
    public void BodyExitedArea(Node _body)
    {
        if (_body is Player && _body != this)
            _opening = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        _translation = GlobalTransform.Translated(new Vector3(delta*DoorSpeed,0,0));
        if(_opening && GlobalTransform.origin > _maxTotalTransform.origin)
            GlobalTransform = GlobalTransform.Translated(new Vector3(delta*DoorSpeed,0,0));
        else if (!_opening && GlobalTransform.origin < _originalTransform.origin)
            GlobalTransform = GlobalTransform.Translated(new Vector3(delta*-DoorSpeed,0,0));
    }
}