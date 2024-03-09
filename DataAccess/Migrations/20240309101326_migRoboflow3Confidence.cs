using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class migRoboflow3Confidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RoboflowResponses",
                newName: "PlantName");

            migrationBuilder.AlterColumn<int>(
                name: "Confidence",
                table: "RoboflowResponses",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlantName",
                table: "RoboflowResponses",
                newName: "Name");

            migrationBuilder.AlterColumn<double>(
                name: "Confidence",
                table: "RoboflowResponses",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
