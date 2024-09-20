using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstone___Api.Migrations
{
    /// <inheritdoc />
    public partial class AddingTicketPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "TicketPrice",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 7.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketPrice",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
