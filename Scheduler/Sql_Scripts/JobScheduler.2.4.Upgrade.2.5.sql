
if not exists (select *
			   from sys.extended_properties
			   where name = 'js_Version'
				 and value = '2.4') 
	begin
		raiserror ('Wrong js_Version', 20, -1) with log
	end

set identity_insert [dbo].[js_JobScheduleTypes] on
go

update [dbo].[js_JobScheduleTypes]
set [Type] = 'Weekly'
where [JobScheduleTypeId] = 3
go

insert [dbo].[js_JobScheduleTypes] (
	   [JobScheduleTypeId]
	  , [Type])
values
	   (
	   5
	  , N'Daily')
go

set identity_insert [dbo].[js_JobScheduleTypes] off
go

exec sp_updateextendedproperty
@name = N'js_Version',
@value = '2.5'
go