alter procedure dbo.consolidate_routes 
as
begin		
	--drop table if it already exists
	if exists (select 1 from information_schema.tables where table_name='consolidated_routes')
		drop table dbo.consolidated_routes;
	
	--recreate it here
	create table dbo.consolidated_routes (
		rowid int identity (1,1),
		route_id nvarchar(16) not null,
		route_short_name nvarchar(16) null,
		route_long_name nvarchar(256) null,
		route_type int null,
		route_url nvarchar(256) null,
		agency_id int null,
		constraint rowid primary key (route_id asc)
		with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on)
	);
	
	--insert the consolidated routes here
	--start from light rail routes first
	insert into dbo.consolidated_routes (route_id, route_short_name,route_long_name,
	route_type,route_url,agency_id)
	select y.route_id, y.route_short_name,y.route_long_name,
	y.route_type,y.route_url,y.agency_id
	from 
	(
	select convert(int,substring(route_id,1,charindex('-',route_id)-1)) as route_id,
		max(substring(route_id,charindex('-',route_id)+1, len(route_id))) as route_extension_id
	from routes
	where convert(int,substring(route_id,1,charindex('-',route_id)-1)) >= 900
	group by  convert(int,substring(route_id,1,charindex('-',route_id)-1))
	) x
	left join
	(
	select convert(int,substring(route_id,1,charindex('-',route_id)-1)) as route_id,
	substring(route_id,charindex('-',route_id)+1, len(route_id)) as route_extension_id,
		route_short_name,
		route_long_name,
		agency_id,
		route_url,
		route_type
		from routes
	) y
	on x.route_id=y.route_id
	and x.route_extension_id = y.route_extension_id
	order by y.route_id;
	
		--insert the consolidated routes here
	--start from light rail routes first
	insert into dbo.consolidated_routes (route_id, route_short_name,route_long_name,
	route_type,route_url,agency_id)
	select y.route_id, y.route_short_name,y.route_long_name,
	y.route_type,y.route_url,y.agency_id
	from 
	(
	select convert(int,substring(route_id,1,charindex('-',route_id)-1)) as route_id,
		max(substring(route_id,charindex('-',route_id)+1, len(route_id))) as route_extension_id
	from routes
	where convert(int,substring(route_id,1,charindex('-',route_id)-1)) < 900
	group by  convert(int,substring(route_id,1,charindex('-',route_id)-1))
	) x
	left join
	(
	select convert(int,substring(route_id,1,charindex('-',route_id)-1)) as route_id,
	substring(route_id,charindex('-',route_id)+1, len(route_id)) as route_extension_id,
		route_short_name,
		route_long_name,
		agency_id,
		route_url,
		route_type
		from routes
	) y
	on x.route_id=y.route_id
	and x.route_extension_id = y.route_extension_id
	order by y.route_id;
	
end;