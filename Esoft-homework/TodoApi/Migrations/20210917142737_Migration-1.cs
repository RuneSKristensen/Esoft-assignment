using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoApi.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Family",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    BirthYear = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Family", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    connection = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonRelation",
                columns: table => new
                {
                    PersonsId = table.Column<long>(type: "INTEGER", nullable: false),
                    RelationsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonRelation", x => new { x.PersonsId, x.RelationsId });
                    table.ForeignKey(
                        name: "FK_PersonRelation_Family_PersonsId",
                        column: x => x.PersonsId,
                        principalTable: "Family",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonRelation_Relation_RelationsId",
                        column: x => x.RelationsId,
                        principalTable: "Relation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonRelation_RelationsId",
                table: "PersonRelation",
                column: "RelationsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonRelation");

            migrationBuilder.DropTable(
                name: "Family");

            migrationBuilder.DropTable(
                name: "Relation");
        }
    }
}
