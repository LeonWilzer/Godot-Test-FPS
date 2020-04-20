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

public class DebugDisplay : Control
{
    private Label _fps;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Label>("OS_Label").Text = "OS: " + OS.GetName();
        GetNode<Label>("Engine_Label").Text = "Godot version: " + Engine.GetVersionInfo();
        _fps = GetNode<Label>("FPS_Label");
    }

    public override void _Process(float delta)
    {
        _fps.Text = "FPS: " + Engine.GetFramesPerSecond();
    }
}