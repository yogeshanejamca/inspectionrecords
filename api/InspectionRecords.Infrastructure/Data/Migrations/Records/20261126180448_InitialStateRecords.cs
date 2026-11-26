using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InspectionRecords.Api.Data.Migrations.Records
{
    /// <inheritdoc />
    public partial class InitialStateRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StateDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlaceIdsJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateMaps",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    X = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Y = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STATERECORD",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Visited = table.Column<bool>(type: "bit", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STATERECORD", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STATERECORD_StateId",
                table: "STATERECORD",
                column: "StateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StateDefinitions");

            migrationBuilder.DropTable(
                name: "StateMaps");

            migrationBuilder.DropTable(
                name: "STATERECORD");
        }
    }
}
