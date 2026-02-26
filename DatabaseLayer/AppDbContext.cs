using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatabaseLayer;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Clubs> Clubs { get; set; }
    public DbSet<Dj> Dj { get; set; }
    public DbSet<DjSets> DjSets { get; set; }
    public DbSet<Songs> Songs { get; set; }
    public DbSet<Status> Status { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SeedUsers(builder);
        SeedRoles(builder);
        SeedUserRoles(builder);
        SeedSongs(builder);
        SeedClubs(builder);
        SeedDjs(builder);
        SeedDjsSets(builder);
        SeedStatus(builder);
        
        builder.Entity<Dj>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Dj>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DjSets>()
            .HasOne(x => x.Dj)
            .WithMany()
            .HasForeignKey(x=> x.DjId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<DjSets>()
            .HasOne(x => x.Club)
            .WithMany()
            .HasForeignKey(x=> x.ClubId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Songs>()
            .HasOne( x=> x.Club)
            .WithMany(x => x.Songs)
            .HasForeignKey(x=> x.ClubId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Songs>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Songs>()
            .HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<Songs>()
            .HasOne(x => x.DjSets)
            .WithMany()
            .HasForeignKey(x => x.DjSetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private void SeedSongs(ModelBuilder builder){
    builder.Entity<Songs>().HasData(
        new Songs {Id = 1, ClubId = 1, Author = "Basshunter", Title = "Now You're Gone", RequestedTime = new DateTime(2025, 11, 06, 20, 00, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 1},
        new Songs {Id = 2, ClubId = 1, Author = "Jennifer Lopez", Title = "On The Floor", RequestedTime = new DateTime(2025, 11, 06, 20, 11, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 1},
        new Songs {Id = 3, ClubId = 1, Author = "David Guetta", Title = "Titanium", RequestedTime = new DateTime(2025, 11, 06, 20, 26, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 2, DjSetId = 1},
        new Songs {Id = 4, ClubId = 1, Author = "Avicii", Title = "Levels", RequestedTime = new DateTime(2025, 11, 06, 20, 51, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 1},
        new Songs {Id = 5, ClubId = 1, Author = "Swedish House Mafia", Title = "Don't You Worry Child", RequestedTime = new DateTime(2025, 11, 06, 21, 16, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 3, DjSetId = 1},
        new Songs {Id = 6, ClubId = 1, Author = "Calvin Harris", Title = "Summer", RequestedTime = new DateTime(2025, 11, 06, 21, 36, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 1},
        new Songs {Id = 7, ClubId = 1, Author = "Rihanna", Title = "We Found Love", RequestedTime = new DateTime(2025, 11, 06, 22, 01, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 2, DjSetId = 1},
        new Songs {Id = 8, ClubId = 1, Author = "LMFAO", Title = "Party Rock Anthem", RequestedTime = new DateTime(2025, 11, 06, 22, 26, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 1},
        new Songs {Id = 9, ClubId = 1, Author = "Taio Cruz", Title = "Dynamite", RequestedTime = new DateTime(2025, 11, 06, 22, 46, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 1},
        new Songs {Id = 10, ClubId = 1, Author = "The Weeknd", Title = "Blinding Lights", RequestedTime = new DateTime(2025, 11, 06, 23, 21, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 3, DjSetId = 1},

        new Songs {Id = 11, ClubId = 2, Author = "Pitbull", Title = "International Love", RequestedTime = new DateTime(2025, 11, 06, 20, 00, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 2},
        new Songs {Id = 12, ClubId = 2, Author = "Usher", Title = "DJ Got Us Fallin' In Love", RequestedTime = new DateTime(2025, 11, 06, 20, 16, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 2},
        new Songs {Id = 13, ClubId = 2, Author = "Ne-Yo", Title = "Closer", RequestedTime = new DateTime(2025, 11, 06, 20, 41, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 2, DjSetId = 2},
        new Songs {Id = 14, ClubId = 2, Author = "Flo Rida", Title = "Good Feeling", RequestedTime = new DateTime(2025, 11, 06, 21, 06, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 3, DjSetId = 2},
        new Songs {Id = 15, ClubId = 2, Author = "Lady Gaga", Title = "Poker Face", RequestedTime = new DateTime(2025, 11, 06, 21, 31, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 2},
        new Songs {Id = 16, ClubId = 2, Author = "Katy Perry", Title = "Firework", RequestedTime = new DateTime(2025, 11, 06, 21, 56, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 2, DjSetId = 2},
        new Songs {Id = 17, ClubId = 2, Author = "Black Eyed Peas", Title = "I Gotta Feeling", RequestedTime = new DateTime(2025, 11, 06, 22, 21, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 2},
        new Songs {Id = 18, ClubId = 2, Author = "Adele", Title = "Rolling in the Deep", RequestedTime = new DateTime(2025, 11, 06, 22, 46, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 3, DjSetId = 2},
        new Songs {Id = 19, ClubId = 2, Author = "Maroon 5", Title = "Moves Like Jagger", RequestedTime = new DateTime(2025, 11, 06, 23, 31, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 2},
        new Songs {Id = 20, ClubId = 2, Author = "Coldplay", Title = "Viva La Vida", RequestedTime = new DateTime(2025, 11, 07, 00, 41, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 2, DjSetId = 2},

        new Songs {Id = 21, ClubId = 3, Author = "Imagine Dragons", Title = "Believer", RequestedTime = new DateTime(2025, 11, 05, 20, 15, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 3},
        new Songs {Id = 22, ClubId = 3, Author = "Ed Sheeran", Title = "Shape of You", RequestedTime = new DateTime(2025, 11, 05, 20, 45, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 3},
        new Songs {Id = 23, ClubId = 3, Author = "Marshmello", Title = "Happier", RequestedTime = new DateTime(2025, 11, 05, 21, 10, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 3},
        new Songs {Id = 24, ClubId = 3, Author = "David Guetta", Title = "Play Hard", RequestedTime = new DateTime(2025, 11, 05, 21, 30, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 3},
        new Songs {Id = 25, ClubId = 3, Author = "Avicii", Title = "Wake Me Up", RequestedTime = new DateTime(2025, 11, 05, 22, 00, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 3},
        new Songs {Id = 26, ClubId = 3, Author = "Calvin Harris", Title = "Feel So Close", RequestedTime = new DateTime(2025, 11, 05, 22, 30, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 3},
        new Songs {Id = 27, ClubId = 4, Author = "Pitbull", Title = "Give Me Everything", RequestedTime = new DateTime(2025, 11, 05, 21, 15, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 4},
        new Songs {Id = 28, ClubId = 4, Author = "Lady Gaga", Title = "Bad Romance", RequestedTime = new DateTime(2025, 11, 05, 21, 45, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 4},
        new Songs {Id = 29, ClubId = 4, Author = "Rihanna", Title = "Diamonds", RequestedTime = new DateTime(2025, 11, 05, 22, 10, 00), UserId = "21677737-93f9-41bd-a318-7027a2a480b4", StatusId = 1, DjSetId = 4},
        new Songs {Id = 30, ClubId = 4, Author = "Katy Perry", Title = "Teenage Dream", RequestedTime = new DateTime(2025, 11, 05, 22, 40, 00), UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8", StatusId = 1, DjSetId = 4}
    );
}


    
    private void SeedClubs(ModelBuilder builder)
    {
        builder.Entity<Clubs>().HasData(
                new Clubs {Id = 1, ClubName = "Pump it up", Location = "Manchester", Floor = 1},
                new Clubs {Id = 2, ClubName = "StudentCrew", Location = "Huddersfield", Floor = 1},
                new Clubs {Id = 3, ClubName = "Revolution", Location = "Leeds", Floor = 1},
                new Clubs {Id = 4, ClubName = "Viviera", Location = "Keszthely", Floor = 1}
            );
    }

    private void SeedDjs(ModelBuilder builder)
    {
        builder.Entity<Dj>().HasData(
            new Dj {Id = 1, UserId = "4609ecd1-a827-4855-96b4-b5dc9d18e048"},
            new Dj {Id = 2, UserId = "8ba45c5e-3d41-46a5-b485-6cde638d12fd"}
        );
    }
    
    private void SeedDjsSets(ModelBuilder builder)
    {
        builder.Entity<DjSets>().HasData(
            new DjSets
            {
                Id = 1, DjId = 1, ClubId = 1,
                PerformanceTimeStarts = new DateTime(2025, 11, 06, 20, 00, 00),
                PerformanceTimeEnds = new DateTime(2025, 11, 07, 01, 00, 00)
            },

            new DjSets
            {
                Id = 2, DjId = 2, ClubId = 2,
                PerformanceTimeStarts = new DateTime(2025, 11, 06, 20, 00, 00),
                PerformanceTimeEnds = new DateTime(2025, 11, 07, 02, 00, 00)
            },

            new DjSets
            {
                Id = 3, DjId = 1, ClubId = 3,
                PerformanceTimeStarts = new DateTime(2025, 11, 05, 20, 00, 00),
                PerformanceTimeEnds = new DateTime(2025, 11, 05, 23, 00, 00)
            },

            new DjSets
            {
                Id = 4, DjId = 2, ClubId = 4,
                PerformanceTimeStarts = new DateTime(2025, 11, 05, 21, 00, 00),
                PerformanceTimeEnds = new DateTime(2025, 11, 05, 23, 00, 00)
            }
        );
    }


    private void SeedStatus(ModelBuilder builder)
    {
        builder.Entity<Status>().HasData(
            new Status {Id = 1, Name = "Accepted" },
            new Status {Id = 2, Name = "Rejected" },
            new Status {Id = 3, Name = "Pending" }
        );
    }

    
    private void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "6f1b98cf-2a8a-4f59-826f-153b81d39120",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "d595d426-f92d-4aec-84a4-dcad31bd86e1",
                Name = "DJ",
                NormalizedName = "DJ"
            },
            new IdentityRole
            {
                Id = "c0fd7c02-4289-4cc3-8abf-2122919c1fc1",
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
    private void SeedUsers(ModelBuilder builder)
    {
        var hasher = new PasswordHasher<IdentityUser>();

        var admin = new IdentityUser
        {
            Id = "8d35d26a-efbb-475b-a17d-7a5bdd38e0cb",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Admin123!"),
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        var user = new IdentityUser
        {
            Id = "21677737-93f9-41bd-a318-7027a2a480b4",
            UserName = "user",
            NormalizedUserName = "USER",
            Email = "user@example.com",
            NormalizedEmail = "USER@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "User123!"),
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        
        var user2 = new IdentityUser
        {
            Id = "0fc69c76-6089-43e0-864f-46f4c95f75b8",
            UserName = "user2",
            NormalizedUserName = "USER2",
            Email = "user2@example.com",
            NormalizedEmail = "USER2@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "User123!"),
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        
        var dj = new IdentityUser
        {
            Id = "4609ecd1-a827-4855-96b4-b5dc9d18e048",
            UserName = "dj",
            NormalizedUserName = "DJ",
            Email = "dj@example.com",
            NormalizedEmail = "DJ@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Dj123!"),
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        var dj2 = new IdentityUser
        {
            Id = "8ba45c5e-3d41-46a5-b485-6cde638d12fd",
            UserName = "dj2",
            NormalizedUserName = "DJ2",
            Email = "dj2@example.com",
            NormalizedEmail = "DJ2@EXAMPLE.COM",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "Dj123!"),
            SecurityStamp = Guid.NewGuid().ToString("D")
        };
        builder.Entity<IdentityUser>().HasData(admin, user, user2, dj,dj2);
    }
    
    private void SeedUserRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "6f1b98cf-2a8a-4f59-826f-153b81d39120",
                UserId = "8d35d26a-efbb-475b-a17d-7a5bdd38e0cb"
            },
            new IdentityUserRole<string>
            {
                RoleId = "c0fd7c02-4289-4cc3-8abf-2122919c1fc1",
                UserId = "21677737-93f9-41bd-a318-7027a2a480b4"
            },
            new IdentityUserRole<string>
            {
                RoleId = "c0fd7c02-4289-4cc3-8abf-2122919c1fc1",
                UserId = "0fc69c76-6089-43e0-864f-46f4c95f75b8"
            },
            new IdentityUserRole<string>
            {
                RoleId = "d595d426-f92d-4aec-84a4-dcad31bd86e1",
                UserId = "4609ecd1-a827-4855-96b4-b5dc9d18e048"
            },
            new IdentityUserRole<string>
            {
                RoleId = "d595d426-f92d-4aec-84a4-dcad31bd86e1",
                UserId = "8ba45c5e-3d41-46a5-b485-6cde638d12fd"
            }
        );
    }
}
