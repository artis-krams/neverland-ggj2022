using System;
using System.Text;
using Godot;
using Newtonsoft.Json;

using static FirestoreWrapper;
using static Player;

public class GameLogic : Control
{
	private Button previousAttackTartget;

	private Button previousAttackType;

	private Button previousBlockTarget;

	private Button previousBlockTarget2;

	private Player LocalPlayer;

	private Player EnemyPlayer;

	private Control loadingOverlay;

	private FirestoreWrapper requestNode;

	private static Random random = new Random();

	private static string
		rootUrl =
			"https://neverland-ggj2022-default-rtdb.europe-west1.firebasedatabase.app/games";

	private Timer botTimer;

	private Timer responseTimer;

	public override void _Ready()
	{
		LocalPlayer = GetNode("Player1") as Player;
		EnemyPlayer = GetNode("Player2") as Player;
		loadingOverlay = GetNode<Control>("LoadingOverlay");

		EnemyPlayer.NameLabel.Align = Label.AlignEnum.Right;

		requestNode = GetNode<FirestoreWrapper>("FirestoreWrapper");
		requestNode.TurnProcessor = new TurnReceived(ProcessTurn);
		requestNode.GameStateProcessor = new GameStateUpdate(ProcessGameState);

		botTimer = GetNode<Timer>("Timer");
		botTimer.Connect("timeout", this, "BotAttack");

		responseTimer = GetNode<Timer>("EnemyTimer");
		responseTimer.Connect("timeout", this, "EnemyAttack");
	}

	public void ProcessTurn(Turn turn)
	{
		GD.Print (turn);
	}

	public void ProcessGameState(GameState game)
	{
		if (game.Player2 == null || game.Player1 == null) return;

		if (!game.Player1.Ready || !game.Player2.Ready) return;

		loadingOverlay.Hide();

		PlayerState enemyState;
		if (game.LocalPlayerIndex == "Player1")
		{
			enemyState = game.Player2;
		}
		else
		{
			enemyState = game.Player1;
		}

		EnemyPlayer.CurrentBlockTarget = enemyState.Turn.defend1;
		EnemyPlayer.CurrentBlockTarget2 = enemyState.Turn.defend2;
		EnemyPlayer.CurrentAttackTarget = enemyState.Turn.attack;
		EnemyPlayer.CurrentAttackType = enemyState.Turn.attack_type;

		// todo wait for network player
		LocalPlayer.AttackSequence();

		EnemyPlayer
			.RecieveDamage(LocalPlayer.CurrentAttackTarget,
			LocalPlayer.CurrentAttackType);

		if (string.IsNullOrEmpty(EnemyPlayer.DisplayName))
			EnemyPlayer.DisplayName = game.Player2.Name;

		requestNode.ResetTurn();
	}

	public void EnemyAttack()
	{
		EnemyPlayer.AttackSequence();

		LocalPlayer
			.RecieveDamage(EnemyPlayer.CurrentAttackTarget,
			EnemyPlayer.CurrentAttackType);
	}

	public void StartGame(
		string gameId,
		string localPlayerName,
		string enemyPlayerName,
		string localPlayerIndex
	)
	{
		LocalPlayer.DisplayName = localPlayerName;
		GD.Print($"init game {gameId}");

		requestNode.StartListener (gameId, localPlayerIndex, localPlayerName);

		if (string.IsNullOrEmpty(enemyPlayerName))
		{
			loadingOverlay.Show();
			GD.Print($"waiting for enemy");
			return;
		}

		GD.Print($"enemy {enemyPlayerName} ready");
		EnemyPlayer.DisplayName = enemyPlayerName;
		loadingOverlay.Hide();
	}

	public void _on_SubmitAttackAction_clicked(Button instance)
	{
		requestNode
			.SendTurn(new Turn()
			{
				attack = LocalPlayer.CurrentAttackTarget,
				attack_type = LocalPlayer.CurrentAttackType,
				defend1 = LocalPlayer.CurrentBlockTarget,
				defend2 = LocalPlayer.CurrentBlockTarget2
			});
	}

	public void BotAttack()
	{
		if (EnemyPlayer.dieded) return;
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

		randomType = (AttackType) types.GetValue(random.Next(types.Length));
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
