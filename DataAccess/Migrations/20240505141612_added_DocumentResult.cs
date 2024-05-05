using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class added_DocumentResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoragePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantGlobalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoboflowJsonModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlantChatGPTResponse = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Copyright = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentResultId = table.Column<int>(type: "int", nullable: true),
                    DocumentResultId1 = table.Column<int>(type: "int", nullable: true),
                    DocumentResultId2 = table.Column<int>(type: "int", nullable: true),
                    DocumentResultId3 = table.Column<int>(type: "int", nullable: true)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentResultId = table.Column<int>(type: "int", nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageDetail");

            migrationBuilder.DropTable(
                name: "PlantCoordinate");

            migrationBuilder.DropTable(
                name: "DocumentResults");
        }
    }
}
