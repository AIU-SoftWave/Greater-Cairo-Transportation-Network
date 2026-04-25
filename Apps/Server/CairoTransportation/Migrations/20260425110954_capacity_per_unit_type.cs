using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations
{
    /// <inheritdoc />
    public partial class capacity_per_unit_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<int>(
                name: "capacity_per_unit",
                table: "transport_routes",
                type: "int",
                nullable: false,
                defaultValue: 50,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 50);

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.AlterColumn<int>(
                name: "capacity_per_unit",
                table: "transport_routes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 50,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 50);
    }
}
