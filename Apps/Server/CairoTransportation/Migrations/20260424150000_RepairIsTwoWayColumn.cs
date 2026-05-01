using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations;

public partial class RepairIsTwoWayColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("ALTER TABLE roads ADD COLUMN is_two_way INTEGER NOT NULL DEFAULT 1;");

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
