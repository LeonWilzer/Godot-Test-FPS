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
	public float Gravity = -50f;
	[Export]
	public float MaxSpeed = 20.0f;
	[Export]
	public float JumpSpeed = 25f;
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
	public float FragGrenadeThrowForce = 50f;
	[Export]
	public float ObjectThrowForce = 60;
	public float ObjectGrabDistance = 10;
	public float ObjectGrabRayDistance = 10;

	// Character Stats
	[Export]
	public int Health = 150;
	[Export]
	public int MaxHealth = 150;
	// 0: Frag FragGrenade 1: Sticky FragGrenade
	[Export]
	private int[] _grenadeAmmounts = new int[] { 2, 2 };
	[Export]
	private int MaxGrenades = 4;
	[Export]
	public float RespawnTime = 4;

	// Variables relevant for character actions.
	private bool _isSprinting;
	private string _changingWeaponName;
	private string _currentWeaponName;
	private bool _changingWeapon;
	private bool _reloadingWeapon;
	private float _mouseScrollValue;
	private float _mouseSensitivityScrollWheel;
	private int _currentGrenade;
	private string[] _grenadeNames;
	private object _grabbedObject;
	private RigidBody _grabbedRigid;
	private float _deadTime;
	private bool _isDead;

	// Relevant nodes.
	private SpotLight _flashlight;
	public AnimationManager AnimationPlayer { get; private set; }
	private PackedScene _simpleAudioPlayer;
	private Vector3 _vel;
	private Vector3 _dir;
	private Label _uiStatusLabel;
	public Camera Camera { get; private set; }
	private Spatial _rotationHelper;
	private PausePopup _pausePopup;
	private Globals _globals;
	public SpawnPoint SpawnPoint { get; set; }

	// Relevant scenes.
	private PackedScene _fragGrenadeScene;
	private PackedScene _stickyGrenadeScene;
	private PackedScene _pausePopupScene;

	// Dictionaries for weapons.
	private Dictionary<string, Weapon> _weapons = new Dictionary<string, Weapon>();
	private Dictionary<int, string> _weaponNumberToName = new Dictionary<int, string>();
	private Dictionary<string, int> _weaponNameToNumber = new Dictionary<string, int>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Constructor
		_vel = new Vector3();
		_dir = new Vector3();

		// Assigns relevant nodes to variables.
		Camera = GetNode<Camera>("Rotation_Helper/Camera");
		_rotationHelper = GetNode<Spatial>("Rotation_Helper");
		_flashlight = GetNode<SpotLight>("Rotation_Helper/Flashlight");
		AnimationPlayer = GetNode<AnimationManager>("Rotation_Helper/Model/Animation_Player");
		_simpleAudioPlayer = ResourceLoader.Load<PackedScene>("res://common/audioplayer/SimpleAudioPlayer.tscn");
		_uiStatusLabel = GetNode<Label>("HUD/Panel/Gun_label");
		Vector3 GunAimPointPos = GetNode<Spatial>("Rotation_Helper/Gun_Aim_Point").GlobalTransform.origin;
		_globals = GetNode<Globals>("/root/Globals");

		// Assigns relevant scenes to variables.
		_fragGrenadeScene = ResourceLoader.Load<PackedScene>("res://weaponry/grenades/frag/FragGrenade.tscn");
		_stickyGrenadeScene = ResourceLoader.Load<PackedScene>("res://weaponry/grenades/sticky/StickyGrenade.tscn");
		_pausePopupScene = ResourceLoader.Load<PackedScene>("res://ui/popups/pause/PausePopup.tscn");

		// Dictionary for Weapon Nodes.
		_weapons.Add("KNIFE", GetNode<Knife>("Rotation_Helper/Gun_Fire_Points/Knife"));
		_weapons.Add("RIFLE", GetNode<Rifle>("Rotation_Helper/Gun_Fire_Points/Rifle"));
		_weapons.Add("PISTOL", GetNode<Pistol>("Rotation_Helper/Gun_Fire_Points/Pistol"));
		_weapons.Add("UNARMED", null);

		// Setup each Weapon in _weapons Dictionary
		foreach (string Weapon in _weapons.Keys)
		{
			if (_weapons[Weapon] != null)
			{
			Weapon _weaponNode = _weapons[Weapon];
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
		_deadTime = 0;
		_isDead = false;

		// Character stats
		_grenadeNames = new string[] { "Frag Grenade", "Sticky Grenade" };
		RespawnTime = 4;

		// The method being called once an fire animation plays.
		AnimationPlayer.CallbackFunction = GD.FuncRef(this, nameof(FireBullet));

		Camera.Far = 1000;
		// Set Transformation to spawnpoint
		// GlobalTransform = _globals.GetRespawnPosition(GlobalTransform.basis);

		// Captures mouse, used as indication that setup is complete.
		Input.SetMouseMode(Input.MouseMode.Captured);
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if (!_isDead)
		{
			ProcessInput(delta);
			ProcessMovement(delta);
		}
		if (_grabbedObject == null)
		{
			ProcessChangingWeapons(delta);
			ProcessReloading(delta);
		}
		ProcessUI(delta);
		ProcessRespawn(delta);
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
		if (IsOnFloor() && Input.IsActionJustPressed("movement_jump"))
				_vel.y = JumpSpeed;
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
			Weapon _currentWeapon = _weapons[_currentWeaponName];
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
			Weapon _currentWeapon = _weapons[_currentWeaponName];
			if (_currentWeapon != null && _currentWeapon.CanReload)
			{
				string _currentAnimState = AnimationPlayer.CurrentState;
				bool _isReloading = false;
				foreach (string _weapon in _weapons.Keys)
				{
					Weapon _weaponNode = _weapons[_weapon];
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
				_grenadeClone = (FragGrenade)_fragGrenadeScene.Instance();
			else
			{
				_grenadeClone = (StickyGrenade)_stickyGrenadeScene.Instance();
				// Sticky grenades will stick to the player if we do not pass ourselves
				_grenadeClone.PlayerBody = this;
			}

			GetTree().Root.AddChild(_grenadeClone);
			_grenadeClone.GlobalTransform = GetNode<Spatial>("Rotation_Helper/FragGrenade_Toss_Pos").GlobalTransform;
			_grenadeClone.ApplyImpulse(new Vector3(0,0,0), _grenadeClone.GlobalTransform.basis.z * FragGrenadeThrowForce);
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//	Grabbing and throwing objects
		if (Input.IsActionJustPressed("fire") && _currentWeaponName == "UNARMED")
			if (_grabbedObject == null)
			{
				PhysicsDirectSpaceState _state = GetWorld().DirectSpaceState;

				Vector2 _centerPosition = GetViewport().Size / 2;
				Vector3 _rayFrom = Camera.ProjectRayOrigin(_centerPosition);
				Vector3 _rayTo = _rayFrom + Camera.ProjectRayNormal(_centerPosition) * ObjectGrabRayDistance;
				Godot.Collections.Array _exclude = new Godot.Collections.Array();
				_exclude.Add(this);
				_exclude.Add(GetNode<Area>("Rotation_Helper/Gun_Fire_Points/Knife/Area"));
				Godot.Collections.Dictionary _rayResult = _state.IntersectRay(_rayFrom, _rayTo, _exclude);
				if (_rayResult.Count != 0 && _rayResult["collider"] is RigidBody)
				{
					_grabbedObject = _rayResult["collider"];
					RigidBody _grabbedRigid = (RigidBody)_grabbedObject;
					_grabbedRigid.Mode = RigidBody.ModeEnum.Static;

					_grabbedRigid.CollisionLayer = 0;
					_grabbedRigid.CollisionMask = 0;
				}
			}
			else
			{
				_grabbedRigid = (RigidBody)_grabbedObject;

				_grabbedRigid.Mode = RigidBody.ModeEnum.Rigid;

				_grabbedRigid.ApplyImpulse(new Vector3(0,0,0), -Camera.GlobalTransform.basis.z.Normalized() * ObjectThrowForce);

				_grabbedRigid.CollisionLayer = 1;
				_grabbedRigid.CollisionMask = 1;

				_grabbedRigid = null;
				_grabbedObject = null;
			}
		if (_grabbedObject != null)
		{
			_grabbedRigid = (RigidBody)_grabbedObject;
			Transform _transform = new Transform(_grabbedRigid.GlobalTransform.basis, Camera.GlobalTransform.origin + (-Camera.GlobalTransform.basis.z.Normalized() * ObjectGrabDistance));
			_grabbedRigid.GlobalTransform = _transform;
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//	Pause Popup
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (_pausePopup == null)
			{
				_pausePopup = (PausePopup)_pausePopupScene.Instance();
				_pausePopup.Player = this;

				_globals.CanvasLayer.AddChild(_pausePopup);
				_pausePopup.PopupCentered();

				Input.SetMouseMode(Input.MouseMode.Visible);

				GetTree().Paused = true;
			}
			else
			{
				Input.SetMouseMode(Input.MouseMode.Captured);
				_pausePopup = null;
			}
		}
		//  -------------------------------------------------------------------
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
		_vel = MoveAndSlide(_vel, new Vector3(0, 1, 0), true, 4, Mathf.Deg2Rad(MaxSlopeAngle));
	}
	private void ProcessChangingWeapons(float delta)
	{
		if (_changingWeapon == true)
		{
			bool WeaponUnequipped = false;
			Weapon _currentWeapon = _weapons[_currentWeaponName];

			if (_currentWeapon == null)
				WeaponUnequipped = true;
			else if (_currentWeapon.IsWeaponEnabled == true)
				WeaponUnequipped = _currentWeapon.UnequipWeapon();
			else
				WeaponUnequipped = true;

			if (WeaponUnequipped == true)
			{
				bool WeaponEquipped = false;
				Weapon WeaponToEquip = _weapons[_changingWeaponName];
			
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
			_uiStatusLabel.Text = "HEALTH: " + Health + "\n" + _grenadeNames[_currentGrenade] + ": " + _grenadeAmmounts[_currentGrenade];
		else
		{
			Weapon _currentWeapon = _weapons[_currentWeaponName];
			_uiStatusLabel.Text = "HEALTH: " + Health.ToString() + "\nAMMO: " + _currentWeapon.AmmoInWeapon.ToString() + "/" + _currentWeapon.SpareAmmo.ToString()
			+ "\n" + _grenadeNames[_currentGrenade] + ": " + _grenadeAmmounts[_currentGrenade];
		}
	}

	private void ProcessReloading(float delta)
	{
		if (_reloadingWeapon)
		{
			Weapon _currentWeapon = _weapons[_currentWeaponName];
			if (_currentWeapon != null)
				_currentWeapon.ReloadWeapon();
			_reloadingWeapon = false;
		}
	}

	private void ProcessRespawn(float delta)
	{
		if (Health <= 0 && !_isDead)
		{
			GetNode<CollisionShape>("Body_CollisionShape").Disabled = true;
			GetNode<CollisionShape>("Feet_CollisionShape").Disabled = true;

			_changingWeapon = true;
			_changingWeaponName = "UNARMED";

			GetNode<ColorRect>("HUD/Death_Screen").Visible = true;

			GetNode<Panel>("HUD/Panel").Visible = false;
			GetNode<Control>("HUD/Crosshair").Visible = false;

			_deadTime = RespawnTime;
			_isDead = true;
		}

		if (_isDead)
		{
			_deadTime -= delta;
			GetNode<Label>("HUD/Death_Screen/Label").Text = "You died\n" + (int)_deadTime + " seconds till respawn";

			if (_deadTime <= 0)
			{
				SpawnPoint.RespawnPlayer(this);

				GetNode<CollisionShape>("Body_CollisionShape").Disabled = false;
				GetNode<CollisionShape>("Feet_CollisionShape").Disabled = false;

				GetNode<ColorRect>("HUD/Death_Screen").Visible = false;

				GetNode<Panel>("HUD/Panel").Visible = true;
				GetNode<Control>("HUD/Crosshair").Visible = true;

				foreach (string _weapon in _weapons.Keys)
				{
					Weapon _weaponNode = _weapons[_weapon];
					if (_weaponNode != null)
						_weaponNode.ResetWeapon();
				}

				Health = MaxHealth;

				for (int i=0; i<_grenadeAmmounts.Length; i++)
					_grenadeAmmounts[i] = MaxGrenades / 2;
				_currentGrenade = 0;

				_isDead = false;
			}
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (_isDead)
			return;
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
			cameraRot.x = Mathf.Clamp(cameraRot.x, -89, 89);
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

	public void AddFragGrenade(int _additionalFragGrenades)
	{
		for (int i = 0; i<_grenadeAmmounts.Length; i++ )
		{
			_grenadeAmmounts[i] += _additionalFragGrenades;
			_grenadeAmmounts[i] = Mathf.Clamp(_grenadeAmmounts[i], 0, MaxGrenades);
		}
	}

	public void BulletHit(int _damage, Transform _bulletGlobalTrans)
	{
		Health -= _damage;
		Health = Mathf.Clamp(Health, 0, MaxHealth);
	}
}