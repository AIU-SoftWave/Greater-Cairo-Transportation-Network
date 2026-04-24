using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations;

public partial class AddTrafficFlowPeriodForeignKey : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS __traffic_flow_tmp (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    road_id INTEGER NOT NULL,
                    period varchar(20) NOT NULL,
                    flow INTEGER NOT NULL,
                    CONSTRAINT chk_flow CHECK (flow >= 0),
                    CONSTRAINT fk_traffic_road FOREIGN KEY (road_id) REFERENCES roads(id) ON DELETE CASCADE,
                    CONSTRAINT fk_traffic_period_multiplier FOREIGN KEY (period) REFERENCES traffic_period_multipliers(period) ON DELETE RESTRICT
                );
            ");

        migrationBuilder.Sql(@"
                INSERT INTO __traffic_flow_tmp (id, road_id, period, flow)
                SELECT tf.id, tf.road_id, tf.period, tf.flow
                FROM traffic_flow tf
                INNER JOIN traffic_period_multipliers tpm ON tpm.period = tf.period;
            ");

        migrationBuilder.Sql("DROP TABLE traffic_flow;");
        migrationBuilder.Sql("ALTER TABLE __traffic_flow_tmp RENAME TO traffic_flow;");
        migrationBuilder.Sql("CREATE INDEX idx_traffic_road ON traffic_flow(road_id);");
        migrationBuilder.Sql("CREATE INDEX idx_traffic_period ON traffic_flow(period);");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS __traffic_flow_tmp (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    road_id INTEGER NOT NULL,
                    period varchar(20) NOT NULL,
                    flow INTEGER NOT NULL,
                    CONSTRAINT chk_flow CHECK (flow >= 0),
                    CONSTRAINT fk_traffic_road FOREIGN KEY (road_id) REFERENCES roads(id) ON DELETE CASCADE
                );
            ");

        migrationBuilder.Sql(@"
                INSERT INTO __traffic_flow_tmp (id, road_id, period, flow)
                SELECT id, road_id, period, flow
                FROM traffic_flow;
            ");

        migrationBuilder.Sql("DROP TABLE traffic_flow;");
        migrationBuilder.Sql("ALTER TABLE __traffic_flow_tmp RENAME TO traffic_flow;");
        migrationBuilder.Sql("CREATE INDEX idx_traffic_road ON traffic_flow(road_id);");
        migrationBuilder.Sql("CREATE INDEX idx_traffic_period ON traffic_flow(period);");
    }
}
