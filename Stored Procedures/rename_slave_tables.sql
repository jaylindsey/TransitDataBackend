alter procedure dbo.rename_slave_tables
as
begin
	declare @master_name varchar(256);
	declare @slave_name varchar(256);
	declare @master_primary_key_name varchar(256);
	declare @slave_primary_key_name varchar(256);
	declare @sql_stmt varchar(2048);

	begin try
		begin transaction
		declare master_table_name_cursor cursor for
			select substring(table_name, 1, charindex('_slave',table_name)-1) as table_name
			from information_schema.tables
			where table_name like '%_slave';
			
			open master_table_name_cursor;
			fetch next from master_table_name_cursor into @master_name;
				
				while @@fetch_status = 0
				begin
					--deduce slave table name from the master: i.e. master=agency, slave=agency_slave
					set @slave_name = @master_name + '_slave';
					--deduce the primary key name of slave table: i.e. if slave table name = agency_slave, primary key must be agency_slave_pk
					set @slave_primary_key_name = @slave_name +'_pk';
					--deduce the primary key name of master table: i.e. if master table name = agency, primary key must be agency_pk
					set @master_primary_key_name = @master_name+'_pk';
					
					--drop the master table
					set @sql_stmt = 'drop table dbo.' + @master_name;
					execute (@sql_stmt);
					
					--rename the slave as the master
					execute sp_rename @slave_name, @master_name;
					
					--rename the slave primary key as the master
					execute sp_rename @slave_primary_key_name, @master_primary_key_name;
					
					--fetch the next one in the cursor
					fetch next from master_table_name_cursor into @master_name ;
				end;
				
			--when finished processing, close and deallocate the cursor
			close master_table_name_cursor;
			deallocate master_table_name_cursor;
			
		--if no errors, commit the transaction
		commit transaction;
	end try
	begin catch
			--if errors, check the cursor status of the master_table_name_cursor
			if (CURSOR_STATUS('global','master_table_name_cursor') > -1) 
			begin
				--if open, close the cursor and release the memory
				close master_table_name_cursor;
				deallocate master_table_name_cursor;
			end
			
			--rollback all the drops 
			rollback transaction;
			--propagate the error 
			throw;
	end catch

end;