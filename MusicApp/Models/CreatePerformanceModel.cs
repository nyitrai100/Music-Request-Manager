using DatabaseLayer.DbTables;

namespace MusicApp.Models;

public class CreatePerformanceModel
{
    public int DjId { get; set; }
    public int ClubId { get; set; }
    public DateTime PerformanceTimeStarts { get; set; }
    public DateTime PerformanceTimeEnds { get; set; }
    public List<Dj> Dj { get; set; } = new();
    public List<Clubs> Clubs { get; set; } = new();
}