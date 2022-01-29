using System;
using System.Collections.Generic;
using Godot;

[Tool]
public class Player : AnimatedSprite
{
	public double MaxHealth = 100;

	public double Health = 100;

	public AttackTarget CurrentAttackTarget;

	public AttackTarget CurrentBlockTarget;

	public AttackTarget CurrentBlockTarget2;

	public AttackType CurrentAttackType;

	private ProgressBar healthBar;

	private AudioStreamPlayer audio;

	private List<AudioStream> blockSounds;

	private List<AudioStream> attackSounds;

	private static Random random = new Random();

	private static string soundsFolder = "res://resources/sounds/";

	public override void _Ready()
	{
		healthBar = GetNode<ProgressBar>("HealthBar");
		audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		base.Connect("animation_finished", this, "AnimationFinished");

		blockSounds = new List<AudioStream>();
		blockSounds.Add(GD.Load<AudioStream>($"{soundsFolder}block1.wav"));
		blockSounds.Add(GD.Load<AudioStream>($"{soundsFolder}block2.wav"));
		blockSounds.Add(GD.Load<AudioStream>($"{soundsFolder}block3.wav"));
		blockSounds.Add(GD.Load<AudioStream>($"{soundsFolder}clank.wav"));

		attackSounds = new List<AudioStream>();
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strike1.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strike2.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strike3.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strike4.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strike1.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strikea.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strikeb.wav"));
		attackSounds.Add(GD.Load<AudioStream>($"{soundsFolder}strikec.wav"));
	}

	public void RecieveDamage(AttackTarget target, AttackType type)
	{
		audio.Stream = blockSounds[random.Next(blockSounds.Count)];
		audio.VolumeDb = 0;
		audio.Play();

		string blockTargetAnimName =
			GetAttackTargetAnimName(CurrentBlockTarget);

		Play($"{blockTargetAnimName} block");

		Health -= GetDamageAmmount(target, type);
		healthBar.Value = Health;

		GD.Print($"def {CurrentBlockTarget} {CurrentBlockTarget2}");
		GD.Print($"dmg {GetDamageAmmount(target, type)}");
	}

	private double GetDamageAmmount(AttackTarget attackTarget, AttackType attackType)
	{
		double baseDamage = 10;

		switch (attackTarget)
		{
			case (AttackTarget.Head):
				baseDamage *= 1.5;
				break;
			case (AttackTarget.Torso):
				baseDamage *= 1.1;
				break;
			case (AttackTarget.Legs):
				baseDamage *= 0.9;
				break;
		}

		if (CurrentBlockTarget == attackTarget || CurrentBlockTarget2 == attackTarget)
		{
			if (attackType == AttackType.Slash) baseDamage *= 0.5;

			if (attackType == AttackType.Stab) baseDamage *= 0.7;
		}
		else
		{
			if (attackType == AttackType.Slash) baseDamage *= 1.5;

			if (attackType == AttackType.Stab) baseDamage *= 1.2;
		}

		return baseDamage;
	}

	public void AttackSequence()
	{
		audio.Stream = attackSounds[random.Next(attackSounds.Count)];
		audio.VolumeDb = -14;
		audio.Play();

		string attackTypeAnimName = GetAttackTypeAnimName(CurrentAttackType);

		string attackTargetAnimName =
			GetAttackTargetAnimName(CurrentAttackTarget);

		Play($"{attackTargetAnimName} {attackTypeAnimName}");
	}

	public void AnimationFinished()
	{
		Play("idle");
	}

	private string GetAttackTypeAnimName(AttackType target)
	{
		switch (target)
		{
			case (AttackType.Slash):
				return "slash";
			case (AttackType.Stab):
				return "stab";
		}

		return String.Empty;
	}

	private string GetAttackTargetAnimName(AttackTarget target)
	{
		switch (target)
		{
			case (AttackTarget.Head):
				return "up";
			case (AttackTarget.Torso):
				return "mid";
			case (AttackTarget.Legs):
				return "down";
			default:
				return String.Empty;
		}
	}

	public enum AttackType
	{
		Slash = 0,
		Stab = 1
	}

	public enum AttackTarget
	{
		None = -1,
		Head = 0,
		Torso = 1,
		Legs = 2
	}
}
