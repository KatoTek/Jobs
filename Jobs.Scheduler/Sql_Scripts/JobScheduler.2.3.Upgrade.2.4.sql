
if not exists (select *
			   from sys.extended_properties
			   where name = 'js_Version'
				 and value = '2.3') 
	begin
		raiserror ('Wrong js_Version', 20, -1) with log
	end

/*********************
** SETUP TEMP TABLE **
*********************/

if not exists (select
					  *
			   from INFORMATION_SCHEMA.TABLES
			   where TABLE_NAME = 'tmpJobExceptions')
	begin
		create table [dbo].[tmpJobExceptions] (
					 [JobExceptionId] [int] identity (1, 1)
											not null
					,
					 [JobInstanceId] [bigint] not null
					,
					 [ChildId] [int] null
					,
					 [Date] [datetime] null
					,
					 [Type] [nvarchar] (500) null
					,
					 [Message] [nvarchar] (max) null
					,
					 [Source] [nvarchar] (500) null
					,
					 [StackTrace] [nvarchar] (max) null
					,
					 [HelpLink] [nvarchar] (500) null
					,
					 primary key clustered
					 (
					 [JobExceptionId] asc
					 )
						 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
		)
		on [PRIMARY] textimage_on [PRIMARY]

		print 'Added tmpJobExceptions table'
	end
go

if not exists (select
					  *
			   from sys.foreign_keys
			   where name = 'FK_tmpJobExceptions_JobExceptions')
	begin
		alter table [dbo].[tmpJobExceptions]
				with check
		add
					 constraint [FK_tmpJobExceptions_JobExceptions] foreign key ([ChildId])
					 references [dbo].[tmpJobExceptions] (
								[JobExceptionId])

		alter table [dbo].[tmpJobExceptions] check constraint
					[FK_tmpJobExceptions_JobExceptions]

		print 'Added FK_tmpJobExceptions_JobExceptions constraint'
	end
go

/*****************
** MIGRATE DATA **
*****************/

set identity_insert tmpJobExceptions on
go

if not exists (select
					  *
			   from tmpJobExceptions)
	begin
		insert into tmpJobExceptions (
			   JobExceptionId
			  , ChildId
			  , JobInstanceId
			  , [Date]
			  , [Type]
			  , [Message]
			  , [Source]
			  , StackTrace
			  , HelpLink)
		select
			   parent.JobExceptionId
			  , child.JobExceptionId as ChildId
			  , parent.JobInstanceId
			  , parent.[Date]
			  , parent.[Type]
			  , parent.[Message]
			  , parent.[Source]
			  , parent.StackTrace
			  , parent.HelpLink
		from
			 js_JobExceptions as parent left join js_JobExceptions as child on parent.JobExceptionId = child.ParentId
		order by
				 parent.JobExceptionId

		print 'Migrated data'
	end
go

set identity_insert tmpJobExceptions off
go

/****************************************
** REMOVE OLD TABLE AND RENAME NEW ONE **
****************************************/

drop table
	 js_JobExceptions
go

exec sp_rename 'dbo.tmpJobExceptions', 'js_JobExceptions'
go

exec sp_rename 'dbo.FK_tmpJobExceptions_JobExceptions', 'FK_js_JobExceptions_js_JobExceptions'
go

/**************************************************************************************************************************************************
 To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.
**************************************************************************************************************************************************/

begin transaction
set quoted_identifier on
set arithabort on
set numeric_roundabort off
set concat_null_yields_null on
set ansi_nulls on
set ansi_padding on
set ansi_warnings on
commit
begin transaction
go
alter table dbo.js_JobInstances set (lock_escalation = table)
go
commit
begin transaction
go
alter table dbo.js_JobExceptions
add
			constraint FK_js_JobExceptions_js_JobInstances foreign key
			(
			JobInstanceId
			) references dbo.js_JobInstances
			(
						 JobInstanceId
			) on update  cascade
			on delete  cascade

go
alter table dbo.js_JobExceptions set (lock_escalation = table)
go
commit


exec sp_updateextendedproperty
@name = N'js_Version',
@value = '2.4'
go