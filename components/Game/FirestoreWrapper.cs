using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Godot;
using Newtonsoft.Json;

public class FirestoreWrapper : Node
{
	private static string
		rootUrl =
			"https://neverland-ggj2022-default-rtdb.europe-west1.firebasedatabase.app";

	private static string[]
		headerz = new string[] { "Content-Type: application/json" };

	[Export]
	private int RefreshInterval = 2;

	private HTTPRequest refreshRequest;

	private HTTPRequest actionRequest;

	float ellapsedTime = 0;

	private string currentGameId;

	public delegate void TurnReceived(Turn turn);

	public delegate void GameStateUpdate(GameState game);

	public TurnReceived TurnProcessor;

	public GameStateUpdate GameStateProcessor;

	private GameState currentGameState;

	private PlayerState localPlayerState;

	private string localPlayerIndex;

	public override void _Ready()
	{
		refreshRequest = GetNode<HTTPRequest>("RefreshRequest");

		// refreshRequest.Connect("request_completed", this, "OnRrefreshResponse");
		actionRequest = GetNode<HTTPRequest>("ActionRequest");
		// actionRequest.Connect("request_completed", this, "OnActionResponse");

		Listener = GetNode<FirebaseDatabase>("");//
	}

	public override void _Process(float delta)
	{
		base._Process(delta);

		ellapsedTime += delta;

		if (
			ellapsedTime > RefreshInterval &&
			!string.IsNullOrEmpty(currentGameId)
		)
		{
			ellapsedTime = 0;

			refreshRequest
				.Request($"{rootUrl}/games/{currentGameId}.json",
				headerz,
				true,
				HTTPClient.Method.Get);
		}
	}

	public void StartListener(
		string gameId,
		string playerIndex,
		string localPlayerName
	)
	{
		currentGameId = gameId;
		localPlayerIndex = playerIndex;
		localPlayerState = new PlayerState() { Name = localPlayerName };
	}

	internal void ResetTurn()
	{
		currentGameState.Player1.Ready = false;
		currentGameState.Player2.Ready = false;

		var query = JsonConvert.SerializeObject(currentGameState);

		actionRequest
			.Request($"{rootUrl}/games/{currentGameId}.json",
			headerz,
			true,
			HTTPClient.Method.Put,
			query);
	}

	public void SendTurn(Turn turn)
	{
		turn.ProcessedByEnemy = false;
		localPlayerState.Turn = turn;
		localPlayerState.Ready = true;
		var query = JsonConvert.SerializeObject(localPlayerState);
		GD.Print (query);

		actionRequest
			.Request($"{rootUrl}/games/{currentGameId}/{localPlayerIndex}.json",
			headerz,
			true,
			HTTPClient.Method.Put,
			query);
	}

	private void OnRrefreshResponse(
		int result,
		int response_code,
		string[] headers,
		byte[] body
	)
	{
		var responseBody = Encoding.UTF8.GetString(body);

		currentGameState =
			JsonConvert.DeserializeObject<GameState>(responseBody);
		currentGameState.LocalPlayerIndex = localPlayerIndex;
		GameStateProcessor (currentGameState);
	}

	private void _on_ActionRequest_request_completed(
		int result,
		int response_code,
		string[] headers,
		byte[] body
	)
	{
		GD.Print($"OnActionResponse {Encoding.UTF8.GetString(body)}");
	}
}
