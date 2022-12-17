using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppleTv.Movie.Price.Tracker.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Initdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    WrapperType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    ArtistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionCensoredName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackCensoredName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionArtistId = table.Column<int>(type: "int", nullable: false),
                    CollectionArtistViewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionViewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackViewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviewUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtworkUrl30 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtworkUrl60 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtworkUrl100 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtworkUrlBase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackRentalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionHdPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackHdPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackHdRentalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReleaseDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CollectionExplicitness = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackExplicitness = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscCount = table.Column<int>(type: "int", nullable: false),
                    DiscNumber = table.Column<int>(type: "int", nullable: false),
                    TrackCount = table.Column<int>(type: "int", nullable: false),
                    TrackNumber = table.Column<int>(type: "int", nullable: false),
                    TrackTimeMillis = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryGenreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentAdvisoryRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasITunesExtras = table.Column<bool>(type: "bit", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoviePrices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CollectionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionHdPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackRentalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackHdPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrackHdRentalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoviePrices_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviePrices_MovieId",
                table: "MoviePrices",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviePrices");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
