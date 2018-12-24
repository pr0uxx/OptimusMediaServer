using Microsoft.EntityFrameworkCore.Migrations;

namespace Optimus.Data.Migrations
{
    public partial class UpdateUAR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RankValueA",
                table: "UserAssessedRank");

            migrationBuilder.DropColumn(
                name: "RankValueB",
                table: "UserAssessedRank");

            migrationBuilder.DropColumn(
                name: "RankValueC",
                table: "UserAssessedRank");

            migrationBuilder.AddColumn<decimal>(
                name: "StandardisedScore",
                table: "UserAssessedRank",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StandardisedScore",
                table: "UserAssessedRank");

            migrationBuilder.AddColumn<int>(
                name: "RankValueA",
                table: "UserAssessedRank",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RankValueB",
                table: "UserAssessedRank",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RankValueC",
                table: "UserAssessedRank",
                nullable: false,
                defaultValue: 0);
        }
    }
}
