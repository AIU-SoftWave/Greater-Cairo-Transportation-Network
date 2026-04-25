using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations
{
    public partial class AddTrafficPeriodMultipliers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS traffic_period_multipliers (
                    period varchar(20) NOT NULL,
                    multiplier REAL NOT NULL,
                    CONSTRAINT PK_traffic_period_multipliers PRIMARY KEY (period),
                    CONSTRAINT chk_multiplier_positive CHECK (multiplier > 0)
                );
            ");

            migrationBuilder.Sql("INSERT OR IGNORE INTO traffic_period_multipliers (period, multiplier) VALUES ('MORNING', 1.15);");
            migrationBuilder.Sql("INSERT OR IGNORE INTO traffic_period_multipliers (period, multiplier) VALUES ('EVENING', 1.25);");
            migrationBuilder.Sql("INSERT OR IGNORE INTO traffic_period_multipliers (period, multiplier) VALUES ('NIGHT', 0.90);");
        }

        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("DROP TABLE IF EXISTS traffic_period_multipliers;");
    }
}
