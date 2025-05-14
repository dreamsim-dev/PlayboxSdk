using System;

public class PurchaseValidator
{
    private string productId;
    private string ticketId;
    private string _saveIndentifier;
    
    public Action<bool> OnCallback;

    public string ProductId
    {
        get => productId;
        set => productId = value;
    }

    public string SaveIndentifier
    {
        get => _saveIndentifier;
        set => _saveIndentifier = value;
    }

    public string TicketId
    {
        get => ticketId;
        set => ticketId = value;
    }
}