using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class DocumentResult4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.AlterColumn<string>(
                name: "PlantChatGPTResponse",
                table: "DocumentResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BarkUrl",
                table: "DocumentResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlowerUrl",
                table: "DocumentResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FruitUrl",
                table: "DocumentResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeafUrl",
                table: "DocumentResults",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarkUrl",
                table: "DocumentResults");

            migrationBuilder.DropColumn(
                name: "FlowerUrl",
                table: "DocumentResults");

            migrationBuilder.DropColumn(
                name: "FruitUrl",
                table: "DocumentResults");

            migrationBuilder.DropColumn(
                name: "LeafUrl",
                table: "DocumentResults");

            migrationBuilder.AlterColumn<string>(
                name: "PlantChatGPTResponse",
                table: "DocumentResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ImageDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Copyright = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentResultBarkId = table.Column<int>(type: "int", nullable: false),
                    DocumentResultFlowerId = table.Column<int>(type: "int", nullable: false),
                    DocumentResultFruitId = table.Column<int>(type: "int", nullable: false),
                    DocumentResultId = table.Column<int>(type: "int", nullable: true),
                    DocumentResultId1 = table.Column<int>(type: "int", nullable: true),
                    DocumentResultId2 = table.Column<int>(type: "int", nullable: true),
                    DocumentResultId3 = table.Column<int>(type: "int", nullable: true),
                    DocumentResultLeafId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageDetail_DocumentResults_DocumentResultId",
                        column: x => x.DocumentResultId,
                        principalTable: "DocumentResults",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImageDetail_DocumentResults_DocumentResultId1",
                        column: x => x.DocumentResultId1,
                        principalTable: "DocumentResults",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImageDetail_DocumentResults_DocumentResultId2",
                        column: x => x.DocumentResultId2,
                        principalTable: "DocumentResults",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ImageDetail_DocumentResults_DocumentResultId3",
                        column: x => x.DocumentResultId3,
                        principalTable: "DocumentResults",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlantCoordinate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentResultId = table.Column<int>(type: "int", nullable: true),
                    Lat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantCoordinate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantCoordinate_DocumentResults_DocumentResultId",
                        column: x => x.DocumentResultId,
                        principalTable: "DocumentResults",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultId",
                table: "ImageDetail",
                column: "DocumentResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultId1",
                table: "ImageDetail",
                column: "DocumentResultId1");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultId2",
                table: "ImageDetail",
                column: "DocumentResultId2");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultId3",
                table: "ImageDetail",
                column: "DocumentResultId3");

            migrationBuilder.CreateIndex(
                name: "IX_PlantCoordinate_DocumentResultId",
                table: "PlantCoordinate",
                column: "DocumentResultId");
        }
    }
}
