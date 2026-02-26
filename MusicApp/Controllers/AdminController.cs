using DatabaseLayer;
using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Models;

namespace MusicApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet("allUsers")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        var users = _userManager.Users.ToList();

        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = string.Join(", ", roles)
            });
        }

        return Ok(userDtos);
    }
    
    [HttpGet("djSets")]
    public async Task<ActionResult<List<DjSets>>> GetAllDjSets()
    {
        var djSets = await _context.DjSets
            .Include(x => x.Club)
            .Include(y => y.Dj)
            .OrderByDescending(z => z.PerformanceTimeStarts)
            .ToListAsync();
        var users = await _userManager.Users.ToListAsync();
        
        var userDict = users.ToDictionary(u => u.Id, u => u);

        foreach (var djSet in djSets)
        {
            if (djSet.Dj?.UserId != null && userDict.TryGetValue(djSet.Dj.UserId, out var user))
                djSet.Dj.User = user;
        }
        
        return Ok(djSets);
        
    }
    
    [HttpGet("allDj")]
    public async Task<ActionResult<List<Dj>>> GetAllDjs()
    {
        var dj = await _context.Dj
            .Include(d => d.User)
            .ToListAsync();

        return Ok(dj);
    }

    [HttpGet("allClub")]
    public async Task<ActionResult<List<Clubs>>> GetAllClub()
    {
        
        var club = await _context.Clubs.ToListAsync();
        
        return Ok(club);
    }
    
    [HttpPost("createPerformance")]
    public async Task<IActionResult> CreatePerformance([FromBody] CreatePerformanceModel model)
    {
        if (model.PerformanceTimeEnds <= model.PerformanceTimeStarts)
            return BadRequest("End time must be after start time.");
    
        var djExists = await _context.Dj.AnyAsync(d => d.Id == model.DjId);
        if (!djExists)
            return BadRequest("Invalid DJ.");
    
        var clubExists = await _context.Clubs.AnyAsync(c => c.Id == model.ClubId);
        if (!clubExists)
            return BadRequest("Invalid club.");
        
        var clubConflict = await _context.DjSets.AnyAsync(p =>
            p.ClubId == model.ClubId &&
            p.PerformanceTimeStarts < model.PerformanceTimeEnds &&
            p.PerformanceTimeEnds > model.PerformanceTimeStarts
        );
        
        var djConflict = await _context.DjSets.AnyAsync(p =>
            p.DjId == model.DjId &&
            p.PerformanceTimeStarts < model.PerformanceTimeEnds &&
            p.PerformanceTimeEnds > model.PerformanceTimeStarts
        );
        
        if (clubConflict)
            return BadRequest("This club already has a performance during the selected time.");

        if (djConflict)
            return BadRequest("This DJ already has a performance during the selected time.");
        
        var lastId = await _context.DjSets.MaxAsync(x => (int?)x.Id) ?? 0;
        var nextId = lastId + 1;

        var djSet = new DjSets
        {
            Id = nextId,
            DjId = model.DjId,
            ClubId = model.ClubId,
            PerformanceTimeStarts = model.PerformanceTimeStarts,
            PerformanceTimeEnds = model.PerformanceTimeEnds
        };

        await _context.Database.OpenConnectionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DjSets ON");

            _context.DjSets.Add(djSet);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DjSets OFF");
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }

        return Ok(djSet);
    }

    
    [HttpDelete("delete/{djSetId:int}")]
    public async Task<IActionResult> DeletePerformance(int djSetId)
    {
        var djSet = await _context.DjSets
            .FirstOrDefaultAsync(x => x.Id ==djSetId);

        if (djSet == null)
            return NotFound("djSet not found");
        
        _context.DjSets.Remove(djSet);
        await _context.SaveChangesAsync();

        return Ok($"DJ set with ID {djSetId} deleted successfully.");
    }
    
    [HttpPut("editPerformance")]
    public async Task<IActionResult> EditPerformance([FromBody] EditPerformanceModel model)
    {
        if (model.PerformanceTimeEnds <= model.PerformanceTimeStarts)
            return BadRequest("End time must be after start time.");
        
        var djSet = await _context.DjSets.FirstOrDefaultAsync(d => d.Id == model.Id);
        if (djSet == null)
            return NotFound("Performance not found.");
        
        var djExists = await _context.Dj.AnyAsync(d => d.Id == model.DjId);
        if (!djExists)
            return BadRequest("Invalid DJ.");

        var clubExists = await _context.Clubs.AnyAsync(c => c.Id == model.ClubId);
        if (!clubExists)
            return BadRequest("Invalid club.");
        
        var clubConflict = await _context.DjSets.AnyAsync(p =>
            p.Id != model.Id &&
            p.ClubId == model.ClubId &&
            p.PerformanceTimeStarts < model.PerformanceTimeEnds &&
            p.PerformanceTimeEnds > model.PerformanceTimeStarts
        );

        if (clubConflict)
            return BadRequest("This club already has a performance during the selected time.");

        var djConflict = await _context.DjSets.AnyAsync(p =>
            p.Id != model.Id &&
            p.DjId == model.DjId &&
            p.PerformanceTimeStarts < model.PerformanceTimeEnds &&
            p.PerformanceTimeEnds > model.PerformanceTimeStarts
        );

        if (djConflict)
            return BadRequest("This DJ already has a performance during the selected time.");

        djSet.DjId = model.DjId;
        djSet.ClubId = model.ClubId;
        djSet.PerformanceTimeStarts = model.PerformanceTimeStarts;
        djSet.PerformanceTimeEnds = model.PerformanceTimeEnds;

        await _context.SaveChangesAsync();

        return Ok("Performance updated successfully.");
    }
    
    [HttpGet("allSongs")]
    public async Task<ActionResult<IEnumerable<Songs>>> GetAllSongs(
        [FromQuery] string? clubId)
    {
        var songsQuery = _context.Songs
            .Include(x => x.Status)
            .Include(x => x.DjSets)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(clubId) &&
            !clubId.Equals("all", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(clubId, out var parsedClubId))
        {
            songsQuery = songsQuery.Where(x => x.ClubId == parsedClubId);
        }

        var songs = await songsQuery
            .OrderBy(x => x.RequestedTime)
            .ToListAsync();

        return Ok(songs);
    }

    [HttpPost("createClub")]
    public async Task<IActionResult> CreateClub([FromBody] Clubs model)
    {
        if (string.IsNullOrWhiteSpace(model.ClubName))
            return BadRequest("Club name is required.");

        int nextId;

        if (await _context.Clubs.AnyAsync())
        {
            nextId = await _context.Clubs.MaxAsync(c => c.Id) + 1;
        }
        else
        {
            nextId = 1;
        }

        model.Id = nextId;

        await _context.Database.OpenConnectionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "SET IDENTITY_INSERT Clubs ON");

            _context.Clubs.Add(model);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync(
                "SET IDENTITY_INSERT Clubs OFF");
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }

        return Ok(model);
    }

    
    [HttpPut("editClub")]
    public async Task<IActionResult> EditClub([FromBody] Clubs model)
    {
        var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == model.Id);
        if (club == null)
            return NotFound("Club not found.");

        club.ClubName = model.ClubName;
        club.Location = model.Location;
        club.Floor = model.Floor;

        await _context.SaveChangesAsync();
        return Ok("Club updated successfully.");
    }
    
    [HttpDelete("deleteClub/{clubId:int}")]
    public async Task<IActionResult> DeleteClub(int clubId)
    {
        var club = await _context.Clubs
            .Include(c => c.Songs)
            .FirstOrDefaultAsync(c => c.Id == clubId);

        if (club == null)
            return NotFound("Club not found.");

        _context.Clubs.Remove(club);
        await _context.SaveChangesAsync();

        return Ok($"Club with ID {clubId} deleted.");
    }


}