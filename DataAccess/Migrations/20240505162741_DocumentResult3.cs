﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    public partial class DocumentResult3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultId",
                table: "ImageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultId1",
                table: "ImageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultId2",
                table: "ImageDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultId3",
                table: "ImageDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImageDetail_DocumentResultId",
                table: "ImageDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImageDetail_DocumentResultId1",
                table: "ImageDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImageDetail_DocumentResultId2",
                table: "ImageDetail");

            migrationBuilder.DropIndex(
                name: "IX_ImageDetail_DocumentResultId3",
                table: "ImageDetail");

            migrationBuilder.DropColumn(
                name: "DocumentResultId",
                table: "ImageDetail");

            migrationBuilder.DropColumn(
                name: "DocumentResultId1",
                table: "ImageDetail");

            migrationBuilder.DropColumn(
                name: "DocumentResultId2",
                table: "ImageDetail");

            migrationBuilder.DropColumn(
                name: "DocumentResultId3",
                table: "ImageDetail");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultBarkId",
                table: "ImageDetail",
                column: "DocumentResultBarkId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultFlowerId",
                table: "ImageDetail",
                column: "DocumentResultFlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultFruitId",
                table: "ImageDetail",
                column: "DocumentResultFruitId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageDetail_DocumentResultLeafId",
                table: "ImageDetail",
                column: "DocumentResultLeafId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultBarkId",
                table: "ImageDetail",
                column: "DocumentResultBarkId",
                principalTable: "DocumentResults",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultFlowerId",
                table: "ImageDetail",
                column: "DocumentResultFlowerId",
                principalTable: "DocumentResults",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultFruitId",
                table: "ImageDetail",
                column: "DocumentResultFruitId",
                principalTable: "DocumentResults",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageDetail_DocumentResults_DocumentResultLeafId",
                table: "ImageDetail",
                column: "DocumentResultLeafId",
                principalTable: "DocumentResults",
                principalColumn: "Id");
        }
    }
}