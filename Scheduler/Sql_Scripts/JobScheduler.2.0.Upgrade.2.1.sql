/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.js_Jobs
	DROP CONSTRAINT DF_js_Jobs_AppId
GO
ALTER TABLE dbo.js_Jobs
	DROP CONSTRAINT DF_js_Jobs_Enabled
GO
ALTER TABLE dbo.js_Jobs
	DROP CONSTRAINT DF_js_Jobs_Last
GO
CREATE TABLE dbo.Tmp_js_Jobs
	(
	JobId int NOT NULL IDENTITY (1, 1),
	JobName nvarchar(100) NOT NULL,
	AppId nvarchar(100) NOT NULL,
	Description nvarchar(500) NULL,
	AlertAfterTries int NOT NULL,
	Enabled bit NOT NULL,
	LastRun datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_js_Jobs ADD CONSTRAINT
	DF_js_Jobs_AppId DEFAULT ('') FOR AppId
GO
ALTER TABLE dbo.Tmp_js_Jobs ADD CONSTRAINT
	DF_js_Jobs_AlertAfterTries DEFAULT 1 FOR AlertAfterTries
GO
ALTER TABLE dbo.Tmp_js_Jobs ADD CONSTRAINT
	DF_js_Jobs_Enabled DEFAULT ((1)) FOR Enabled
GO
ALTER TABLE dbo.Tmp_js_Jobs ADD CONSTRAINT
	DF_js_Jobs_Last DEFAULT (getdate()) FOR LastRun
GO
SET IDENTITY_INSERT dbo.Tmp_js_Jobs ON
GO
IF EXISTS(SELECT * FROM dbo.js_Jobs)
	 EXEC('INSERT INTO dbo.Tmp_js_Jobs (JobId, JobName, AppId, Description, Enabled, LastRun)
		SELECT JobId, JobName, AppId, Description, Enabled, LastRun FROM dbo.js_Jobs WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_js_Jobs OFF
GO
ALTER TABLE dbo.js_JobInstances
	DROP CONSTRAINT FK_js_JobInstances_js_Jobs
GO
ALTER TABLE dbo.js_JobSchedules
	DROP CONSTRAINT FK_js_JobSchedules_js_Jobs
GO
DROP TABLE dbo.js_Jobs
GO
EXECUTE sp_rename N'dbo.Tmp_js_Jobs', N'js_Jobs', 'OBJECT' 
GO
ALTER TABLE dbo.js_Jobs ADD CONSTRAINT
	PK_js_Jobs PRIMARY KEY CLUSTERED 
	(
	JobId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.js_JobSchedules ADD CONSTRAINT
	FK_js_JobSchedules_js_Jobs FOREIGN KEY
	(
	JobId
	) REFERENCES dbo.js_Jobs
	(
	JobId
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.js_JobInstances ADD CONSTRAINT
	FK_js_JobInstances_js_Jobs FOREIGN KEY
	(
	JobId
	) REFERENCES dbo.js_Jobs
	(
	JobId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
