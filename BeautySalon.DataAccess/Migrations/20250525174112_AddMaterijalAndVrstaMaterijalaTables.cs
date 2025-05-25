using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeautySalon.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterijalAndVrstaMaterijalaTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "opis",
                table: "USLUGA",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateTable(
                name: "VRSTA_MATERIJALA",
                columns: table => new
                {
                    vrsta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VRSTA_MATERIJALA", x => x.vrsta_id);
                });

            migrationBuilder.CreateTable(
                name: "MATERIJAL",
                columns: table => new
                {
                    materijal_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    naziv = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cijena = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    minimalna_kolicina = table.Column<int>(type: "integer", nullable: false),
                    trenutna_kolicina = table.Column<int>(type: "integer", nullable: false),
                    jedinica_mjere = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    vrsta_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATERIJAL", x => x.materijal_id);
                    table.ForeignKey(
                        name: "FK_MATERIJAL_VRSTA_MATERIJALA_vrsta_id",
                        column: x => x.vrsta_id,
                        principalTable: "VRSTA_MATERIJALA",
                        principalColumn: "vrsta_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MATERIJAL_naziv",
                table: "MATERIJAL",
                column: "naziv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MATERIJAL_vrsta_id",
                table: "MATERIJAL",
                column: "vrsta_id");

            migrationBuilder.CreateIndex(
                name: "IX_VRSTA_MATERIJALA_naziv",
                table: "VRSTA_MATERIJALA",
                column: "naziv",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MATERIJAL");

            migrationBuilder.DropTable(
                name: "VRSTA_MATERIJALA");

            migrationBuilder.AlterColumn<string>(
                name: "opis",
                table: "USLUGA",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
