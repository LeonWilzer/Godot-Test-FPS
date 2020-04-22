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

public class MainMenu : Control
{
    private VBoxContainer _startMenu;
    private VBoxContainer _optionsMenu;
    private Globals _globals;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _startMenu = GetNode<VBoxContainer>("StartMenu");
        _optionsMenu = GetNode<VBoxContainer>("OptionsMenu");
        
        // Note: insert Connect methods for star and options andhere once found out how to pass parameters down to methods.

        _globals = GetNode<Globals>("/root/Globals");

        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public void StartMenuButtonPressed(string _buttonName)
    {
        if (_buttonName == "start")
            _globals.LoadNewScene("res://levels/main/Main.tscn");
        else if (_buttonName == "options")
        {
            _startMenu.Visible = false;
            _optionsMenu.Visible = true;
        }
        else if (_buttonName == "quit")
            GetTree().Quit();
    }

    public void OptionsMenuButtonPressed(string _buttonName)
    {
        if (_buttonName == "back")
        {
            _optionsMenu.Visible = false;
           _startMenu.Visible = true;
        }
        else if (_buttonName == "fullscreen")
            OS.WindowFullscreen = !OS.WindowFullscreen;
        else if (_buttonName == "vsync")
            OS.VsyncEnabled = GetNode<CheckButton>("OptionsMenu/Check_Button_VSync").Pressed;
        else if (_buttonName == "debug")
            _globals.SetDebugDisplay(GetNode<CheckButton>("OptionsMenu/Check_Button_Debug").Pressed);
    }

    public void SetResolution(int _index)
    {
        GD.Print(_index);
    }
}