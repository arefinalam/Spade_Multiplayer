using System.Collections.Generic;
using System;

[Serializable]
public class UserGameData
{
    public string UserId;// { get; set; } // Unique identifier for the user
    public string DisplayName;// { get; set; } // User's display name
    public string Email; // User's email address
    public bool IsConnected;// { get; set; } // Is player online
    public bool IsPlaying; // Is player currently playing
    public int TotalMatches; // Total number of matches played
    public int Wins; // Number of matches won
    public int Losses; // Number of matches lost
    public int TotalCurrency; // Total currency earned or owned
    public bool IsLoggedIn;
    public bool IsVarified; // Total currency earned or owned
    public bool IsActive; // Total currency earned or owned

    public  UserGameData(string name, string email, string userid)
    {
        this.UserId = userid;
        this.DisplayName = name;
        this.Email = email;
        this.IsConnected = true;
        this.IsPlaying = false;
        this.TotalMatches = 0;
        this.Wins = 0;
        this.Losses = 0;
        this.TotalCurrency = 0;
        this.IsLoggedIn = true;
        this.IsVarified = false;
        this.IsActive = true;
    }
}
