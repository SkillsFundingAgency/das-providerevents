CREATE TABLE [DataLock].[DataLockEventErrors](
	[DataLockEventId] [uniqueidentifier] NOT NULL,
	[ErrorCode] [varchar](15) NOT NULL,
	[SystemDescription] [nvarchar](255) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [DataLock].[DataLockEventErrors] ADD PRIMARY KEY CLUSTERED 
(
	[DataLockEventId] ASC,
	[ErrorCode] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO