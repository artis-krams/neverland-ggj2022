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
	
	public override void _Ready()
	{
		requestNode = GetNode("HTTPRequest");
		requestNode.Connect("request_completed", this, "OnRequestCompleted");

	}
	
	private void Connect_Button_pressed()
	{
			HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
			httpRequest.Request(rootUrl + "games.json");
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
			Dictionary<string, GameInstance> games =
				JsonConvert
					.DeserializeObject
					<Dictionary<string, GameInstance>>(responseBody);

			var lastGame = games.Last();

			if (lastGame.Value.participants == 1)
			{
				requestNode
					.Disconnect("request_completed",
					this,
					"OnRequestCompleted");

				lastGame.Value.participants = 2;
				lastGame.Value.player2 = UserName;

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
				StartGame(lastGame.Key);
			}
			else
			{
				var query =
					JsonConvert
						.SerializeObject(new GameInstance()
						{ player1 = UserName, participants = 1 });

				HTTPRequest httpRequest = GetNode<HTTPRequest>("HTTPRequest");
				string[] headerz =
					new string[] { "Content-Type: application/json" };
				httpRequest
					.Request(rootUrl + "games.json",
					headerz,
					true,
					HTTPClient.Method.Post,
					query);
			}
		}
		catch
		{
			var newGameName =
				JsonConvert.DeserializeObject<NameOfTheGame>(responseBody);
			StartGame(newGameName.name);
			return;
		}
	}

	void StartGame(string gameId)
	{
		GetNode<Control>("Menu").Hide();
		var gamenode = GetNode<GameLogic>("GameScene");
		gamenode.Show();
		gamenode.StartGame(gameId);
	}
}


