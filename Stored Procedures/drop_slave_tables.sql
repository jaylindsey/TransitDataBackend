alter procedure dbo.drop_slave_tables (
	@table_name varchar(256)
)
as
begin
	declare @table_name_to_drop varchar(256);
	declare @sql_stmnt varchar(2048);

	if (@table_name in ('all', 'agency'))
	begin
		if exists (select 1 from information_schema.tables where table_name='agency_slave')
			drop table dbo.agency_slave;
	end;
	
	if (@table_name in ('all', 'calendar'))
	begin	
		if exists (select 1 from information_schema.tables where table_name='calendar_slave')
			drop table dbo.calendar_slave;
	end;
	
	if (@table_name in ('all', 'calendar_dates'))
	begin	
		if exists (select 1 from information_schema.tables where table_name='calendar_dates_slave')
			drop table dbo.calendar_dates_slave;
	end;
	
	if (@table_name in ('all', 'routes'))
	begin				
		if exists (select 1 from information_schema.tables where table_name='routes_slave')
			drop table dbo.routes_slave;
	end;
	
	if (@table_name in ('all', 'shapes'))
	begin				
		if exists (select 1 from information_schema.tables where table_name='shapes_slave')
			drop table dbo.shapes_slave;
	end;
	
	if (@table_name in ('all', 'stop_times'))
	begin				
		if exists (select 1 from information_schema.tables where table_name='stop_times_slave')
			drop table dbo.stop_times_slave;
	end;
	
	if (@table_name in ('all', 'stops'))
	begin				
		if exists (select 1 from information_schema.tables where table_name='stops_slave')
			drop table dbo.stops_slave;
	end;
	
	if (@table_name in ('all', 'trips'))
	begin				
		if exists (select 1 from information_schema.tables where table_name='trips_slave')	
			drop table dbo.trips_slave;
			
		if exists (select 1 from information_schema.tables where table_name='route_directions_slave')	
			drop table dbo.route_directions_slave;		
	end;
end;