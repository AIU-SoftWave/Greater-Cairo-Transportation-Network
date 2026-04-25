CREATE TABLE
  `locations` (
    `id` VARCHAR(10) PRIMARY KEY,
    `name` VARCHAR(100) NOT NULL,
    `type` VARCHAR(20) NOT NULL,
    `category` VARCHAR(50),
    `population` INT,
    `x` DOUBLE NOT NULL,
    `y` DOUBLE NOT NULL,
    `is_critical` BOOLEAN DEFAULT false
  );

CREATE TABLE
  `roads` (
    `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
    `from_location_id` VARCHAR(10) NOT NULL,
    `to_location_id` VARCHAR(10) NOT NULL,
    `distance` DOUBLE NOT NULL,
    `capacity` INT NOT NULL,
    `condition` INT,
    `is_existing` BOOLEAN NOT NULL,
    `is_two_way` BOOLEAN NOT NULL DEFAULT true,
    `construction_cost` DOUBLE,
    CONSTRAINT `chk_distance` CHECK (distance > 0),
    CONSTRAINT `chk_capacity` CHECK (capacity > 0)
  );

CREATE TABLE
  `traffic_period_multipliers` (
    `period` VARCHAR(20) PRIMARY KEY,
    `multiplier` DOUBLE NOT NULL,
    CONSTRAINT `chk_multiplier_positive` CHECK (multiplier > 0)
  );

CREATE TABLE
  `traffic_flow` (
    `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
    `road_id` BIGINT NOT NULL,
    `period` VARCHAR(20) NOT NULL,
    `flow` INT NOT NULL,
    CONSTRAINT `chk_flow` CHECK (flow >= 0)
  );

CREATE TABLE
  `transport_routes` (
    `id` VARCHAR(10) PRIMARY KEY,
    `type` VARCHAR(20) NOT NULL,
    `daily_passengers` INT,
    `vehicles_assigned` INT
  );

CREATE TABLE
  `route_stops` (
    `route_id` VARCHAR(10),
    `location_id` VARCHAR(10),
    `stop_order` INT NOT NULL,
    PRIMARY KEY (`route_id`, `location_id`)
  );

CREATE TABLE
  `transport_demand` (
    `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
    `from_location_id` VARCHAR(10) NOT NULL,
    `to_location_id` VARCHAR(10) NOT NULL,
    `daily_passengers` INT NOT NULL
  );

-- FIXED: multiple maintenance records per road
CREATE TABLE
  `road_maintenance` (
    `id` BIGINT PRIMARY KEY AUTO_INCREMENT,
    `road_id` BIGINT NOT NULL,
    `priority` INT,
    `estimated_cost` DOUBLE
  );

-- INDEXES
CREATE INDEX `idx_roads_from` ON `roads` (`from_location_id`);

CREATE INDEX `idx_roads_to` ON `roads` (`to_location_id`);

CREATE INDEX `idx_traffic_road` ON `traffic_flow` (`road_id`);

CREATE INDEX `idx_traffic_period` ON `traffic_flow` (`period`);

CREATE UNIQUE INDEX `uq_traffic_road_period` ON `traffic_flow` (`road_id`, `period`);

CREATE INDEX `idx_route_stops_order` ON `route_stops` (`route_id`, `stop_order`);

CREATE INDEX `idx_demand_from` ON `transport_demand` (`from_location_id`);

CREATE INDEX `idx_demand_to` ON `transport_demand` (`to_location_id`);

-- FOREIGN KEYS
ALTER TABLE `roads` ADD CONSTRAINT `fk_roads_from` FOREIGN KEY (`from_location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE;

ALTER TABLE `roads` ADD CONSTRAINT `fk_roads_to` FOREIGN KEY (`to_location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE;

ALTER TABLE `traffic_flow` ADD CONSTRAINT `fk_traffic_road` FOREIGN KEY (`road_id`) REFERENCES `roads` (`id`) ON DELETE CASCADE;

ALTER TABLE `traffic_flow` ADD CONSTRAINT `fk_traffic_period_multiplier` FOREIGN KEY (`period`) REFERENCES `traffic_period_multipliers` (`period`) ON DELETE RESTRICT;

ALTER TABLE `route_stops` ADD CONSTRAINT `fk_route_stops_route` FOREIGN KEY (`route_id`) REFERENCES `transport_routes` (`id`) ON DELETE CASCADE;

ALTER TABLE `route_stops` ADD CONSTRAINT `fk_route_stops_location` FOREIGN KEY (`location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE;

ALTER TABLE `transport_demand` ADD CONSTRAINT `fk_demand_from` FOREIGN KEY (`from_location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE;

ALTER TABLE `transport_demand` ADD CONSTRAINT `fk_demand_to` FOREIGN KEY (`to_location_id`) REFERENCES `locations` (`id`) ON DELETE CASCADE;

ALTER TABLE `road_maintenance` ADD CONSTRAINT `fk_road_maintenance_road` FOREIGN KEY (`road_id`) REFERENCES `roads` (`id`) ON DELETE CASCADE;