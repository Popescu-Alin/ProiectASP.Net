using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV1.Migrations
{
    public partial class OredrFull2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Orders",
                newName: "Address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Orders",
                newName: "Adress");
        }
    }
}
