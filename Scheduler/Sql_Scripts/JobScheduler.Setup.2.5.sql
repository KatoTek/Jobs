
if exists (select *
			   from sys.extended_properties
			   where name = 'js_Version') 
	begin
		raiserror ('JobScheduler already exists', 20, -1) with log
	end

/****************************************************************************************
***** Object:  Table [dbo].[js_JobExceptions]    Script Date: 11/24/2014 4:00:21 PM *****
****************************************************************************************/

set ansi_nulls on
go
set quoted_identifier on
go
create table [dbo].[js_JobExceptions](
			 [JobExceptionId] [int] identity(1,1)
									not null
			,
			 [JobInstanceId] [bigint] not null
			,
			 [ChildId] [int] null
			,
			 [Date] [datetime] null
			,
			 [Type] [nvarchar](500) null
			,
			 [Message] [nvarchar](max) null
			,
			 [Source] [nvarchar](500) null
			,
			 [StackTrace] [nvarchar](max) null
			,
			 [HelpLink] [nvarchar](500) null
			,
			 primary key clustered
			 (
			 [JobExceptionId] asc
			 )
				 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
)
on [PRIMARY] textimage_on [PRIMARY]

go

/***************************************************************************************
***** Object:  Table [dbo].[js_JobInstances]    Script Date: 11/24/2014 4:00:21 PM *****
***************************************************************************************/

set ansi_nulls on
go
set quoted_identifier on
go
create table [dbo].[js_JobInstances](
			 [JobInstanceId] [bigint] identity(1,1)
									  not null
			,
			 [JobId] [int] not null
			,
			 [Start] [datetime] not null
								constraint [DF_js_JobInstances_Start]  default (getdate())
			,
			 [End] [datetime] null
			,
			 constraint [PK_js_JobInstances] primary key clustered
			 (
			 [JobInstanceId] asc
			 )
				 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
)
on [PRIMARY]

go

/*******************************************************************************
***** Object:  Table [dbo].[js_Jobs]    Script Date: 11/24/2014 4:00:21 PM *****
*******************************************************************************/

set ansi_nulls on
go
set quoted_identifier on
go
create table [dbo].[js_Jobs](
			 [JobId] [int] identity(1,1)
						   not null
			,
			 [JobName] [nvarchar](1000) not null
			,
			 [AppId] [nvarchar](500) not null
									 constraint [DF_js_Jobs_AppId]  default ('')
			,
			 [Description] [nvarchar](1000) null
			,
			 [AlertAfterTries] [int] not null
									 constraint [DF_js_Jobs_AlertAfterTries]  default ((1))
			,
			 [Enabled] [bit] not null
							 constraint [DF_js_Jobs_Enabled]  default ((1))
			,
			 [LastRun] [datetime] not null
								  constraint [DF_js_Jobs_Last]  default (getdate())
			,
			 constraint [PK_js_Jobs] primary key clustered
			 (
			 [JobId] asc
			 )
				 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
)
on [PRIMARY]

go

/***************************************************************************************
***** Object:  Table [dbo].[js_JobSchedules]    Script Date: 11/24/2014 4:00:21 PM *****
***************************************************************************************/

set ansi_nulls on
go
set quoted_identifier on
go
create table [dbo].[js_JobSchedules](
			 [JobScheduleId] [int] identity(1,1)
								   not null
			,
			 [JobId] [int] not null
			,
			 [JobScheduleTypeId] [int] not null
			,
			 [RepeatInterval] [int] null
			,
			 [ScheduleStart] [datetime] not null
			,
			 [ScheduleEnd] [datetime] null
			,
			 [Sunday] [bit] not null
							constraint [DF_js_JobSchedules_Sunday]  default ((0))
			,
			 [Monday] [bit] not null
							constraint [DF_js_JobSchedules_Monday]  default ((0))
			,
			 [Tuesday] [bit] not null
							 constraint [DF_js_JobSchedules_Tuesday]  default ((0))
			,
			 [Wednesday] [bit] not null
							   constraint [DF_js_JobSchedules_Wednesday]  default ((0))
			,
			 [Thursday] [bit] not null
							  constraint [DF_js_JobSchedules_Thursday]  default ((0))
			,
			 [Friday] [bit] not null
							constraint [DF_js_JobSchedules_Friday]  default ((0))
			,
			 [Saturday] [bit] not null
							  constraint [DF_js_JobSchedules_Saturday]  default ((0))
			,
			 [Hour] [int] not null
						  constraint [DF_js_JobSchedules_Hour]  default ((0))
			,
			 [Minute] [int] not null
							constraint [DF_js_JobSchedules_Minute]  default ((0))
			,
			 [AllowedDelayDays] [int] not null
									  constraint [DF_js_JobSchedules_AllowedDelayDays]  default ((7))
			,
			 [January] [bit] not null
							 constraint [DF_js_JobSchedules_January]  default ((0))
			,
			 [February] [bit] not null
							  constraint [DF_js_JobSchedules_February]  default ((0))
			,
			 [March] [bit] not null
						   constraint [DF_js_JobSchedules_March]  default ((0))
			,
			 [April] [bit] not null
						   constraint [DF_js_JobSchedules_April]  default ((0))
			,
			 [May] [bit] not null
						 constraint [DF_js_JobSchedules_May]  default ((0))
			,
			 [June] [bit] not null
						  constraint [DF_js_JobSchedules_June]  default ((0))
			,
			 [July] [bit] not null
						  constraint [DF_js_JobSchedules_July]  default ((0))
			,
			 [August] [bit] not null
							constraint [DF_js_JobSchedules_August]  default ((0))
			,
			 [September] [bit] not null
							   constraint [DF_js_JobSchedules_September]  default ((0))
			,
			 [October] [bit] not null
							 constraint [DF_js_JobSchedules_October]  default ((0))
			,
			 [November] [bit] not null
							  constraint [DF_js_JobSchedules_November]  default ((0))
			,
			 [December] [bit] not null
							  constraint [DF_js_JobSchedules_December]  default ((0))
			,
			 [One] [bit] not null
						 constraint [DF_js_JobSchedules_One]  default ((0))
			,
			 [Two] [bit] not null
						 constraint [DF_js_JobSchedules_Two]  default ((0))
			,
			 [Three] [bit] not null
						   constraint [DF_js_JobSchedules_Three]  default ((0))
			,
			 [Four] [bit] not null
						  constraint [DF_js_JobSchedules_Four]  default ((0))
			,
			 [Five] [bit] not null
						  constraint [DF_js_JobSchedules_Five]  default ((0))
			,
			 [Six] [bit] not null
						 constraint [DF_js_JobSchedules_Six]  default ((0))
			,
			 [Seven] [bit] not null
						   constraint [DF_js_JobSchedules_Seven]  default ((0))
			,
			 [Eight] [bit] not null
						   constraint [DF_js_JobSchedules_Eight]  default ((0))
			,
			 [Nine] [bit] not null
						  constraint [DF_js_JobSchedules_Nine]  default ((0))
			,
			 [Ten] [bit] not null
						 constraint [DF_js_JobSchedules_Ten]  default ((0))
			,
			 [Eleven] [bit] not null
							constraint [DF_js_JobSchedules_Eleven]  default ((0))
			,
			 [Twelve] [bit] not null
							constraint [DF_js_JobSchedules_Twelve]  default ((0))
			,
			 [Thirteen] [bit] not null
							  constraint [DF_js_JobSchedules_Thirteen]  default ((0))
			,
			 [Fourteen] [bit] not null
							  constraint [DF_js_JobSchedules_Fourteen]  default ((0))
			,
			 [Fifteen] [bit] not null
							 constraint [DF_js_JobSchedules_Fifteen]  default ((0))
			,
			 [Sixteen] [bit] not null
							 constraint [DF_js_JobSchedules_Sixteen]  default ((0))
			,
			 [Seventeen] [bit] not null
							   constraint [DF_js_JobSchedules_Seventeen]  default ((0))
			,
			 [Eighteen] [bit] not null
							  constraint [DF_js_JobSchedules_Eighteen]  default ((0))
			,
			 [Nineteen] [bit] not null
							  constraint [DF_js_JobSchedules_Nineteen]  default ((0))
			,
			 [Twenty] [bit] not null
							constraint [DF_js_JobSchedules_Twenty]  default ((0))
			,
			 [TwentyOne] [bit] not null
							   constraint [DF_js_JobSchedules_TwentyOne]  default ((0))
			,
			 [TwentyTwo] [bit] not null
							   constraint [DF_js_JobSchedules_TwentyTwo]  default ((0))
			,
			 [TwentyThree] [bit] not null
								 constraint [DF_js_JobSchedules_TwentyThree]  default ((0))
			,
			 [TwentyFour] [bit] not null
								constraint [DF_js_JobSchedules_TwentyFour]  default ((0))
			,
			 [TwentyFive] [bit] not null
								constraint [DF_js_JobSchedules_TwentyFive]  default ((0))
			,
			 [TwentySix] [bit] not null
							   constraint [DF_js_JobSchedules_TwentySix]  default ((0))
			,
			 [TwentySeven] [bit] not null
								 constraint [DF_js_JobSchedules_TwentySeven]  default ((0))
			,
			 [TwentyEight] [bit] not null
								 constraint [DF_js_JobSchedules_TwentyEight]  default ((0))
			,
			 [TwentyNine] [bit] not null
								constraint [DF_js_JobSchedules_TwentyNine]  default ((0))
			,
			 [Thirty] [bit] not null
							constraint [DF_js_JobSchedules_Thirty]  default ((0))
			,
			 [ThirtyOne] [bit] not null
							   constraint [DF_js_JobSchedules_ThirtyOne]  default ((0))
			,
			 [LastDayOfMonth] [bit] not null
									constraint [DF_js_JobSchedules_LastDayOfMonth]  default ((0))
			,
			 constraint [PK_js_JobSchedules] primary key clustered
			 (
			 [JobScheduleId] asc
			 )
				 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
)
on [PRIMARY]

go

/*******************************************************************************************
***** Object:  Table [dbo].[js_JobScheduleTypes]    Script Date: 11/24/2014 4:00:21 PM *****
*******************************************************************************************/

set ansi_nulls on
go
set quoted_identifier on
go
create table [dbo].[js_JobScheduleTypes](
			 [JobScheduleTypeId] [int] identity(1,1)
									   not null
			,
			 [Type] [nvarchar](100) not null
			,
			 constraint [PK_js_JobScheduleTypes] primary key clustered
			 (
			 [JobScheduleTypeId] asc
			 )
				 with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
)
on [PRIMARY]

go
alter table [dbo].[js_JobExceptions]
		with check
add
			 constraint [FK_js_JobExceptions_js_JobExceptions] foreign key([ChildId])
			 references [dbo].[js_JobExceptions] (
						[JobExceptionId])
go
alter table [dbo].[js_JobExceptions] check constraint
			[FK_js_JobExceptions_js_JobExceptions]
go
alter table [dbo].[js_JobExceptions]
		with check
add
			 constraint [FK_js_JobExceptions_js_JobInstances] foreign key([JobInstanceId])
			 references [dbo].[js_JobInstances] (
						[JobInstanceId])
			 on update cascade
			 on delete cascade
go
alter table [dbo].[js_JobExceptions] check constraint
			[FK_js_JobExceptions_js_JobInstances]
go
alter table [dbo].[js_JobInstances]
		with check
add
			 constraint [FK_js_JobInstances_js_Jobs] foreign key([JobId])
			 references [dbo].[js_Jobs] (
						[JobId])
go
alter table [dbo].[js_JobInstances] check constraint
			[FK_js_JobInstances_js_Jobs]
go
alter table [dbo].[js_JobSchedules]
		with check
add
			 constraint [FK_js_JobSchedules_js_Jobs] foreign key([JobId])
			 references [dbo].[js_Jobs] (
						[JobId])
			 on update cascade
			 on delete cascade
go
alter table [dbo].[js_JobSchedules] check constraint
			[FK_js_JobSchedules_js_Jobs]
go
alter table [dbo].[js_JobSchedules]
		with check
add
			 constraint [FK_js_JobSchedules_js_JobScheduleTypes] foreign key([JobScheduleTypeId])
			 references [dbo].[js_JobScheduleTypes] (
						[JobScheduleTypeId])
go
alter table [dbo].[js_JobSchedules] check constraint
			[FK_js_JobSchedules_js_JobScheduleTypes]
go

set identity_insert [dbo].[js_JobScheduleTypes] on

go
insert [dbo].[js_JobScheduleTypes] (
	   [JobScheduleTypeId]
	  , [Type])
values
	   (
	   1
	  , N'Minutely')
go
insert [dbo].[js_JobScheduleTypes] (
	   [JobScheduleTypeId]
	  , [Type])
values
	   (
	   2
	  , N'Hourly')
go
insert [dbo].[js_JobScheduleTypes] (
	   [JobScheduleTypeId]
	  , [Type])
values
	   (
	   3
	  , N'Weekly')
go
insert [dbo].[js_JobScheduleTypes] (
	   [JobScheduleTypeId]
	  , [Type])
values
	   (
	   4
	  , N'Monthly')
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

exec sp_addextendedproperty
@name = N'js_Version',
@value = '2.5'
go