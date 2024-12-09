using System;


public class TransactionData
{
    public string TransactionId { get; set; } // Unique identifier for the transaction
    public string UserId { get; set; } // User ID of the user making the transaction
    public string Email { get; set; } // User ID of the user making the transaction

    public string Type { get; set; } // Type of transaction ("purchase" or "withdraw")
    public string Method { get; set; } // Transaction Method { None, Card, Bank, PayPal}
    public float Amount { get; set; } // Amount of money involved in the transaction
    public DateTime CreatedAt { get; set; } // Transaction date and time
    public string Status { get; set; } // Status of the transaction (e.g., "pending", "completed")
}
