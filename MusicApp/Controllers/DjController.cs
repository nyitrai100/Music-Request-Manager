using DatabaseLayer;
using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DjController : ControllerBase
{
    private readonly AppDbContext _context;

    public DjController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("songs")]
    public async Task<ActionResult<IEnumerable<Songs>>> GetDjSongs([FromQuery] string djId, [FromQuery] string timeScope)
    {
        var dj = await _context.Dj.FirstOrDefaultAsync(x => x.UserId == djId);
        if (dj == null)
            return BadRequest("You are not currently assigned to any DJ profile.");

        var timeNow = DateTime.Now;
        var djSetQuery = _context.DjSets.Where(x => x.DjId == dj.Id);

        List<DjSets> pastSets = new();
        DjSets? targetSet = null;
        
        switch (timeScope.ToLower())
        {
            case "past":
                pastSets = await djSetQuery
                    .Where(x => x.PerformanceTimeEnds <= timeNow)
                    .ToListAsync();

                if (!pastSets.Any())
                    return BadRequest("You haven't performed in the past at any set.");

                break;

            case "future":
                targetSet = await djSetQuery
                    .Where(x => x.PerformanceTimeStarts >= timeNow)
                    .OrderBy(x => x.PerformanceTimeStarts)
                    .FirstOrDefaultAsync();

                if (targetSet == null)
                    return BadRequest("You don't have any upcoming performances.");

                break;

            default:
                targetSet = await djSetQuery
                    .FirstOrDefaultAsync(x =>
                        x.PerformanceTimeStarts <= timeNow &&
                        x.PerformanceTimeEnds >= timeNow);

                if (targetSet == null)
                    return BadRequest("You are not currently performing at any set.");

                break;
        }
        
        if (timeScope.Equals("future", StringComparison.OrdinalIgnoreCase))
        {
            var placeholderSong = new Songs
            {
                Id = 0,
                ClubId = targetSet!.ClubId,
                Author = null,
                Title = $"Upcoming performance at {targetSet.ClubId}",
                StatusId = 0,
                DjSetId = targetSet.DjId,
                RequestedTime = targetSet.PerformanceTimeStarts,
                UserId = dj.UserId,
                DjSets = targetSet
            };

            return Ok(new List<Songs> { placeholderSong });
        }
        
        if (timeScope.Equals("past", StringComparison.OrdinalIgnoreCase))
        {
            var songs = await _context.Songs
                .Include(x => x.Status)
                .Where(x => pastSets.Select(s => s.Id).Contains(x.DjSetId))
                .ToListAsync();

            foreach (var song in songs)
                song.DjSets = pastSets.First(s => s.Id == song.DjSetId);

            return Ok(songs);
        }
        
        var currentSongs = await _context.Songs
            .Include(x => x.Status)
            .Where(x =>
                x.DjSetId == targetSet!.Id &&
                x.ClubId == targetSet.ClubId &&
                x.RequestedTime >= targetSet.PerformanceTimeStarts &&
                x.RequestedTime <= targetSet.PerformanceTimeEnds)
            .ToListAsync();

        foreach (var song in currentSongs)
            song.DjSets = targetSet;

        return Ok(currentSongs);
    }


    [HttpPost("updateStatus")]
    public async Task<ActionResult> UpdateStatus([FromBody] Songs song)
    {
        var selectedSong = await _context.Songs
            .FirstOrDefaultAsync(s => s.Id == song.Id);

        if (selectedSong == null)
            return NotFound("Song not found");

        selectedSong.StatusId = song.StatusId;
        _context.Songs.Update(selectedSong);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Song status updated successfully" });
    }
}