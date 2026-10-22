using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaTecnicaHermes.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustesModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Guaraní");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "RateToBase" },
                values: new object[] { "Dólar", 6100m });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "RateToBase",
                value: 6250m);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Guaraní paraguayo");

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "RateToBase" },
                values: new object[] { "Dólar estadounidense", 7300m });

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 3,
                column: "RateToBase",
                value: 7900m);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Real brasileño");
        }
    }
}
