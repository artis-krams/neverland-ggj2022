using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Newtonsoft.Json;

public class Root : Control
{
	string
		rootUrl =
			"https://neverland-ggj2022-default-rtdb.europe-west1.firebasedatabase.app/";

	private Node requestNode;

	public string UserName = "traveler";

	public string EnemyName = String.Empty;

	public override void _Ready()
	{
		requestNode = GetNode("HTTPRequest");
		requestNode.Connect("request_completed", this, "OnRequestCompleted");
	}

	private void Connect_Button_pressed()
	{
		HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
		httpRequest.Request(rootUrl + "games.json?orderBy=\"$key\"&limitToLast=1");
		UserName = GetNode("Menu").GetNode<TextEdit>("TextEdit").Text;
	}

	public void OnRequestCompleted(
		int result,
		int response_code,
		string[] headers,
		byte[] body
	)
	{
		var responseBody = Encoding.UTF8.GetString(body);

		try
		{
			Dictionary<string, GameState> games =
				JsonConvert
					.DeserializeObject
					<Dictionary<string, GameState>>(responseBody);

			var lastGame = games.Last();

			if (lastGame.Value.Player2 == null)
			{
				EnemyName = lastGame.Value.Player1.Name;

				requestNode
					.Disconnect("request_completed",
					this,
					"OnRequestCompleted");

				lastGame.Value.Player2 = new PlayerState() { Name = UserName };

				string query =
					"{\"" +
					lastGame.Key +
					"\":" +
					JsonConvert.SerializeObject(lastGame.Value) +
					"}";

				HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
				string[] headerz =
					new string[] { "Content-Type: application/json" };
				httpRequest
					.Request(rootUrl + "games.json",
					headerz,
					true,
					HTTPClient.Method.Patch,
					query);
				GD.Print("joining game");
				StartGame(lastGame.Key, "Player2");
			}
			else
			{
				var query =
					JsonConvert
						.SerializeObject(new GameState()
						{ Player1 = new PlayerState() { Name = UserName } });

				HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
				string[] headerz =
					new string[] { "Content-Type: application/json" };
				httpRequest
					.Request(rootUrl + "games.json",
					headerz,
					true,
					HTTPClient.Method.Post,
					query);
				GD.Print("creating new game");
			}
		}
		catch (Exception ex)
		{
			GD.Print (responseBody);
			GD.Print(ex.Message);

			var newGameName =
				JsonConvert.DeserializeObject<NameOfTheGame>(responseBody);
			StartGame(newGameName.name, "Player1");
			return;
		}
	}

	void StartGame(string gameId, string localPlayerIndex)
	{
		GetNode<Control>("Menu").Hide();
		var gamenode = GetNode<GameLogic>("GameScene");
		gamenode.Show();
		gamenode.StartGame (gameId, UserName, EnemyName, localPlayerIndex);
	}
}
