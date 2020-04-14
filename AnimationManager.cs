using Godot;
using System;
using System.Collections.Generic; 

public class AnimationManager : AnimationPlayer
{
	private Dictionary<string, List<string>> _states = new Dictionary<string, List<string>>();
	private Dictionary<string, float> _animationSpeeds = new Dictionary<string, float>();
	
	public string CurrentState { get; set; }
	public FuncRef CallbackFunction { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Constructor:
		_states.Add("Idle_unarmed", new List<string> {"Knife_equip", "Pistol_equip", "Rifle_equip", "Idle_unarmed"});

		_states.Add("Pistol_equip",new List<string> {"Pistol_idle"});
		_states.Add("Pistol_fire",new List<string> {"Pistol_idle"});
		_states.Add("Pistol_idle",new List<string> {"Pistol_fire", "Pistol_reload", "Pistol_unequip", "Pistol_idle"});
		_states.Add("Pistol_reload",new List<string> {"Pistol_idle"});
		_states.Add("Pistol_unequip",new List<string> {"Idle_unarmed"});

		_states.Add("Rifle_equip",new List<string> {"Rifle_idle"});
		_states.Add("Rifle_fire",new List<string> {"Rifle_idle"});
		_states.Add("Rifle_idle",new List<string> {"Rifle_fire", "Rifle_reload", "Rifle_unequip", "Rifle_idle"});
		_states.Add("Rifle_reload",new List<string> {"Rifle_idle"});
		_states.Add("Rifle_unequip",new List<string> {"Idle_unarmed"});

		_states.Add("Knife_equip",new List<string> {"Knife_idle"});
		_states.Add("Knife_fire",new List<string> {"Knife_idle"});
		_states.Add("Knife_idle",new List<string> {"Knife_fire", "Knife_unequip", "Knife_idle"});
		_states.Add("Knife_unequip",new List<string> {"Idle_unarmed"});

		_animationSpeeds.Add("Idle_unarmed",1f);

		_animationSpeeds.Add("Pistol_equip",2.5f);
		_animationSpeeds.Add("Pistol_fire",1.8f);
		_animationSpeeds.Add("Pistol_idle",1f);
		_animationSpeeds.Add("Pistol_reload",1f);
		_animationSpeeds.Add("Pistol_unequip",2.5f);

		_animationSpeeds.Add("Rifle_equip",1.5f);
		_animationSpeeds.Add("Rifle_fire",6f);
		_animationSpeeds.Add("Rifle_idle",1f);
		_animationSpeeds.Add("Rifle_reload",1);
		_animationSpeeds.Add("Rifle_unequip",1.5f);

		_animationSpeeds.Add("Knife_equip",3f);
		_animationSpeeds.Add("Knife_fire",1.35f);
		_animationSpeeds.Add("Knife_idle",1f);
		_animationSpeeds.Add("Knife_unequip",3f);
		
		CurrentState = "Idle_unarmed";
		CallbackFunction = new FuncRef();
		
		// Methods
		SetAnimation("Idle_unarmed");
		Connect("animation_finished", this, nameof(AnimationEnded));
	}
	
	public bool SetAnimation(string AnimationName)
	{
		if (AnimationName == CurrentState)
		{
			Console.WriteLine("AnimationPlayer.cs -- WARNING: animation is already ", AnimationName);
			return true;
		}
			
		if (HasAnimation(AnimationName))
		{
			if (CurrentState != null)
				//var possible_animations = _states[CurrentState];
				if (_states[CurrentState].Contains(AnimationName))
				{
					CurrentState = AnimationName;
					Play(AnimationName, -1, _animationSpeeds[AnimationName]);
					return true;
				}
				else
				{
					Console.WriteLine("AnimationPlayer.cs -- WARNING: Cannot change to ", AnimationName);
					return false;
				}
		}
		else
		{
			CurrentState = AnimationName;
			Play(AnimationName, -1, _animationSpeeds[AnimationName]);
			return true;
		}

		return false;
	}
	
	public void AnimationEnded(string AnimName)
	{
		switch (CurrentState)
		{
			case "Idle_unarmed":
				break;
			case "Knife_equip":
				SetAnimation("Knife_idle");
				break;
			case "Knife_idle":
				break;
			case "Knife_fire":
				SetAnimation("Knife_idle");
				break;
			case "Knife_unequip":
				SetAnimation("Idle_unarmed");
				break;
			case "Pistol_equip":
				SetAnimation("Pistol_idle");
				break;
			case "Pistol_idle":
				break;
			case "Pistol_fire":
				SetAnimation("Pistol_idle");
				break;
			case "Pistol_unequip":
				SetAnimation("Idle_unarmed");
				break;
			case "Pistol_reload":
				SetAnimation("Pistol_idle");
				break;
			case "Rifle_equip":
				SetAnimation("Rifle_idle");
				break;
			case "Rifle_idle":
				break;
			case "Rifle_fire":
				SetAnimation("Rifle_idle");
				break;
			case "Rifle_unequip":
				SetAnimation("Idle_unarmed");
				break;
			case "Rifle_reload":
				SetAnimation("Rifle_idle");
				break;
		}
	}
	
	public void AnimationCallback()
	{
		if (CallbackFunction == null)
			GD.Print("AnimationPlayer.cs -- WARNING: No callback function for the animation to call!");
		else
			CallbackFunction.CallFunc();
	}
}