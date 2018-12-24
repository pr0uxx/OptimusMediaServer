using Microsoft.EntityFrameworkCore.Migrations;

namespace Optimus.Data.Migrations
{
    public partial class UpdateUAR02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssessedRank_AspNetUsers_UserId",
                table: "UserAssessedRank");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssessedRank",
                table: "UserAssessedRank");

            migrationBuilder.RenameTable(
                name: "UserAssessedRank",
                newName: "UserAssessedRanks");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssessedRank_UserId",
                table: "UserAssessedRanks",
                newName: "IX_UserAssessedRanks_UserId");

            migrationBuilder.AlterColumn<decimal>(
                name: "StandardisedScore",
                table: "UserAssessedRanks",
                type: "decimal(18,17)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssessedRanks",
                table: "UserAssessedRanks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssessedRanks_AspNetUsers_UserId",
                table: "UserAssessedRanks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssessedRanks_AspNetUsers_UserId",
                table: "UserAssessedRanks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssessedRanks",
                table: "UserAssessedRanks");

            migrationBuilder.RenameTable(
                name: "UserAssessedRanks",
                newName: "UserAssessedRank");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssessedRanks_UserId",
                table: "UserAssessedRank",
                newName: "IX_UserAssessedRank_UserId");

            migrationBuilder.AlterColumn<decimal>(
                name: "StandardisedScore",
                table: "UserAssessedRank",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,17)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssessedRank",
                table: "UserAssessedRank",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssessedRank_AspNetUsers_UserId",
                table: "UserAssessedRank",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
