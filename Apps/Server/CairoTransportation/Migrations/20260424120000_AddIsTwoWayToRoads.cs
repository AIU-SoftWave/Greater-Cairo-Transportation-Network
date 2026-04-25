using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations
{
    public partial class AddIsTwoWayToRoads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<bool>(
                name: "is_two_way",
                table: "roads",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
                name: "is_two_way",
                table: "roads");
    }
}
