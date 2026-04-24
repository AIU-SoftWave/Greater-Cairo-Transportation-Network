using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoTransportation.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    population = table.Column<int>(type: "INTEGER", nullable: true),
                    x = table.Column<double>(type: "REAL", nullable: false),
                    y = table.Column<double>(type: "REAL", nullable: false),
                    is_critical = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transport_routes",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    daily_passengers = table.Column<int>(type: "INTEGER", nullable: true),
                    vehicles_assigned = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transport_routes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roads",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    from_location_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    to_location_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    distance = table.Column<double>(type: "REAL", nullable: false),
                    capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    condition = table.Column<int>(type: "INTEGER", nullable: true),
                    is_existing = table.Column<bool>(type: "INTEGER", nullable: false),
                    construction_cost = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roads", x => x.id);
                    table.CheckConstraint("chk_capacity", "capacity > 0");
                    table.CheckConstraint("chk_distance", "distance > 0");
                    table.ForeignKey(
                        name: "fk_roads_from",
                        column: x => x.from_location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_roads_to",
                        column: x => x.to_location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transport_demand",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    from_location_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    to_location_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    daily_passengers = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transport_demand", x => x.id);
                    table.ForeignKey(
                        name: "fk_demand_from",
                        column: x => x.from_location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_demand_to",
                        column: x => x.to_location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_stops",
                columns: table => new
                {
                    route_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    location_id = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    stop_order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_stops", x => new { x.route_id, x.location_id });
                    table.ForeignKey(
                        name: "fk_route_stops_location",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_route_stops_route",
                        column: x => x.route_id,
                        principalTable: "transport_routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "road_maintenance",
                columns: table => new
                {
                    road_id = table.Column<long>(type: "INTEGER", nullable: false),
                    priority = table.Column<int>(type: "INTEGER", nullable: true),
                    estimated_cost = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_road_maintenance", x => x.road_id);
                    table.ForeignKey(
                        name: "FK_road_maintenance_roads_road_id",
                        column: x => x.road_id,
                        principalTable: "roads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "traffic_flow",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    road_id = table.Column<long>(type: "INTEGER", nullable: false),
                    period = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    flow = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traffic_flow", x => x.id);
                    table.CheckConstraint("chk_flow", "flow >= 0");
                    table.ForeignKey(
                        name: "fk_traffic_road",
                        column: x => x.road_id,
                        principalTable: "roads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_roads_from",
                table: "roads",
                column: "from_location_id");

            migrationBuilder.CreateIndex(
                name: "idx_roads_to",
                table: "roads",
                column: "to_location_id");

            migrationBuilder.CreateIndex(
                name: "idx_route_stops_order",
                table: "route_stops",
                columns: new[] { "route_id", "stop_order" });

            migrationBuilder.CreateIndex(
                name: "IX_route_stops_location_id",
                table: "route_stops",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "idx_traffic_period",
                table: "traffic_flow",
                column: "period");

            migrationBuilder.CreateIndex(
                name: "idx_traffic_road",
                table: "traffic_flow",
                column: "road_id");

            migrationBuilder.CreateIndex(
                name: "idx_demand_from",
                table: "transport_demand",
                column: "from_location_id");

            migrationBuilder.CreateIndex(
                name: "idx_demand_to",
                table: "transport_demand",
                column: "to_location_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "road_maintenance");

            migrationBuilder.DropTable(
                name: "route_stops");

            migrationBuilder.DropTable(
                name: "traffic_flow");

            migrationBuilder.DropTable(
                name: "transport_demand");

            migrationBuilder.DropTable(
                name: "transport_routes");

            migrationBuilder.DropTable(
                name: "roads");

            migrationBuilder.DropTable(
                name: "locations");
        }
    }
}
