using Microsoft.AspNetCore.Identity;

namespace DatabaseLayer.DbTables;

public class Dj
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public virtual IdentityUser? User { get; set; } 
}