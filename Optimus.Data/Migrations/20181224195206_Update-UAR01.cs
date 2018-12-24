using Microsoft.EntityFrameworkCore.Migrations;

namespace Optimus.Data.Migrations
{
    public partial class UpdateUAR01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "StandardisedScore",
                table: "UserAssessedRank",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "StandardisedScore",
                table: "UserAssessedRank",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");
        }
    }
}
