using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV1.Migrations
{
    public partial class RevStar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stars",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stars",
                table: "Reviews");
        }
    }
}
