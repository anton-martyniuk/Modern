using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modern.EfCore.Examples.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    area = table.Column<double>(type: "REAL", nullable: false),
                    population = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cities_name",
                table: "cities",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cities");
        }
    }
}
