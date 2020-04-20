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

public class PausePopup : WindowDialog
{
    private string _mainMenuPath;
    private PackedScene _popupScene;
    private Globals _globals;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainMenuPath = "res://ui/menus/mainmenu/MainMenu.tscn";
        _globals = GetNode<Globals>("/root/Globals");

		GetNode("Button_quit").Connect("pressed", this, "PopupQuit");
		Connect("popup_hide", this, "PopupHide");
		GetNode("Button_resume").Connect("pressed", this, "PopupClosed");
    }

    public void PopupHide(){
        GetTree().Paused = false;
        QueueFree();
    }
     public void PopupClosed()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
        PopupHide();
    }

    public void PopupQuit()
    {
        GetTree().Paused = false;

        _globals.LoadNewScene(_mainMenuPath);

        QueueFree();

        Input.SetMouseMode(Input.MouseMode.Visible);
    }
}