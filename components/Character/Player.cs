using System;
using Godot;

public class Player : AnimatedSprite
{
	public double MaxHealth = 100;

	public double Health = 100;

	private ProgressBar healthBar;

	public override void _Ready()
	{
		healthBar = GetNode<ProgressBar>("HealthBar");

		healthBar.MaxValue = MaxHealth;
		healthBar.Value = MaxHealth;

		base.Connect("animation_finished", this, "AnimationFinished");
	}

	public void RecieveDamage(double ammount)
	{
		Health -= ammount;
		GD.Print($"dmg{ammount}/{Health}");
		healthBar.Value = Health;
	}

	public void AnimationFinished()
	{
		Play("idle");
	}
}
