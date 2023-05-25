using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pracical1.dataAccess.Migrations
{
    public partial class change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "addresses",
                newName: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "addresses",
                newName: "Id");
        }
    }
}
