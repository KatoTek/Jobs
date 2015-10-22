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
alter table dbo.js_Jobs
	drop constraint DF_js_Jobs_AppId
go
alter table dbo.js_Jobs
	drop constraint DF_js_Jobs_AlertAfterTries
go
alter table dbo.js_Jobs
	drop constraint DF_js_Jobs_Enabled
go
alter table dbo.js_Jobs
	drop constraint DF_js_Jobs_Last
go
create table dbo.Tmp_js_Jobs
(
			 JobId int not null
					   identity (1, 1)
			,
			 JobName nvarchar(1000) not null
			,
			 AppId nvarchar(500) not null
			,
			 Description nvarchar(1000) null
			,
			 AlertAfterTries int not null
			,
			 Enabled bit not null
			,
			 LastRun datetime not null
)
on [PRIMARY]
go
alter table dbo.Tmp_js_Jobs set (lock_escalation = table)
go
alter table dbo.Tmp_js_Jobs
add
			constraint DF_js_Jobs_AppId default ('') for AppId
go
alter table dbo.Tmp_js_Jobs
add
			constraint DF_js_Jobs_AlertAfterTries default ((1)) for AlertAfterTries
go
alter table dbo.Tmp_js_Jobs
add
			constraint DF_js_Jobs_Enabled default ((1)) for Enabled
go
alter table dbo.Tmp_js_Jobs
add
			constraint DF_js_Jobs_Last default (getdate()) for LastRun
go
set identity_insert dbo.Tmp_js_Jobs on
go
if exists(select
				 *
		  from dbo.js_Jobs)
	begin
		exec ('INSERT INTO dbo.Tmp_js_Jobs (JobId, JobName, AppId, Description, AlertAfterTries, Enabled, LastRun)
		SELECT JobId, JobName, AppId, Description, AlertAfterTries, Enabled, LastRun FROM dbo.js_Jobs WITH (HOLDLOCK TABLOCKX)')
	end
go
set identity_insert dbo.Tmp_js_Jobs off
go
alter table dbo.js_JobSchedules
	drop constraint FK_js_JobSchedules_js_Jobs
go
alter table dbo.js_JobInstances
	drop constraint FK_js_JobInstances_js_Jobs
go
drop table
	 dbo.js_Jobs
go
execute sp_rename N'dbo.Tmp_js_Jobs', N'js_Jobs', 'OBJECT'
go
alter table dbo.js_Jobs
add
			constraint PK_js_Jobs primary key clustered
			(
			JobId
			)
			with( statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]

go
commit
begin transaction
go
alter table dbo.js_JobInstances
add
			constraint FK_js_JobInstances_js_Jobs foreign key
			(
			JobId
			) references dbo.js_Jobs
			(
						 JobId
			) on update  no action
			on delete  no action

go
alter table dbo.js_JobInstances set (lock_escalation = table)
go
commit
begin transaction
go
alter table dbo.js_JobSchedules
add
			constraint FK_js_JobSchedules_js_Jobs foreign key
			(
			JobId
			) references dbo.js_Jobs
			(
						 JobId
			) on update  cascade
			on delete  cascade

go
alter table dbo.js_JobSchedules set (lock_escalation = table)
go
commit
