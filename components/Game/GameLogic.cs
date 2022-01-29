using Godot;

using static Player;

public class GameLogic : Node
{
	private Button previousAttackTartget;

	private Button previousAttackType;

	private Button previousBlockTarget;

	private Button previousBlockTarget2;

	private Player Player1;

	private Player Player2;

	public override void _Ready()
	{
		Player1 = GetParent().GetNode("Player1") as Player;
		Player2 = GetParent().GetNode("Player2") as Player;
	}

	public void _on_SubmitAttackAction_clicked(Button instance)
	{
		Player1.AttackSequence();
		Player2.RecieveDamage(7);
	}

	private void SetAttackTarget(Button instance, AttackTarget target)
	{
		Player1.CurrentAttackTarget = target;
		if (previousAttackTartget != null)
			previousAttackTartget.Call("set_filled", false);
		instance.Call("set_filled", true);
		previousAttackTartget = instance;
	}

	private void SetAttackType(Button instance, AttackType type)
	{
		if (previousAttackType == instance || previousAttackType == instance)
			return;
		Player1.CurrentAttackType = type;
		if (previousAttackType != null)
			previousAttackType.Call("set_filled", false);
		instance.Call("set_filled", true);
		previousAttackType = instance;
	}

	private void SetBlockTarget(Button instance, AttackTarget type)
	{
		if (previousBlockTarget == null)
		{
			GD.Print("previousBlockTarget = instance");
			instance.Call("set_filled", true);
			previousBlockTarget = instance;
		}
		else if (previousBlockTarget2 == null && previousBlockTarget != instance
		)
		{
			GD.Print("previousBlockTarget2 = instance");
			instance.Call("set_filled", true);
			previousBlockTarget2 = instance;
		}
		else
		{
			if (instance == previousBlockTarget)
			{
				instance.Call("set_filled", false);
				GD.Print("previousBlockTarget = null");
				previousBlockTarget = null;
			}

			if (instance == previousBlockTarget2)
			{
				GD.Print("previousBlockTarget2 = null");
				instance.Call("set_filled", false);
				previousBlockTarget2 = null;
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
