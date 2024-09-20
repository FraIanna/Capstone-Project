using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone___Api.Migrations
{
    /// <inheritdoc />
    public partial class Update5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_HallSeats_HallSeatId",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "HallSeats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HallSeats_MovieId",
                table: "HallSeats",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_HallSeats_Movies_MovieId",
                table: "HallSeats",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_HallSeats_HallSeatId",
                table: "Reservations",
                column: "HallSeatId",
                principalTable: "HallSeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HallSeats_Movies_MovieId",
                table: "HallSeats");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_HallSeats_HallSeatId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_HallSeats_MovieId",
                table: "HallSeats");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "HallSeats");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_HallSeats_HallSeatId",
                table: "Reservations",
                column: "HallSeatId",
                principalTable: "HallSeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
