using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIDToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Payment_ID",
                table: "Reservations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Payment_ID",
                table: "Reservations",
                column: "Payment_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Payments_Payment_ID",
                table: "Reservations",
                column: "Payment_ID",
                principalTable: "Payments",
                principalColumn: "Payment_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Payments_Payment_ID",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_Payment_ID",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Payment_ID",
                table: "Reservations");
        }
    }
}
