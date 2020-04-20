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

public class Globals : Node
{
    public float MouseSensitivity;

    // GUI/UI related variables
    private CanvasLayer _canvasLayer;

    private PackedScene _debugDisplayScene;
    private DebugDisplay _debugDisplay;

    private string _mainMenuPath;
    private PackedScene _popupScene;
    private Popup _popup;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MouseSensitivity = 0.08f;
        _debugDisplayScene = ResourceLoader.Load<PackedScene>("res://ui/hud/debugging/DebugDisplay.tscn");
        _canvasLayer = new CanvasLayer();
        AddChild(_canvasLayer);
        _mainMenuPath = "res://ui/menus/mainmenu/MainMenu.tscn";
        _popupScene = ResourceLoader.Load<PackedScene>("res://ui/popups/pause/PausePopup.tscn");
    }

    public void LoadNewScene(string _newScenePath)
    {
        GetTree().ChangeScene(_newScenePath);
    }

    public void SetDebugDisplay(bool _displayOn)
    {
        if (!_displayOn)
        {
            if (_debugDisplay != null)
                {
                    _debugDisplay.QueueFree();
                    _debugDisplay = null;
                }
        }
        else if (_debugDisplay == null)
        {
            _debugDisplay = (DebugDisplay)_debugDisplayScene.Instance();
            _canvasLayer.AddChild(_debugDisplay);
        }
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_cancel") && _popup == null)
        {
            _popup = (Popup)_popupScene.Instance();

            _popup.GetNode<Button>("Button_quit").Connect("pressed", this, "PopupQuit");
            _popup.Connect("popup_hide", this, "PopupClosed");
            _popup.GetNode<Button>("Button_resume").Connect("pressed", this, "PopupClosed");

            _canvasLayer.AddChild(_popup);
            _popup.PopupCentered();

            Input.SetMouseMode(Input.MouseMode.Visible);

            GetTree().Paused = true;
        }
    }

    public void PopupClosed()
    {
        GetTree().Paused = false;
        Input.SetMouseMode(Input.MouseMode.Captured);
        
        if (_popup != null)
        {
            _popup.QueueFree();
            _popup = null;
        }
    }

    public void PopupQuit()
    {
        GetTree().Paused = false;

        Input.SetMouseMode(Input.MouseMode.Visible);

        if (_popup != null)
        {
            _popup.QueueFree();
            _popup = null;
        }
        LoadNewScene(_mainMenuPath);
    }
}