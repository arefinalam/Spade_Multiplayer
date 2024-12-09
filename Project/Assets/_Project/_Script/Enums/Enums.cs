public enum GameMode { Dev, QA, Prod }
public enum TransactionTypes {  Purchase, Withdraw }
public enum PaymentMethod { None, Card, Bank, PayPal}
public enum WithdrawMethod { None, Card, Bank, PayPal}
public enum GameState
{
    Initialization,
    Bidding,
    TrickPlaying,
    Scoring,
    EndGame
}

public enum Suit
{
    None,
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

public enum Rank
{
    None,
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}