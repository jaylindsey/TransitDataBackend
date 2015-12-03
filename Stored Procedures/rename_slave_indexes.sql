alter procedure dbo.rename_slave_indexes
as
begin
	declare @master_index_name varchar(256);
	declare @slave_index_name varchar(256);
	declare @deduced_table_name varchar(256);
	declare @full_index_name varchar(512);
	declare @sql_stmt varchar(2048);

	begin try
		begin transaction
		declare index_name_cursor cursor for
		select index_name from (
			select name as index_name 
			from sys.indexes
			where name like '%_slave_ix_%'
			) x;
			
			open index_name_cursor;
			fetch next from index_name_cursor into @slave_index_name;
				
				while @@fetch_status = 0
				begin
					--slave index name is like trips_slave_ix_02
					--we need to rename it to trips_ix_02
					set @master_index_name = replace(@slave_index_name, '_slave', '');
					
					--we also need to deduce the table name from the index name as it is required as a part of sp_rename thingy
					set @deduced_table_name = substring(@slave_index_name, 1, charindex('_slave', @slave_index_name) - 1);
					--this constructs 'dbo.table_name.index_name'
					set @full_index_name = 'dbo.' + @deduced_table_name + '.' + @slave_index_name;
					
					--rename the slave as the master
					execute sp_rename @full_index_name, @master_index_name;
					--print @full_index_name + ' ' + @master_index_name
					
					--fetch the next one in the cursor
					fetch next from index_name_cursor into @slave_index_name ;
				end;
				
			--when finished processing, close and deallocate the cursor
			close index_name_cursor;
			deallocate index_name_cursor;
			
		--if no errors, commit the transaction
		commit transaction;
	end try
	begin catch
			--if errors, check the cursor status of the index_name_cursor
			if (CURSOR_STATUS('global','index_name_cursor') > -1) 
			begin
				--if open, close the cursor and release the memory
				close index_name_cursor;
				deallocate index_name_cursor;
			end
			
			--rollback all the drops 
			rollback transaction;
			--propagate the error 
			throw;
	end catch

end;