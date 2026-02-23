using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaApi2.Migrations
{
    /// <inheritdoc />
    public partial class costoEstimadoPrestamo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoEstimado",
                table: "Prestamos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoEstimado",
                table: "Prestamos");
        }
    }
}
