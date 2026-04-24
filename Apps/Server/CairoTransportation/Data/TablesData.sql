-- =========================
-- LOCATIONS
-- =========================
INSERT INTO locations (id, name, type, category, population, x, y, is_critical) VALUES
('1','Maadi','NEIGHBORHOOD','Residential',250000,31.25,29.96,0),
('2','Nasr City','NEIGHBORHOOD','Mixed',500000,31.34,30.06,0),
('3','Downtown Cairo','NEIGHBORHOOD','Business',100000,31.24,30.04,1),
('4','New Cairo','NEIGHBORHOOD','Residential',300000,31.47,30.03,0),
('5','Heliopolis','NEIGHBORHOOD','Mixed',200000,31.32,30.09,0),
('6','Zamalek','NEIGHBORHOOD','Residential',50000,31.22,30.06,0),
('7','6th October City','NEIGHBORHOOD','Mixed',400000,30.98,29.93,0),
('8','Giza','NEIGHBORHOOD','Mixed',550000,31.21,29.99,0),
('9','Mohandessin','NEIGHBORHOOD','Business',180000,31.20,30.05,0),
('10','Dokki','NEIGHBORHOOD','Mixed',220000,31.21,30.03,0),
('11','Shubra','NEIGHBORHOOD','Residential',450000,31.24,30.11,0),
('12','Helwan','NEIGHBORHOOD','Industrial',350000,31.33,29.85,0),
('13','New Capital','NEIGHBORHOOD','Government',50000,31.80,30.02,1),
('14','Al Rehab','NEIGHBORHOOD','Residential',120000,31.49,30.06,0),
('15','Sheikh Zayed','NEIGHBORHOOD','Residential',150000,30.94,30.01,0),

('F1','Cairo Airport','FACILITY','Airport',NULL,31.41,30.11,1),
('F2','Ramses Station','FACILITY','Transit',NULL,31.25,30.06,1),
('F3','Cairo University','FACILITY','Education',NULL,31.21,30.03,0),
('F4','Al-Azhar University','FACILITY','Education',NULL,31.26,30.05,0),
('F5','Egyptian Museum','FACILITY','Tourism',NULL,31.23,30.05,0),
('F6','Stadium','FACILITY','Sports',NULL,31.30,30.07,0),
('F7','Smart Village','FACILITY','Business',NULL,30.97,30.07,0),
('F8','Festival City','FACILITY','Commercial',NULL,31.40,30.03,0),
('F9','Qasr El Aini','FACILITY','Medical',NULL,31.23,30.03,1),
('F10','Maadi Military Hospital','FACILITY','Medical',NULL,31.25,29.95,1);

-- =========================
-- ROADS (EXISTING)
-- =========================
INSERT INTO roads (from_location_id, to_location_id, distance, capacity, condition, is_existing, construction_cost) VALUES
('1','3',8.5,3000,7,1,NULL),
('1','8',6.2,2500,6,1,NULL),
('2','3',5.9,2800,8,1,NULL),
('2','5',4.0,3200,9,1,NULL),
('3','5',6.1,3500,7,1,NULL),
('3','6',3.2,2000,8,1,NULL),
('3','9',4.5,2600,6,1,NULL),
('3','10',3.8,2400,7,1,NULL),
('4','2',15.2,3800,9,1,NULL),
('4','14',5.3,3000,10,1,NULL),
('5','11',7.9,3100,7,1,NULL),
('6','9',2.2,1800,8,1,NULL),
('7','8',24.5,3500,8,1,NULL),
('7','15',9.8,3000,9,1,NULL),
('8','10',3.3,2200,7,1,NULL),
('8','12',14.8,2600,5,1,NULL),
('9','10',2.1,1900,7,1,NULL),
('10','11',8.7,2400,6,1,NULL),
('11','F2',3.6,2200,7,1,NULL),
('12','1',12.7,2800,6,1,NULL),
('13','4',45.0,4000,10,1,NULL),
('14','13',35.5,3800,9,1,NULL),
('15','7',9.8,3000,9,1,NULL),
('F1','5',7.5,3500,9,1,NULL),
('F1','2',9.2,3200,8,1,NULL),
('F2','3',2.5,2000,7,1,NULL),
('F7','15',8.3,2800,8,1,NULL),
('F8','4',6.1,3000,9,1,NULL);

-- =========================
-- ROADS (POTENTIAL)
-- =========================
INSERT INTO roads (from_location_id, to_location_id, distance, capacity, condition, is_existing, construction_cost) VALUES
('1','4',22.8,4000,NULL,0,450),
('1','14',25.3,3800,NULL,0,500),
('2','13',48.2,4500,NULL,0,950),
('3','13',56.7,4500,NULL,0,1100),
('5','4',16.8,3500,NULL,0,320),
('6','8',7.5,2500,NULL,0,150),
('7','13',82.3,4000,NULL,0,1600),
('9','11',6.9,2800,NULL,0,140),
('10','F7',27.4,3200,NULL,0,550),
('11','13',62.1,4200,NULL,0,1250),
('12','14',30.5,3600,NULL,0,610),
('14','5',18.2,3300,NULL,0,360),
('15','9',22.7,3000,NULL,0,450),
('F1','13',40.2,4000,NULL,0,800),
('F7','9',26.8,3200,NULL,0,540);

-- =========================
-- TRAFFIC FLOW (SAMPLE)
-- =========================
INSERT INTO traffic_flow (road_id, period, flow)
SELECT id,'MORNING',2800 FROM roads WHERE from_location_id='1' AND to_location_id='3';

INSERT INTO traffic_flow (road_id, period, flow)
SELECT id,'EVENING',2600 FROM roads WHERE from_location_id='1' AND to_location_id='3';

-- (Repeat pattern for other roads if needed)

-- =========================
-- TRANSPORT ROUTES
-- =========================
INSERT INTO transport_routes (id, type, daily_passengers, vehicles_assigned) VALUES
('M1','METRO',1500000,NULL),
('M2','METRO',1200000,NULL),
('M3','METRO',800000,NULL),
('B1','BUS',35000,25),
('B2','BUS',42000,30);

-- =========================
-- ROUTE STOPS (SAMPLE)
-- =========================
INSERT INTO route_stops (route_id, location_id, stop_order) VALUES
('M1','12',1),('M1','1',2),('M1','3',3),('M1','F2',4),('M1','11',5),
('B1','1',1),('B1','3',2),('B1','6',3),('B1','9',4);

-- =========================
-- TRANSPORT DEMAND
-- =========================
INSERT INTO transport_demand (from_location_id, to_location_id, daily_passengers) VALUES
('3','5',15000),
('1','3',12000),
('2','3',18000),
('F2','11',25000),
('F1','3',20000),
('7','3',14000),
('4','3',16000),
('8','3',22000);

-- =========================
-- ROAD MAINTENANCE (SAMPLE)
-- =========================
INSERT INTO road_maintenance (road_id, priority, estimated_cost) VALUES
(1,9,50),
(2,7,40),
(3,10,80),
(4,6,30),
(5,8,60);