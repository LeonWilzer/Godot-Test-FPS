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
	// Engine Properties
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
	[Export]
	public float GrenadeThrowForce = 50f;

	// Character Stats
	[Export]
	public int Health = 100;
	[Export]
	public int MaxHealth = 150;
	[Export]
	public int MaxGrenades = 4;

	// Variables relevant for character actions.
	private bool _isSprinting;
	private string _changingWeaponName;
	private string _currentWeaponName;
	private bool _changingWeapon;
	private bool _reloadingWeapon;
	private float _mouseScrollValue;
	private float _mouseSensitivityScrollWheel;
	private int _currentGrenade;
	private int[] _grenadeAmmounts;
	private string[] _grenadeNames;


	// Relevant Nodes.
	private SpotLight _flashlight;
	public AnimationManager AnimationPlayer { get; private set; }
	private PackedScene _simpleAudioPlayer;
	private Vector3 _vel;
	private Vector3 _dir;
	private Label _uiStatusLabel;
	public Camera Camera { get; private set; }
	private Spatial _rotationHelper;
	private PackedScene _grenadeScene;
	private PackedScene _stickyGrenadeScene;

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
		Camera = GetNode<Camera>("Rotation_Helper/Camera");
		_rotationHelper = GetNode<Spatial>("Rotation_Helper");
		_flashlight = GetNode<SpotLight>("Rotation_Helper/Flashlight");
		AnimationPlayer = GetNode<AnimationManager>("Rotation_Helper/Model/Animation_Player");
		_simpleAudioPlayer = ResourceLoader.Load<PackedScene>("res://Simple_Audio_Player.tscn");
		_uiStatusLabel = GetNode<Label>("HUD/Panel/Gun_label");
		Vector3 GunAimPointPos = GetNode<Spatial>("Rotation_Helper/Gun_Aim_Point").GlobalTransform.origin;
		_grenadeScene = ResourceLoader.Load<PackedScene>("res://Grenade.tscn");
		_stickyGrenadeScene = ResourceLoader.Load<PackedScene>("res://Sticky_Grenade.tscn");

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
			WeaponPoint _weaponNode = _weapons[Weapon];
			_weaponNode.Playernode = this;
			// Make Weaponnode point towards the aim position.
			_weaponNode.LookAt(GunAimPointPos, new Vector3(0,1,0));
			// Rotate it by 180° on the Y axis.
			_weaponNode.RotateObjectLocal(new Vector3(0,1,0), Mathf.Deg2Rad(180));
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
		_reloadingWeapon = false;
		_mouseSensitivityScrollWheel = 0.08f;
		_currentGrenade = 0;

		// Character stats
		// 0: Frag Grenade 1: Sticky Grenade
		_grenadeAmmounts = new int[] { 2, 2 };
		_grenadeNames = new string[] { "Frag Grenade", "Sticky Grenade" };

		// The method being called once an fire animation plays.
		AnimationPlayer.CallbackFunction = GD.FuncRef(this, nameof(FireBullet));

		// Captures mouse, used as indication that all the variables are assigned.
		Input.SetMouseMode(Input.MouseMode.Captured);

		Camera.Far = 1000;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		ProcessInput(delta);
		ProcessMovement(delta);
		ProcessChangingWeapons(delta);
		ProcessUI(delta);
		ProcessReloading(delta);
	}
	
	private void ProcessInput(float delta)
	{
		//  -------------------------------------------------------------------
		//  Walking
		_dir = new Vector3();
		Transform camXform = Camera.GlobalTransform;

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

		int _weaponChangeNumber = _weaponNameToNumber[_currentWeaponName];

		if (Input.IsActionJustPressed("Weapon1"))
			_weaponChangeNumber = 0;
		if (Input.IsActionJustPressed("Weapon2"))
			_weaponChangeNumber = 1;
		if (Input.IsActionJustPressed("Weapon3"))
			_weaponChangeNumber = 2;			
		if (Input.IsActionJustPressed("Weapon4"))
			_weaponChangeNumber = 3;

		if (Input.IsActionJustPressed("shift_weapon_positive"))
			_weaponChangeNumber++;
		if (Input.IsActionJustPressed("shift_weapon_negative"))
			_weaponChangeNumber--;

		_weaponChangeNumber = Mathf.Clamp(_weaponChangeNumber, 0 , _weaponNumberToName.Count);

		if (_weaponNumberToName[_weaponChangeNumber] != _currentWeaponName)
			if ( !_reloadingWeapon && !_changingWeapon)
			{
				_changingWeaponName = _weaponNumberToName[_weaponChangeNumber];
				_changingWeapon = true;
				_mouseScrollValue = _weaponChangeNumber;
			}
		//  -------------------------------------------------------------------
		
		//  -------------------------------------------------------------------
		//  Firing Weapon
		if (Input.IsActionPressed("fire") && !_changingWeapon && !_reloadingWeapon)
		{
			WeaponPoint _currentWeapon = _weapons[_currentWeaponName];
			if (_currentWeapon != null && _currentWeapon.AmmoInWeapon > 0 )
			{
				if (AnimationPlayer.CurrentState == _currentWeapon.IdleAnimName)
				{
					AnimationPlayer.CallbackFunction = GD.FuncRef(this, nameof(FireBullet));
					AnimationPlayer.SetAnimation(_currentWeapon.FireAnimName);
				}
			}
			else
				_reloadingWeapon = true;
				
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//	Reloading
		if (!_reloadingWeapon && !_changingWeapon && Input.IsActionJustPressed("reload"))
		{
			WeaponPoint _currentWeapon = _weapons[_currentWeaponName];
			if (_currentWeapon != null && _currentWeapon.CanReload)
			{
				string _currentAnimState = AnimationPlayer.CurrentState;
				bool _isReloading = false;
				foreach (string _weapon in _weapons.Keys)
				{
					WeaponPoint _weaponNode = _weapons[_weapon];
						if ( _weaponNode != null && _currentAnimState == _weaponNode.ReloadingAnimName )
							_isReloading = true;
				}
				if (!_isReloading)
					_reloadingWeapon = true;
			}
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//	Changing and throwing grenades

		if (Input.IsActionJustPressed("change_grenade"))
		{
			if (_currentGrenade == 0)
				_currentGrenade = 1;
			else if (_currentGrenade == 1)
				_currentGrenade = 0;
		}

		if (Input.IsActionJustPressed("fire_grenade") && _grenadeAmmounts[_currentGrenade] > 0)
		{
			_grenadeAmmounts[_currentGrenade]--;

			Grenade _grenadeClone;
			if (_currentGrenade == 0)
				_grenadeClone = (Grenade)_grenadeScene.Instance();
			else
			{
				_grenadeClone = (StickyGrenade)_stickyGrenadeScene.Instance();
				// Sticky grenades will stick to the player if we do not pass ourselves
				_grenadeClone.PlayerBody = this;
			}

			GetTree().Root.AddChild(_grenadeClone);
			_grenadeClone.GlobalTransform = GetNode<Spatial>("Rotation_Helper/Grenade_Toss_Pos").GlobalTransform;
			_grenadeClone.ApplyImpulse(new Vector3(0,0,0), _grenadeClone.GlobalTransform.basis.z * GrenadeThrowForce);
		}
	}
	 private void ProcessMovement(float delta)
	{
		_dir.y = 0;
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
	private void ProcessChangingWeapons(float delta)
	{
		if (_changingWeapon == true)
		{
			bool WeaponUnequipped = false;
			WeaponPoint _currentWeapon = _weapons[_currentWeaponName];

			if (_currentWeapon == null)
				WeaponUnequipped = true;
			else if (_currentWeapon.IsWeaponEnabled == true)
				WeaponUnequipped = _currentWeapon.UnequipWeapon();
			else
				WeaponUnequipped = true;

			if (WeaponUnequipped == true)
			{
				bool WeaponEquipped = false;
				WeaponPoint WeaponToEquip = _weapons[_changingWeaponName];
			
				if (WeaponToEquip == null)
					WeaponEquipped = true;
				else if (WeaponToEquip.IsWeaponEnabled == false)
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

	private void ProcessUI(float delta)
	{
		//_uiStatusLabel.Text = "FPS: " + Engine.GetFramesPerSecond();
		if (_currentWeaponName == "UNARMED" || _currentWeaponName == "KNIFE")
			_uiStatusLabel.Text = "HEALTH: " + Health + "\n" + _grenadeNames[_currentGrenade] + ": " + _grenadeAmmounts[_currentGrenade]
			+ "\nFPS: " + Engine.GetFramesPerSecond();
		else
		{
			WeaponPoint _currentWeapon = _weapons[_currentWeaponName];
			_uiStatusLabel.Text = "HEALTH: " + Health.ToString() + "\nAMMO: " + _currentWeapon.AmmoInWeapon.ToString() + "/" + _currentWeapon.SpareAmmo.ToString()
			+ "\n" + _grenadeNames[_currentGrenade] + ": " + _grenadeAmmounts[_currentGrenade] + "\nFPS: " + Engine.GetFramesPerSecond();
		}
	}

	private void ProcessReloading(float delta)
	{
		if (_reloadingWeapon)
		{
			WeaponPoint _currentWeapon = _weapons[_currentWeaponName];
			if (_currentWeapon != null)
				_currentWeapon.ReloadWeapon();
			_reloadingWeapon = false;
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
			if (mouseEvent.ButtonMask == 4 || mouseEvent.ButtonMask == 5)
			{
			if (mouseEvent.ButtonMask == 4)
				_mouseScrollValue += _mouseSensitivityScrollWheel;
			else if (mouseEvent.ButtonMask == 5)
				_mouseScrollValue -= _mouseSensitivityScrollWheel;

			_mouseScrollValue = Mathf.Clamp(_mouseScrollValue, 0, _weaponNumberToName.Count -1);

			if (!_changingWeapon && !_reloadingWeapon)
				{
					int _roundMouseScrollValue = (int)Mathf.Round(_mouseScrollValue);
					if (_weaponNumberToName[_roundMouseScrollValue] != _currentWeaponName)
					{
						_changingWeaponName = _weaponNumberToName[_roundMouseScrollValue];
						_changingWeapon = true;
						_mouseScrollValue = _roundMouseScrollValue;
					}
				}
			}
		}
	}

	public void FireBullet()
	{
		if (_changingWeapon)
			return;
		_weapons[_currentWeaponName].FireWeapon();
	}

	public void CreateSound(string _soundName, Vector3 _position = new Vector3())
	{
		SimpleAudioPlayer _audioClone = (SimpleAudioPlayer)_simpleAudioPlayer.Instance();
		Node _sceneRoot = GetTree().Root.GetChild(0);
		_sceneRoot.AddChild(_audioClone);
		if (_position == new Vector3())
			_audioClone.PlaySoundGlobal(_soundName);
		else
			_audioClone.PlaySoundLocal(_soundName, _position);
	}

	// Temporary method for play a cock sound at specified times of an animation
	// WILL BE REPLACED WITH SOMETHING MORE SOPHISTICATED!
	public void CockGun()
	{
		CreateSound(_weapons[_currentWeaponName].GunCockSound);
	}

	public void AddHealth(int _additionalHealth)
	{
		Health += _additionalHealth;
		Health = Mathf.Clamp(Health, 0, MaxHealth);
	}

	public void AddAmmo(int _additionalAmmo)
	{
		if (_currentWeaponName != "UNARMED" && _weapons[_currentWeaponName].CanRefill)
			foreach (string _weapon in _weapons.Keys)
				if (_weapon != "UNARMED" && _weapon != "KNIFE")
				{
					_weapons[_weapon].SpareAmmo += _weapons[_weapon].AmmoInMag * _additionalAmmo;
					_weapons[_weapon].SpareAmmo = Mathf.Clamp(_weapons[_weapon].SpareAmmo, 0, _weapons[_weapon].MaxAmmo);
				}
	}

	public void AddGrenade(int _additionalGrenades)
	{
		for (int i = 0; i<_grenadeAmmounts.Length; i++ )
		{
			_grenadeAmmounts[i] += _additionalGrenades;
			_grenadeAmmounts[i] = Mathf.Clamp(_grenadeAmmounts[i], 0, MaxGrenades);
		}
	}
}