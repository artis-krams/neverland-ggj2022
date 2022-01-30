using System;
using System.IO;
using System.Text;
using Godot;
using Newtonsoft.Json;

using static Player;

public class GameLogic : Control
{
	private Button previousAttackTartget;

	private Button previousAttackType;

	private Button previousBlockTarget;

	private Button previousBlockTarget2;

	private Player LocalPlayer;

	private Player EnemyPlayer;

	private HTTPRequest requestNode;

	private string currentGameId;

	private static Random random = new Random();

	private static string
		rootUrl =
			"https://neverland-ggj2022-default-rtdb.europe-west1.firebasedatabase.app/games/";

	public override void _Ready()
	{
		LocalPlayer = GetNode("Player1") as Player;
		EnemyPlayer = GetNode("Player2") as Player;

		requestNode = GetNode<HTTPRequest>("HTTPRequest");
		requestNode.Connect("request_completed", this, "OnRequestCompleted");

		headerz = new string[] { "Content-Type: application/json" };
		botTimer = GetNode<Timer>("Timer");
		botTimer.Connect("timeout", this, "BotAttack");
	}

	float ellapsedTime = 0;

	private string[] headerz;

	private Timer botTimer;

	public override void _Process(float delta)
	{
		base._Process(delta);
		ellapsedTime += delta;
		if (ellapsedTime > 2)
		{
			ellapsedTime = 0;
			RefreshGameStatus();
		}
	}

	public void OnRequestCompleted(
		int result,
		int response_code,
		string[] headers,
		byte[] body
	)
	{
		var responseBody = Encoding.UTF8.GetString(body);
		GD.Print (responseBody);
	}

	private void RefreshGameStatus()
	{
		// requestNode
		// 	.Request($"{rootUrl}{currentGameId}.json",
		// 	headerz,
		// 	true,
		// 	HTTPClient.Method.Get);
	}

	public void StartGame(string gameId)
	{
		currentGameId = gameId;
		//GD.Print($"starting game {gameId}");
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
		botTimer.Start(1);
	}

	public void BotAttack()
	{
		if(EnemyPlayer.dieded)
		return;
		GD.Print("bot attack");

		var targets = Enum.GetValues(typeof (AttackTarget));
		AttackTarget randomTarget =
			(AttackTarget) targets.GetValue(random.Next(targets.Length));

		EnemyPlayer.CurrentAttackTarget = randomTarget;

		randomTarget =
			(AttackTarget) targets.GetValue(random.Next(targets.Length));
		EnemyPlayer.CurrentBlockTarget = randomTarget;

		randomTarget =
			(AttackTarget) targets.GetValue(random.Next(targets.Length));
		EnemyPlayer.CurrentBlockTarget2 = randomTarget;

		randomTarget =
			(AttackTarget) targets.GetValue(random.Next(targets.Length));

		var types = Enum.GetValues(typeof (AttackType));
		AttackType randomType =
			(AttackType) types.GetValue(random.Next(types.Length));
			EnemyPlayer.CurrentAttackType = randomType;

		randomType =
			(AttackType) types.GetValue(random.Next(types.Length));
		LocalPlayer.RecieveDamage (randomTarget, randomType);

		botTimer.Stop();

		
		EnemyPlayer.AttackSequence();
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
