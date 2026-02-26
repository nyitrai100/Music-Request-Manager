using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DatabaseLayer.Migrations
{
    /// <inheritdoc />
    public partial class DbTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dj_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DjSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DjId = table.Column<int>(type: "int", nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    PerformanceTimeStarts = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformanceTimeEnds = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DjSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DjSets_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DjSets_Dj_DjId",
                        column: x => x.DjId,
                        principalTable: "Dj",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    DjSetId = table.Column<int>(type: "int", nullable: false),
                    RequestedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Songs_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Songs_DjSets_DjSetId",
                        column: x => x.DjSetId,
                        principalTable: "DjSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Songs_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6f1b98cf-2a8a-4f59-826f-153b81d39120", null, "Admin", "ADMIN" },
                    { "c0fd7c02-4289-4cc3-8abf-2122919c1fc1", null, "User", "USER" },
                    { "d595d426-f92d-4aec-84a4-dcad31bd86e1", null, "DJ", "DJ" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0fc69c76-6089-43e0-864f-46f4c95f75b8", 0, "5618633f-6f6c-4a0d-be2b-fff4051033e1", "user2@example.com", true, false, null, "USER2@EXAMPLE.COM", "USER2", "AQAAAAIAAYagAAAAEK+b/0tDVFYPlaxIAtCwPbwlFVy6mPWE/N8fXMGJHZ+xOhLdt7tm6CBDyKAwWwWp5w==", null, false, "1920ecbb-6f6d-4900-8495-148ccc69bba9", false, "user2" },
                    { "21677737-93f9-41bd-a318-7027a2a480b4", 0, "325da8cc-52c8-4b9d-b592-abd574d43fb8", "user@example.com", true, false, null, "USER@EXAMPLE.COM", "USER", "AQAAAAIAAYagAAAAEIzbN2wTZjWvn58eRcf1kY8lUzo0uci2gtw/VBvApAvWKySbZ2xcHfSIKzLASbZcYQ==", null, false, "17aa6313-0910-4ae3-bec4-7479eb053bd9", false, "user" },
                    { "4609ecd1-a827-4855-96b4-b5dc9d18e048", 0, "fe4bf0d4-a764-4ef4-a41b-cab577928892", "dj@example.com", true, false, null, "DJ@EXAMPLE.COM", "DJ", "AQAAAAIAAYagAAAAEBCs4mf+M3KRmqDyUHX48rDDZIQbyvcTyiYzm7DJkc5lwCC5ghmp/X074oLZVeyz2Q==", null, false, "3003fe68-b5da-40db-b6f3-806431668a1e", false, "dj" },
                    { "8ba45c5e-3d41-46a5-b485-6cde638d12fd", 0, "3aa8b05b-4636-482f-831d-1310587931e5", "dj2@example.com", true, false, null, "DJ2@EXAMPLE.COM", "DJ2", "AQAAAAIAAYagAAAAEJL6480F4BQFC+edkU6VlreONG8YLmmGfYzm4dQK+WyeiL6RQ93kT/TUnS7YszqRiA==", null, false, "2afa7ae4-6b60-470f-91c4-cb6fbef7478d", false, "dj2" },
                    { "8d35d26a-efbb-475b-a17d-7a5bdd38e0cb", 0, "dd48c681-cff1-4b6c-8406-d00f34c99e36", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEGKwef27BwC4jACPYLeDjfDMZ52Df1khErJ8KlT2Ii6zL6cuat1kXXV1VVFtDthMCA==", null, false, "ee70639e-a5c5-4004-9dc7-91a548638d39", false, "admin" }
                });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "ClubName", "Floor", "Location" },
                values: new object[,]
                {
                    { 1, "Pump it up", 1, "Manchester" },
                    { 2, "StudentCrew", 1, "Huddersfield" },
                    { 3, "Revolution", 1, "Leeds" },
                    { 4, "Viviera", 1, "Keszthely" }
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Accepted" },
                    { 2, "Rejected" },
                    { 3, "Pending" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "c0fd7c02-4289-4cc3-8abf-2122919c1fc1", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { "c0fd7c02-4289-4cc3-8abf-2122919c1fc1", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { "d595d426-f92d-4aec-84a4-dcad31bd86e1", "4609ecd1-a827-4855-96b4-b5dc9d18e048" },
                    { "d595d426-f92d-4aec-84a4-dcad31bd86e1", "8ba45c5e-3d41-46a5-b485-6cde638d12fd" },
                    { "6f1b98cf-2a8a-4f59-826f-153b81d39120", "8d35d26a-efbb-475b-a17d-7a5bdd38e0cb" }
                });

            migrationBuilder.InsertData(
                table: "Dj",
                columns: new[] { "Id", "UserId" },
                values: new object[,]
                {
                    { 1, "4609ecd1-a827-4855-96b4-b5dc9d18e048" },
                    { 2, "8ba45c5e-3d41-46a5-b485-6cde638d12fd" }
                });

            migrationBuilder.InsertData(
                table: "DjSets",
                columns: new[] { "Id", "ClubId", "DjId", "PerformanceTimeEnds", "PerformanceTimeStarts" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2025, 11, 7, 1, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 6, 20, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, 2, new DateTime(2025, 11, 7, 2, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 6, 20, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, 1, new DateTime(2025, 11, 5, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 5, 20, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 4, 2, new DateTime(2025, 11, 5, 23, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 5, 21, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "Author", "ClubId", "DjSetId", "RequestedTime", "StatusId", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, "Basshunter", 1, 1, new DateTime(2025, 11, 6, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, "Now You're Gone", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 2, "Jennifer Lopez", 1, 1, new DateTime(2025, 11, 6, 20, 11, 0, 0, DateTimeKind.Unspecified), 1, "On The Floor", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 3, "David Guetta", 1, 1, new DateTime(2025, 11, 6, 20, 26, 0, 0, DateTimeKind.Unspecified), 2, "Titanium", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 4, "Avicii", 1, 1, new DateTime(2025, 11, 6, 20, 51, 0, 0, DateTimeKind.Unspecified), 1, "Levels", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 5, "Swedish House Mafia", 1, 1, new DateTime(2025, 11, 6, 21, 16, 0, 0, DateTimeKind.Unspecified), 3, "Don't You Worry Child", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 6, "Calvin Harris", 1, 1, new DateTime(2025, 11, 6, 21, 36, 0, 0, DateTimeKind.Unspecified), 1, "Summer", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 7, "Rihanna", 1, 1, new DateTime(2025, 11, 6, 22, 1, 0, 0, DateTimeKind.Unspecified), 2, "We Found Love", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 8, "LMFAO", 1, 1, new DateTime(2025, 11, 6, 22, 26, 0, 0, DateTimeKind.Unspecified), 1, "Party Rock Anthem", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 9, "Taio Cruz", 1, 1, new DateTime(2025, 11, 6, 22, 46, 0, 0, DateTimeKind.Unspecified), 1, "Dynamite", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 10, "The Weeknd", 1, 1, new DateTime(2025, 11, 6, 23, 21, 0, 0, DateTimeKind.Unspecified), 3, "Blinding Lights", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 11, "Pitbull", 2, 2, new DateTime(2025, 11, 6, 20, 0, 0, 0, DateTimeKind.Unspecified), 1, "International Love", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 12, "Usher", 2, 2, new DateTime(2025, 11, 6, 20, 16, 0, 0, DateTimeKind.Unspecified), 1, "DJ Got Us Fallin' In Love", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 13, "Ne-Yo", 2, 2, new DateTime(2025, 11, 6, 20, 41, 0, 0, DateTimeKind.Unspecified), 2, "Closer", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 14, "Flo Rida", 2, 2, new DateTime(2025, 11, 6, 21, 6, 0, 0, DateTimeKind.Unspecified), 3, "Good Feeling", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 15, "Lady Gaga", 2, 2, new DateTime(2025, 11, 6, 21, 31, 0, 0, DateTimeKind.Unspecified), 1, "Poker Face", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 16, "Katy Perry", 2, 2, new DateTime(2025, 11, 6, 21, 56, 0, 0, DateTimeKind.Unspecified), 2, "Firework", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 17, "Black Eyed Peas", 2, 2, new DateTime(2025, 11, 6, 22, 21, 0, 0, DateTimeKind.Unspecified), 1, "I Gotta Feeling", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 18, "Adele", 2, 2, new DateTime(2025, 11, 6, 22, 46, 0, 0, DateTimeKind.Unspecified), 3, "Rolling in the Deep", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 19, "Maroon 5", 2, 2, new DateTime(2025, 11, 6, 23, 31, 0, 0, DateTimeKind.Unspecified), 1, "Moves Like Jagger", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 20, "Coldplay", 2, 2, new DateTime(2025, 11, 7, 0, 41, 0, 0, DateTimeKind.Unspecified), 2, "Viva La Vida", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 21, "Imagine Dragons", 3, 3, new DateTime(2025, 11, 5, 20, 15, 0, 0, DateTimeKind.Unspecified), 1, "Believer", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 22, "Ed Sheeran", 3, 3, new DateTime(2025, 11, 5, 20, 45, 0, 0, DateTimeKind.Unspecified), 1, "Shape of You", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 23, "Marshmello", 3, 3, new DateTime(2025, 11, 5, 21, 10, 0, 0, DateTimeKind.Unspecified), 1, "Happier", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 24, "David Guetta", 3, 3, new DateTime(2025, 11, 5, 21, 30, 0, 0, DateTimeKind.Unspecified), 1, "Play Hard", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 25, "Avicii", 3, 3, new DateTime(2025, 11, 5, 22, 0, 0, 0, DateTimeKind.Unspecified), 1, "Wake Me Up", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 26, "Calvin Harris", 3, 3, new DateTime(2025, 11, 5, 22, 30, 0, 0, DateTimeKind.Unspecified), 1, "Feel So Close", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 27, "Pitbull", 4, 4, new DateTime(2025, 11, 5, 21, 15, 0, 0, DateTimeKind.Unspecified), 1, "Give Me Everything", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 28, "Lady Gaga", 4, 4, new DateTime(2025, 11, 5, 21, 45, 0, 0, DateTimeKind.Unspecified), 1, "Bad Romance", "0fc69c76-6089-43e0-864f-46f4c95f75b8" },
                    { 29, "Rihanna", 4, 4, new DateTime(2025, 11, 5, 22, 10, 0, 0, DateTimeKind.Unspecified), 1, "Diamonds", "21677737-93f9-41bd-a318-7027a2a480b4" },
                    { 30, "Katy Perry", 4, 4, new DateTime(2025, 11, 5, 22, 40, 0, 0, DateTimeKind.Unspecified), 1, "Teenage Dream", "0fc69c76-6089-43e0-864f-46f4c95f75b8" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Dj_UserId",
                table: "Dj",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DjSets_ClubId",
                table: "DjSets",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_DjSets_DjId",
                table: "DjSets",
                column: "DjId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_ClubId",
                table: "Songs",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_DjSetId",
                table: "Songs",
                column: "DjSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_StatusId",
                table: "Songs",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_UserId",
                table: "Songs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DjSets");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Dj");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
