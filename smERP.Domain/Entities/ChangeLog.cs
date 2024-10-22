namespace smERP.Domain.Entities;

public class ChangeLog
{
    public int Id { get; set; }
    public string EntityName { get; set; }
    public string Action { get; set; }
    public string Changes { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public int EntityId { get; set; }
}

