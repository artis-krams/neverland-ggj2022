using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameLogic : Node
{
	public enum AttackType
	{
		Slash = 0,
		Stab = 1
	}

	public enum AttackTarget
	{
		Head = 0,
		Torso = 1,
		Legs = 2
	}

	public AttackTarget CurrentAttackTarget;

	public AttackTarget CurrentBlockTarget;

	public AttackType CurrentAttackType;

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
		string attackTypeAnimName = String.Empty;

		switch (CurrentAttackType)
		{
			case (AttackType.Slash):
				attackTypeAnimName = "slash";
				break;
			case (AttackType.Stab):
				attackTypeAnimName = "stab";
				break;
			default:
				break;
		}

		string attackTargetAnimName =
			GetAttackTargetAnimName(CurrentAttackTarget);

		Player1.Play($"{attackTargetAnimName} {attackTypeAnimName}");

		string blockTargetAnimName =
			GetAttackTargetAnimName(CurrentBlockTarget);

		Player2.Play($"{blockTargetAnimName} block");

		Player1.RecieveDamage(23);
		Player2.RecieveDamage(27);
	}

	private void SetAttackTarget(Button instance, AttackTarget target)
	{
		CurrentAttackTarget = target;
		if (previousAttackTartget != null)
			previousAttackTartget.Call("set_filled", false);
		instance.Call("set_filled", true);
		previousAttackTartget = instance;
	}

	private void SetAttackType(Button instance, AttackType type)
	{
		if (previousAttackType == instance || previousAttackType == instance)
			return;
		CurrentAttackType = type;
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
