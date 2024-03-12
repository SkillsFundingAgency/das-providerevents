CREATE TABLE [DataLock].[DataLockEventCommitmentVersions](
    [DataLockEventId] [uniqueidentifier] NOT NULL,
    [CommitmentVersion] [varchar](25) NOT NULL,
    [CommitmentStartDate] [date] NOT NULL,
    [CommitmentStandardCode] [bigint] NULL,
    [CommitmentProgrammeType] [int] NULL,
    [CommitmentFrameworkCode] [int] NULL,
    [CommitmentPathwayCode] [int] NULL,
    [CommitmentNegotiatedPrice] [decimal](12, 5) NOT NULL,
    [CommitmentEffectiveDate] [date] NOT NULL
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DataLockEventCommitmentVersions_DataLockEventId] ON [DataLock].[DataLockEventCommitmentVersions]
(
    [DataLockEventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DataLockEventCommitmentVersions_DataLockId] ON [DataLock].[DataLockEventCommitmentVersions]
(
    [DataLockEventId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO