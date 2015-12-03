alter procedure dbo.create_slave_tables (
	@table_name varchar(256)
)
as
begin
	
	if (@table_name in ('all', 'agency'))
	begin
		create table dbo.agency_slave (
			agency_id int not null,
			agency_name nvarchar(256) null,
			agency_url nvarchar(256) null,
			agency_timezone nvarchar(16) null,
			agency_lang nvarchar(16) null,
			constraint agency_slave_pk primary key clustered  (agency_id asc)
			with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
	if (@table_name in ('all', 'calendar'))
	begin
		create table dbo.calendar_slave (
			service_id nvarchar(256) not null,
			monday bit null,
			tuesday bit null,
			wednesday bit null,
			thursday bit null,
			friday bit null,
			saturday bit null,
			sunday bit null,
			start_date datetime not null,
			end_date datetime null,
		 constraint calendar_slave_pk primary key clustered (service_id asc, start_date asc )
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
	if (@table_name in ('all', 'calendar_dates'))
	begin		
		create table dbo.calendar_dates_slave (
			service_id nvarchar(256) not null,
			date datetime not null,
			exception_type nvarchar(48) null,
		 constraint calendar_dates_slave_pk primary key clustered (service_id asc, date asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
	if (@table_name in ('all', 'routes'))
	begin		
		create table dbo.routes_slave (
			route_id nvarchar(55) not null,
			agency_id int null,
			route_short_name nvarchar(16) null,
			route_long_name nvarchar(256) null,
			route_desc nvarchar(8) null,
			route_type int null,
			route_url nvarchar(256) null,
			route_color nvarchar(48) null,
			route_text_color nvarchar(16) null,
		 constraint routes_slave_pk primary key clustered (route_id asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;

	if (@table_name in ('all', 'shapes'))
	begin		
		create table dbo.shapes_slave (
			shape_id int not null,
			shape_pt_lat float null,
			shape_pt_lon float null,
			shape_pt_sequence int not null,
		 constraint shapes_slave_pk primary key clustered (	shape_id asc, shape_pt_sequence asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
	if (@table_name in ('all', 'stop_times'))
	begin		
		create table dbo.stop_times_slave (
			trip_id nvarchar(256) not null,
			arrival_time nvarchar(16) not null,
			departure_time nvarchar(16) null,
			stop_id int not null,
			stop_sequence int null,
			pickup_type int null,
			drop_off_type int null,
		 constraint stop_times_slave_pk primary key clustered (trip_id asc,arrival_time asc,stop_id asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
	if (@table_name in ('all', 'stops'))
	begin		
		create table dbo.stops_slave (
			stop_id int not null,
			stop_name nvarchar(256) null,
			stop_desc nvarchar(16) null,
			stop_lat nvarchar(32) null,
			stop_lon nvarchar(32) null,
			stop_street nvarchar(64) null,
			stop_city nvarchar(64) null,
			stop_region nvarchar(64) null,
			stop_postcode nvarchar(16) null,
			stop_country nvarchar(32) null,
			zone_id nvarchar(16) null,
			wheelchair_boarding int null,
			stop_url nvarchar(256) null,
		 constraint stops_slave_pk primary key clustered (stop_id asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
	if (@table_name in ('all', 'trips'))
	begin		
		create table dbo.trips_slave (
			route_id nvarchar(32) null,
			service_id nvarchar(48) null,
			trip_id nvarchar(256) not null,
			trip_headsign nvarchar(64) null,
			block_id int null,
			shape_id int null,
			wheelchair_accessible bit null,
		 constraint trips_slave_pk primary key clustered (trip_id asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	
		create table dbo.route_directions_slave (
			route_id nvarchar(256) not null,
			direction_long nvarchar(16) null,
			direction_short nvarchar(1) not null,
		 constraint route_directions_slave_pk primary key clustered (route_id asc, direction_short asc)
		 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
		);
	end;
	
end;