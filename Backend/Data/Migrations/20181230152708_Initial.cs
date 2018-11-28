using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlarmWorkflow.Backend.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operation",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    acknowledged = table.Column<bool>(nullable: false),
                    operationguid = table.Column<Guid>(nullable: false),
                    operationnumber = table.Column<string>(nullable: true),
                    timestampincome = table.Column<DateTime>(nullable: false),
                    timestampalarm = table.Column<DateTime>(nullable: false),
                    messenger = table.Column<string>(nullable: true),
                    comment = table.Column<string>(nullable: true),
                    plan = table.Column<string>(nullable: true),
                    picture = table.Column<string>(nullable: true),
                    priority = table.Column<string>(nullable: true),
                    einsatzortlocation = table.Column<string>(nullable: true),
                    einsatzortzipcode = table.Column<string>(nullable: true),
                    einsatzortcity = table.Column<string>(nullable: true),
                    einsatzortstreet = table.Column<string>(nullable: true),
                    einsatzortstreetnumber = table.Column<string>(nullable: true),
                    einsatzortintersection = table.Column<string>(nullable: true),
                    Einsatzort_GeoLatitude = table.Column<double>(nullable: true),
                    Einsatzort_GeoLongitude = table.Column<double>(nullable: true),
                    einsatzortproperty = table.Column<string>(nullable: true),
                    einsatzortlatlng = table.Column<string>(nullable: true),
                    zielortlocation = table.Column<string>(nullable: true),
                    zielortzipcode = table.Column<string>(nullable: true),
                    zielortcity = table.Column<string>(nullable: true),
                    zielortstreet = table.Column<string>(nullable: true),
                    zielortstreetnumber = table.Column<string>(nullable: true),
                    zielortintersection = table.Column<string>(nullable: true),
                    Zielort_GeoLatitude = table.Column<double>(nullable: true),
                    Zielort_GeoLongitude = table.Column<double>(nullable: true),
                    zielortproperty = table.Column<string>(nullable: true),
                    zielortlatlng = table.Column<string>(nullable: true),
                    keyword = table.Column<string>(nullable: true),
                    keywordmisc = table.Column<string>(nullable: true),
                    keywordb = table.Column<string>(nullable: true),
                    keywordr = table.Column<string>(nullable: true),
                    keywords = table.Column<string>(nullable: true),
                    keywordt = table.Column<string>(nullable: true),
                    loopscsv = table.Column<string>(nullable: true),
                    customdatajson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usersetting",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    identifier = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usersetting", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dispresource",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    operation_id = table.Column<int>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false),
                    emkresourceid = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispresource", x => x.id);
                    table.ForeignKey(
                        name: "FK_dispresource_operation_operation_id",
                        column: x => x.operation_id,
                        principalTable: "operation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "operationresource",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                        .Annotation("Sqlite:Autoincrement", true),
                    operation_id = table.Column<int>(nullable: false),
                    timestamp = table.Column<string>(nullable: true),
                    fullname = table.Column<string>(nullable: true),
                    equipmentcsv = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operationresource", x => x.id);
                    table.ForeignKey(
                        name: "FK_operationresource_operation_operation_id",
                        column: x => x.operation_id,
                        principalTable: "operation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dispresource_operation_id",
                table: "dispresource",
                column: "operation_id");

            migrationBuilder.CreateIndex(
                name: "IX_operationresource_operation_id",
                table: "operationresource",
                column: "operation_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dispresource");

            migrationBuilder.DropTable(
                name: "operationresource");

            migrationBuilder.DropTable(
                name: "usersetting");

            migrationBuilder.DropTable(
                name: "operation");
        }
    }
}
