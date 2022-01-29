using System;
using Godot;

public class Player : Node
{
    public double MaxHealth = 100;

    public double Health = 100;

    private ProgressBar healthBar;

    public override void _Ready()
    {
        healthBar = GetNode<ProgressBar>("HealthBar");

        // healthBar.MaxValue = MaxHealth;
        healthBar.Value = MaxHealth;
    }

    public void RecieveDamage(double ammount)
    {
        Health -= ammount;
        GD.Print($"dmg{ammount}/{Health}");
        healthBar.Value = Health;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}
