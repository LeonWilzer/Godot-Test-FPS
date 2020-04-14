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
using System.Collections.Generic; 

public class Player : KinematicBody
{
	// Engine Player Properties
	[Export]
	public float Gravity = -24.8f;
	[Export]
	public float MaxSpeed = 20.0f;
	[Export]
	public float JumpSpeed = 18.0f;
	[Export]
	public float Accel = 4.5f;
	[Export]
	public float Deaccel = 16.0f;
	[Export]
	public float MaxSlopeAngle = 40.0f;
	[Export]
	public float MouseSensitivity = 0.05f;
	[Export]
	public float MaxSprintSpeed = 30.0f;
	[Export]
	public float SprintAccel = 18.0f;
	// Character Stats
	private float Health;

	// Variables relevant for character actions.
	private bool _isSprinting;
	private string _changingWeaponName;
	private string _currentWeaponName;
	private bool _changingWeapon;

	// Relevant Nodes.
	private SpotLight _flashlight;
	public AnimationManager AnimationPlayer { get; set; }
	private Vector3 _vel;
	private Vector3 _dir;
	private Label _uiStatusLabel;
	private Camera _camera;
	private Spatial _rotationHelper;

	// Dictionaries for weapons.
	private Dictionary<string, WeaponPoint> _weapons = new Dictionary<string, WeaponPoint>();
	private Dictionary<int, string> _weaponNumberToName = new Dictionary<int, string>();
	private Dictionary<string, int> _weaponNameToNumber = new Dictionary<string, int>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Constructor
		_vel = new Vector3();
		_dir = new Vector3();

		// Assigns relevant Nodes to variables.
		_camera = GetNode<Camera>("Rotation_Helper/Camera");
		_rotationHelper = GetNode<Spatial>("Rotation_Helper");
		_flashlight = GetNode<SpotLight>("Rotation_Helper/Flashlight");
		AnimationPlayer = GetNode<AnimationManager>("Rotation_Helper/Model/Animation_Player");
		//AnimationPlayer = GetNode<AnimationManager>("Rotation_Helper/Model/Animation_Player");
		_uiStatusLabel = GetNode<Label>("HUD/Panel/Gun_label");
		var GunAimPointPos = GetNode<Spatial>("Rotation_Helper/Gun_Aim_Point").GlobalTransform.origin;

		// Dictionary for Weapon Nodes.
		_weapons.Add("KNIFE", GetNode<Knife_Point>("Rotation_Helper/Gun_Fire_Points/Knife_Point"));
		_weapons.Add("RIFLE", GetNode<Rifle_Point>("Rotation_Helper/Gun_Fire_Points/Rifle_Point"));
		_weapons.Add("PISTOL", GetNode<Pistol_Point>("Rotation_Helper/Gun_Fire_Points/Pistol_Point"));
		_weapons.Add("UNARMED", null);

		// Setup each WeaponPoint in _weapons Dictionary
		foreach (string Weapon in _weapons.Keys)
		{
			if (_weapons[Weapon] != null)
			{
			WeaponPoint WeaponNode = _weapons[Weapon];
			WeaponNode.playernode = this;
			// Make Weaponnode point towards the aim position.
			WeaponNode.LookAt(GunAimPointPos, new Vector3(0,1,0));
			// Rotate it by 180° on the Y axis.
			WeaponNode.RotateObjectLocal(new Vector3(0,1,0), Mathf.Deg2Rad(180));
			}
		}

		// Weapon Dictionaries.
		_weaponNumberToName.Add(0,"UNARMED");
		_weaponNumberToName.Add(1,"KNIFE");
		_weaponNumberToName.Add(2,"PISTOL"); 
		_weaponNumberToName.Add(3,"RIFLE");

		_weaponNameToNumber.Add("UNARMED",0);
		_weaponNameToNumber.Add("KNIFE",1);
		_weaponNameToNumber.Add("PISTOL",2);
		_weaponNameToNumber.Add("RIFLE",3);
		
		// Variables relevant for character actions.
		_changingWeapon = false;
		_changingWeaponName = "UNARMED";
		_currentWeaponName = "UNARMED";
		_isSprinting = false;

		// The method being called once an fire animation plays.
		AnimationPlayer.CallbackFunction = GD.FuncRef(this, nameof(FireBullet));

		// Captures mouse, used as indication that all the variables are assigned.
		Input.SetMouseMode(Input.MouseMode.Captured);
	}
	
	public override void _PhysicsProcess(float delta)
	{
		ProcessInput(delta);
		ProcessMovement(delta);
		ProcessChangingWeapons(delta);
		Health = Engine.GetFramesPerSecond();
		GD.Print(Engine.GetFramesPerSecond());
	}
	
	private void ProcessInput(float delta)
	{
		//  -------------------------------------------------------------------
		//  Walking
		_dir = new Vector3();
		Transform camXform = _camera.GlobalTransform;

		Vector2 inputMovementVector = new Vector2();

		if (Input.IsActionPressed("movement_forward"))
			inputMovementVector.y += 1;
		if (Input.IsActionPressed("movement_backward"))
			inputMovementVector.y -= 1;
		if (Input.IsActionPressed("movement_left"))
			inputMovementVector.x -= 1;
		if (Input.IsActionPressed("movement_right"))
			inputMovementVector.x += 1;

		inputMovementVector = inputMovementVector.Normalized();

		// Basis vectors are already normalized.
		_dir += -camXform.basis.z * inputMovementVector.y;
		_dir += camXform.basis.x * inputMovementVector.x;
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//  Jumping
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("movement_jump"))
				_vel.y = JumpSpeed;
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//  Capturing/Freeing the cursor
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (Input.GetMouseMode() == Input.MouseMode.Visible)
				Input.SetMouseMode(Input.MouseMode.Captured);
			else
				Input.SetMouseMode(Input.MouseMode.Visible);
		}
		//  -------------------------------------------------------------------
		
		//  -------------------------------------------------------------------
		//  Sprinting
		if (Input.IsActionPressed("movement_sprint"))
			_isSprinting = true;
		else
			_isSprinting = false;
		//  -------------------------------------------------------------------
		
		//  -------------------------------------------------------------------
		//  Turning the flashlight on/off
		
		if (Input.IsActionJustPressed("flashlight"))
		{
			if(_flashlight.IsVisibleInTree())
				_flashlight.Hide();
			else
				_flashlight.Show();
		}

		//  -------------------------------------------------------------------
		//  Changing _weapons

		var WeaponChangeNumber = _weaponNameToNumber[_currentWeaponName];

		if (Input.IsActionJustPressed("Weapon1"))
			WeaponChangeNumber = 0;
		if (Input.IsActionJustPressed("Weapon2"))
			WeaponChangeNumber = 1;
		if (Input.IsActionJustPressed("Weapon3"))
			WeaponChangeNumber = 2;			
		if (Input.IsActionJustPressed("Weapon4"))
			WeaponChangeNumber = 3;

		if (Input.IsActionJustPressed("shift_weapon_positive"))
			WeaponChangeNumber++;
		if (Input.IsActionJustPressed("shift_weapon_negative"))
			WeaponChangeNumber--;

		WeaponChangeNumber = Mathf.Clamp(WeaponChangeNumber, 0 , _weaponNumberToName.Count);

		if (_weaponNumberToName[WeaponChangeNumber] != _currentWeaponName && !_changingWeapon)
		{
			_changingWeaponName = _weaponNumberToName[WeaponChangeNumber];
			_changingWeapon = true;
		}
		//  -------------------------------------------------------------------
		
		//  -------------------------------------------------------------------
		//  Firing Weapon
		if (Input.IsActionPressed("fire") && !_changingWeapon)
		{
			var CurrentWeapon = _weapons[_currentWeaponName];
			if (CurrentWeapon != null && AnimationPlayer.CurrentState == CurrentWeapon.idleAnimName)
				AnimationPlayer.SetAnimation(CurrentWeapon.fireAnimName);
		}
		//  -------------------------------------------------------------------
	}
	 private void ProcessMovement(float delta)
	{
		//_dir.y = 0;
		_dir = _dir.Normalized();

		_vel.y += delta * Gravity;

		Vector3 hvel = _vel;
		hvel.y = 0;

		Vector3 target = _dir;
		if (_isSprinting)
			target *= MaxSprintSpeed;
		else
			target *= MaxSpeed;

		float accel;
		if (_dir.Dot(hvel) > 0)
			if (_isSprinting)
				accel = SprintAccel;
			else
				accel = Accel;
		else
			accel = Deaccel;

		hvel = hvel.LinearInterpolate(target, accel * delta);
		_vel.x = hvel.x;
		_vel.z = hvel.z;
		_vel = MoveAndSlide(_vel, new Vector3(0, 1, 0), false, 4, Mathf.Deg2Rad(MaxSlopeAngle = 40.0f));
	}
	public void ProcessChangingWeapons(float delta)
	{
		if (_changingWeapon == true)
		{
			var WeaponUnequipped = false;
			var CurrentWeapon = _weapons[_currentWeaponName];

			if (CurrentWeapon == null)
				WeaponUnequipped = true;
			else if (CurrentWeapon.isWeaponEnabled == true)
				WeaponUnequipped = CurrentWeapon.UnequipWeapon();
			else
				WeaponUnequipped = true;

			if (WeaponUnequipped == true)
			{
				var WeaponEquipped = false;
				var WeaponToEquip = _weapons[_changingWeaponName];
			
				if (WeaponToEquip == null)
					WeaponEquipped = true;
				else if (WeaponToEquip.isWeaponEnabled == false)
					WeaponEquipped = WeaponToEquip.EquipWeapon();
				else
					WeaponEquipped = true;

				if (WeaponEquipped == true)
				{
					_changingWeapon = false;
					_currentWeaponName = _changingWeaponName;
					_changingWeaponName = null;
				}
			}
		}
	}
	
	public override void _Input(InputEvent @event)
	{	
		// Rotates player and camera based on mouse input, but only if it's captured.
		if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			// Rotate camera around the x-axis
			_rotationHelper.RotateX(Mathf.Deg2Rad(mouseEvent.Relative.y * MouseSensitivity));
			// Rotate player around the y-axis
			RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

			Vector3 cameraRot = _rotationHelper.RotationDegrees;
			// Clamp rotation around x-axis so that player doesn't overturn
			cameraRot.x = Mathf.Clamp(cameraRot.x, -90, 90);
			_rotationHelper.RotationDegrees = cameraRot;
		}
	}

	public void FireBullet()
	{
		if (_changingWeapon)
			return;
		_weapons[_currentWeaponName].FireWeapon();
	}
}