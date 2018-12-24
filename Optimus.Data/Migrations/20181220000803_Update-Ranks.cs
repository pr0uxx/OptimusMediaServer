using Microsoft.EntityFrameworkCore.Migrations;

namespace Optimus.Data.Migrations
{
    public partial class UpdateRanks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RankName",
                table: "UserAssessedRank",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "RankName",
                table: "UserAssessedRank",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
