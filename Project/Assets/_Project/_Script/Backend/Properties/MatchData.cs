using System;
using System.Collections.Generic;

public class MatchData
{
    public string MatchId { get; set; } // Unique identifier for the match

    public DateTime CreatedAt { get; set; } // Match creation date and time

    public List<string> Players { get; set; } // List of user IDs participating in the match

    public long BetLevel { get; set; } // Bet level for the match

    public string Winner { get; set; } // User ID of the match winner

    public string Status { get; set; } // Current status of the match (e.g., "ongoing", "completed")
}
