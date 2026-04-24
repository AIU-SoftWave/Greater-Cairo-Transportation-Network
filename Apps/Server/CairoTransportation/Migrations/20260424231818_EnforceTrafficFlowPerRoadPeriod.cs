using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations
{
    /// <inheritdoc />
    public partial class EnforceTrafficFlowPerRoadPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Consolidate duplicate road+period rows by summing flow.
            migrationBuilder.Sql(@"
                CREATE TEMP TABLE tmp_traffic_flow_agg AS
                SELECT
                    road_id,
                    period,
                    SUM(flow) AS flow
                FROM traffic_flow
                GROUP BY road_id, period;

                DELETE FROM traffic_flow;

                INSERT INTO traffic_flow (road_id, period, flow)
                SELECT road_id, period, flow
                FROM tmp_traffic_flow_agg;

                DROP TABLE tmp_traffic_flow_agg;
            ");

            // Ensure every existing road has a flow row for every configured period.
            migrationBuilder.Sql(@"
                INSERT INTO traffic_flow (road_id, period, flow)
                SELECT
                    r.id,
                    pm.period,
                    CAST(ROUND(r.capacity * pm.multiplier * 0.60, 0) AS INTEGER)
                FROM roads r
                JOIN traffic_period_multipliers pm ON 1 = 1
                LEFT JOIN traffic_flow tf ON tf.road_id = r.id AND tf.period = pm.period
                WHERE r.is_existing = 1
                  AND tf.id IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "uq_traffic_road_period",
                table: "traffic_flow",
                columns: new[] { "road_id", "period" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_traffic_road_period",
                table: "traffic_flow");
        }
    }
}
