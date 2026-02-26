using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DatabaseLayer.DbTables;

public class DjSets
{
    public int Id { get; set; }
    public int DjId { get; set; }
    public virtual Dj? Dj { get; set; }
    public int ClubId { get; set; }
    public virtual Clubs? Club { get; set; }
    public DateTime PerformanceTimeStarts { get; set; }
    public DateTime PerformanceTimeEnds { get; set; }
}