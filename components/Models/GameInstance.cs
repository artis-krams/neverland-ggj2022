using System.Collections.Generic;

public class GameInstance
{
    public string player1 { get; set; }

    public string player2 { get; set; }

    public int participants { get; set; }

    public List<Turn> turns { get; set; }
}