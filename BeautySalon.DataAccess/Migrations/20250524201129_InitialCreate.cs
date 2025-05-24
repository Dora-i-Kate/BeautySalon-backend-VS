using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BeautySalon.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    KorisnikId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Prezime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Lozinka = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BrojTelefona = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DatumRegistracije = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PosljednjaPrijava = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Uloga = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Zahtjevi = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BrojBodova = table.Column<int>(type: "integer", nullable: true),
                    Specijalizacija = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Certifikat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DatumZaposlenja = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.KorisnikId);
                });

            migrationBuilder.CreateTable(
                name: "Termini",
                columns: table => new
                {
                    TerminId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DatumVrijeme = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KlijentId = table.Column<int>(type: "integer", nullable: false),
                    ZaposlenikId = table.Column<int>(type: "integer", nullable: false),
                    KomentarZaposlenika = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Termini", x => x.TerminId);
                    table.ForeignKey(
                        name: "FK_Termini_Korisnici_KlijentId",
                        column: x => x.KlijentId,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Termini_Korisnici_ZaposlenikId",
                        column: x => x.ZaposlenikId,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Termini_KlijentId",
                table: "Termini",
                column: "KlijentId");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_ZaposlenikId",
                table: "Termini",
                column: "ZaposlenikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Termini");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
