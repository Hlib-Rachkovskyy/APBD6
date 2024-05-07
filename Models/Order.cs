namespace APBD6.Models;

public class Order
{
    public int IdOrder { get; set; }
    private int IdProduct { get; set; }
    private int Amount { get; set; }
    private DateTime CreatedAt { get; set; }
    private DateTime FulfiledAt { get; set; }
}