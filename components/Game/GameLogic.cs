using Godot;

using static Player;

public class GameLogic : Node
{
	private Button previousAttackTartget;

	private Button previousAttackType;

	private Button previousBlockTarget;

	private Button previousBlockTarget2;

	private Player LocalPlayer;

	private Player EnemyPlayer;

	public override void _Ready()
	{
		LocalPlayer = GetParent().GetNode("Player1") as Player;
		EnemyPlayer = GetParent().GetNode("Player2") as Player;
	}

	public void _on_SubmitAttackAction_clicked(Button instance)
	{
		EnemyPlayer.CurrentBlockTarget = LocalPlayer.CurrentBlockTarget;
		EnemyPlayer.CurrentBlockTarget2 = LocalPlayer.CurrentBlockTarget2;
		// todo wait for network player

		LocalPlayer.AttackSequence();
		EnemyPlayer
			.RecieveDamage(LocalPlayer.CurrentAttackTarget,
			LocalPlayer.CurrentAttackType);
	}

	private void SetAttackTarget(Button instance, AttackTarget target)
	{
		LocalPlayer.CurrentAttackTarget = target;
		if (previousAttackTartget != null)
			previousAttackTartget.Call("set_filled", false);
		instance.Call("set_filled", true);
		previousAttackTartget = instance;
	}

	private void SetAttackType(Button instance, AttackType type)
	{
		if (previousAttackType == instance || previousAttackType == instance)
			return;
		LocalPlayer.CurrentAttackType = type;
		if (previousAttackType != null)
			previousAttackType.Call("set_filled", false);
		instance.Call("set_filled", true);
		previousAttackType = instance;
	}

	private void SetBlockTarget(Button instance, AttackTarget target)
	{
		if (previousBlockTarget == null)
		{
			GD.Print("previousBlockTarget = instance");
			instance.Call("set_filled", true);
			previousBlockTarget = instance;
			LocalPlayer.CurrentBlockTarget = target;
		}
		else if (previousBlockTarget2 == null && previousBlockTarget != instance
		)
		{
			GD.Print("previousBlockTarget2 = instance");
			instance.Call("set_filled", true);
			previousBlockTarget2 = instance;
			LocalPlayer.CurrentBlockTarget2 = target;
		}
		else
		{
			if (instance == previousBlockTarget)
			{
				instance.Call("set_filled", false);
				GD.Print("previousBlockTarget = null");
				previousBlockTarget = null;
				LocalPlayer.CurrentBlockTarget = AttackTarget.None;
			}

			if (instance == previousBlockTarget2)
			{
				GD.Print("previousBlockTarget2 = null");
				instance.Call("set_filled", false);
				previousBlockTarget2 = null;
				LocalPlayer.CurrentBlockTarget2 = AttackTarget.None;
			}
		}
	}

	public void _on_AttackHeadAction_clicked(Button instance)
	{
		SetAttackTarget(instance, AttackTarget.Head);
	}

	public void _on_AttackTorsoAction_clicked(Button instance)
	{
		SetAttackTarget(instance, AttackTarget.Torso);
	}

	public void _on_AttackLegsAction_clicked(Button instance)
	{
		SetAttackTarget(instance, AttackTarget.Legs);
	}

	public void _on_AttackSlashAction_clicked(Button instance)
	{
		SetAttackType(instance, AttackType.Slash);
	}

	public void _on_AttackStabAction_clicked(Button instance)
	{
		SetAttackType(instance, AttackType.Stab);
	}

	public void _on_BlockHeadAction_clicked(Button instance)
	{
		SetBlockTarget(instance, AttackTarget.Head);
	}

	public void _on_BlockTorsoAction_clicked(Button instance)
	{
		SetBlockTarget(instance, AttackTarget.Torso);
	}

	public void _on_BlockLegsAction_clicked(Button instance)
	{
		SetBlockTarget(instance, AttackTarget.Legs);
	}
}
