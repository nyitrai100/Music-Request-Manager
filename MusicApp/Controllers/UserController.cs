using DatabaseLayer;
using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MusicApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("songs")]
    public async Task<ActionResult<IEnumerable<Songs>>> GetUserCurrentClubSongs([FromQuery] string userId, int? clubId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("UserId is required");
        
        var allDjSets = await _context.DjSets.ToListAsync();
        var timeNow = DateTime.Now;
        var currentDjSet = allDjSets.FirstOrDefault(x => x.ClubId == clubId  && x.PerformanceTimeStarts <= timeNow && x.PerformanceTimeEnds >= timeNow);
        if (currentDjSet == null)
        {
            return BadRequest("Dj are not currently performing in this club.");
        }
        
        var songs = await _context.Songs
            .Include(x=> x.Status)
            .Include(x => x.DjSets)
            .Where(x => x.UserId.ToLower() == userId.ToLower() && x.ClubId == clubId && x.RequestedTime >= currentDjSet.PerformanceTimeStarts && x.RequestedTime <= currentDjSet.PerformanceTimeEnds)
            .ToListAsync();
        
        return Ok(songs);
    }

    [HttpGet("clubs")]
    public async Task<ActionResult<IEnumerable<Clubs>>> GetAllClubs()
    {
        var clubs = await _context.Clubs.ToListAsync();

        return Ok(clubs);
    }
    
    [HttpPost("songs")]
    public async Task<ActionResult> RequestSong([FromBody] Songs song)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet("djSets")]
    public async Task<ActionResult<DjSets>> GetClubsDjSet([FromQuery] int clubId)
    {
        var djSets = await _context.DjSets.ToListAsync();
        var timeNow = DateTime.Now;
        var djSet = djSets.FirstOrDefault(x => x.ClubId == clubId && x.PerformanceTimeStarts <= timeNow && x.PerformanceTimeEnds >= timeNow);
        if (djSet == null)
            return BadRequest("Couldn't find dj for the club"); 
    
        return Ok(djSet);
    }
    
    [HttpGet("currentDj")]
    public async Task<ActionResult<Dj>> GetCurrentDj([FromQuery] string djId)
    {
        if (!int.TryParse(djId, out var djIdInt))
            return BadRequest("Invalid DJ ID.");

        var dj = await _context.Dj
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == djIdInt);

        if (dj == null)
            return NotFound("DJ not found.");

        return Ok(dj.User?.UserName);
    }

    [HttpGet("currentClub")]
    public async Task<ActionResult<Clubs>> GetCurrentClub([FromQuery] string clubId)
    {
        if (!int.TryParse(clubId, out var clubIdInt))
            return BadRequest("Invalid Club ID.");
        
        var club = await _context.Clubs.FirstOrDefaultAsync(x => x.Id == clubIdInt);

        if (club == null)
        {
            return NotFound("Club not found");
        }
        return Ok(club.ClubName);
    }
    
    [HttpGet("currentDjSet")]
    public async Task<ActionResult<DjSets>> GetCurrentDjSet([FromQuery] int clubId)
    {
        var timeNow = DateTime.Now;
        var currentDjSet = await _context.DjSets
            .FirstOrDefaultAsync(x => x.ClubId == clubId && 
                                      x.PerformanceTimeStarts <= timeNow && 
                                      x.PerformanceTimeEnds >= timeNow);

        if (currentDjSet == null)
            return NotFound("No DJ currently performing at this club.");

        return Ok(currentDjSet);
    }


    [HttpGet("pastSongs")]
    public async Task<ActionResult<IEnumerable<Songs>>> GetUserPastClubSongs([FromQuery] string userId, int? clubId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("UserId is required");
        
        var songs = await _context.Songs
            .Where(x => x.UserId.ToLower() == userId.ToLower() && x.ClubId == clubId)
            .ToListAsync();

        return Ok(songs);
    }
}