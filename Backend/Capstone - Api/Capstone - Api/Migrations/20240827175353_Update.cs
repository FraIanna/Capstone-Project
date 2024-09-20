using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone___Api.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HallMovie");

            migrationBuilder.DropTable(
                name: "MovieMovieShowtime");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "MovieShowtimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HallId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MovieShowtimes_MovieId",
                table: "MovieShowtimes",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_HallId",
                table: "Movies",
                column: "HallId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Halls_HallId",
                table: "Movies",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieShowtimes_Movies_MovieId",
                table: "MovieShowtimes",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Halls_HallId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieShowtimes_Movies_MovieId",
                table: "MovieShowtimes");

            migrationBuilder.DropIndex(
                name: "IX_MovieShowtimes_MovieId",
                table: "MovieShowtimes");

            migrationBuilder.DropIndex(
                name: "IX_Movies_HallId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "MovieShowtimes");

            migrationBuilder.DropColumn(
                name: "HallId",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "HallMovie",
                columns: table => new
                {
                    HallsId = table.Column<int>(type: "int", nullable: false),
                    MoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HallMovie", x => new { x.HallsId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_HallMovie_Halls_HallsId",
                        column: x => x.HallsId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HallMovie_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieMovieShowtime",
                columns: table => new
                {
                    MovieShowtimesId = table.Column<int>(type: "int", nullable: false),
                    MoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieMovieShowtime", x => new { x.MovieShowtimesId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_MovieMovieShowtime_MovieShowtimes_MovieShowtimesId",
                        column: x => x.MovieShowtimesId,
                        principalTable: "MovieShowtimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieMovieShowtime_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HallMovie_MoviesId",
                table: "HallMovie",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieMovieShowtime_MoviesId",
                table: "MovieMovieShowtime",
                column: "MoviesId");
        }
    }
}
