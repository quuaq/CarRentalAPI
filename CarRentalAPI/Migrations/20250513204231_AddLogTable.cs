using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_User_ID1",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_User_ID1",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "User_ID1",
                table: "Logs");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_User_ID",
                table: "Logs",
                column: "User_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_User_ID",
                table: "Logs",
                column: "User_ID",
                principalTable: "Users",
                principalColumn: "User_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_User_ID",
                table: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Logs_User_ID",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "User_ID1",
                table: "Logs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_User_ID1",
                table: "Logs",
                column: "User_ID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_User_ID1",
                table: "Logs",
                column: "User_ID1",
                principalTable: "Users",
                principalColumn: "User_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
