using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeautySalon.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ULOGA",
                columns: table => new
                {
                    uloga_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    naziv_uloge = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ULOGA", x => x.uloga_id);
                });

            migrationBuilder.CreateTable(
                name: "USLUGA",
                columns: table => new
                {
                    usluga_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    naziv = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    opis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    cijena = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    trajanje = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USLUGA", x => x.usluga_id);
                });

            migrationBuilder.CreateTable(
                name: "KORISNIK",
                columns: table => new
                {
                    korisnik_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prezime = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    lozinka = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    telefon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    datum_registracije = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    vrijeme_zadnje_prijave = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    uloga_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KORISNIK", x => x.korisnik_id);
                    table.ForeignKey(
                        name: "FK_KORISNIK_ULOGA_uloga_id",
                        column: x => x.uloga_id,
                        principalTable: "ULOGA",
                        principalColumn: "uloga_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TERMIN",
                columns: table => new
                {
                    termin_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    datum = table.Column<DateTime>(type: "date", nullable: false),
                    vrijeme = table.Column<TimeSpan>(type: "time", nullable: false),
                    trajanje = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    korisnik_id = table.Column<int>(type: "integer", nullable: false),
                    zaposlenik_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TERMIN", x => x.termin_id);
                    table.ForeignKey(
                        name: "FK_TERMIN_KORISNIK_korisnik_id",
                        column: x => x.korisnik_id,
                        principalTable: "KORISNIK",
                        principalColumn: "korisnik_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TERMIN_KORISNIK_zaposlenik_id",
                        column: x => x.zaposlenik_id,
                        principalTable: "KORISNIK",
                        principalColumn: "korisnik_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STAVKA_TERMINA",
                columns: table => new
                {
                    stavka_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kolicina = table.Column<int>(type: "integer", nullable: false),
                    cijena = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    usluga_id = table.Column<int>(type: "integer", nullable: false),
                    termin_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STAVKA_TERMINA", x => x.stavka_id);
                    table.ForeignKey(
                        name: "FK_STAVKA_TERMINA_TERMIN_termin_id",
                        column: x => x.termin_id,
                        principalTable: "TERMIN",
                        principalColumn: "termin_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STAVKA_TERMINA_USLUGA_usluga_id",
                        column: x => x.usluga_id,
                        principalTable: "USLUGA",
                        principalColumn: "usluga_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ULOGA",
                columns: new[] { "uloga_id", "naziv_uloge" },
                values: new object[,]
                {
                    { 1, "Klijent" },
                    { 2, "Zaposlenik" },
                    { 3, "Administrator" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KORISNIK_email",
                table: "KORISNIK",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KORISNIK_uloga_id",
                table: "KORISNIK",
                column: "uloga_id");

            migrationBuilder.CreateIndex(
                name: "IX_STAVKA_TERMINA_termin_id",
                table: "STAVKA_TERMINA",
                column: "termin_id");

            migrationBuilder.CreateIndex(
                name: "IX_STAVKA_TERMINA_usluga_id",
                table: "STAVKA_TERMINA",
                column: "usluga_id");

            migrationBuilder.CreateIndex(
                name: "IX_TERMIN_korisnik_id",
                table: "TERMIN",
                column: "korisnik_id");

            migrationBuilder.CreateIndex(
                name: "IX_TERMIN_zaposlenik_id",
                table: "TERMIN",
                column: "zaposlenik_id");

            migrationBuilder.CreateIndex(
                name: "IX_ULOGA_naziv_uloge",
                table: "ULOGA",
                column: "naziv_uloge",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USLUGA_naziv",
                table: "USLUGA",
                column: "naziv",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STAVKA_TERMINA");

            migrationBuilder.DropTable(
                name: "TERMIN");

            migrationBuilder.DropTable(
                name: "USLUGA");

            migrationBuilder.DropTable(
                name: "KORISNIK");

            migrationBuilder.DropTable(
                name: "ULOGA");
        }
    }
}
