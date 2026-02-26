using Microsoft.AspNetCore.Identity;

namespace DatabaseLayer.DbTables;

public class Songs
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public string? Author { get; set; }
    public required string Title { get; set; }
    public int StatusId { get; set; }
    public int DjSetId { get; set; }
    public DateTime RequestedTime { get; set; }
    public required string UserId { get; set; }
    public virtual IdentityUser? User { get; set; } 
    public virtual Clubs? Club { get; set; }
    public virtual Status? Status { get; set; }
    public virtual DjSets? DjSets { get; set; }
}