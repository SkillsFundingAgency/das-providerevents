CREATE TABLE [dbo].[VersionInfo](
	[Version] [int] NOT NULL,
	[Date] [date] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VersionInfo] ADD  DEFAULT ((0)) FOR [Version]
GO