namespace Kt7Api.Models;

public class Booking
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public string CustomerName { get; set; } = "";
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
