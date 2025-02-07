using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeVilla_VillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVillaNoColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VillaNO",
                table: "VillaNumbers",
                newName: "VillaNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VillaNo",
                table: "VillaNumbers",
                newName: "VillaNO");
        }
    }
}
