alter procedure dbo.create_indexes (
	@table_name varchar(256)
)
with recompile
as
begin
	if @table_name in ('all','trips')
	begin
		create index trips_slave_ix_01 on dbo.trips_slave(route_id); 
		create index trips_slave_ix_03 on dbo.trips_slave(shape_id);
		create index trips_slave_ix_02 on dbo.trips_slave(service_id);
	end;
	
	if @table_name in ('all','stop_times')
	begin
		alter table [dbo].[stop_times_slave] add  constraint [stop_times_slave_pk] primary key clustered 
		(
			[trip_id] asc,
			[arrival_time] asc,
			[stop_id] asc
		)with (pad_index = off, statistics_norecompute = off, sort_in_tempdb = off, ignore_dup_key = off, online = off, allow_row_locks = on, allow_page_locks = on);

		create index stop_times_slave_ix_01 on dbo.stop_times_slave(trip_id);
	end;
end;