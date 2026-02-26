namespace DatabaseLayer.DbTables;

public class Clubs
{
    public int Id { get; set; }
    public required string ClubName { get; set; }
    public string Location { get; set; }
    public int Floor { get; set; }
    public virtual ICollection<Songs>? Songs { get; set; }
}