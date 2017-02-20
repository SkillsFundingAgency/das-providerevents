-------------------------------------------------------------------------------------
DAS Data Lock Events Component - ILR Submission & DAS Period End
-------------------------------------------------------------------------------------

-------------------------------------------------------------------------------------
1. Package contents
------------------------------------------------------------------------------------- 
 1.1 DLLs:
  - component\CS.Common.External.dll
  - component\Dapper.dll
  - component\FastMember.dll
  - component\MediatR.dll
  - component\NLog.dll
  - component\SFA.DAS.Payments.DCFS.dll
  - component\SFA.DAS.Payments.DCFS.StructureMap.dll
  - component\SFA.DAS.Provider.Events.DataLock.dll
  - component\StructureMap.dll
 
 1.2 SQL scripts:
  - sql\ddl\datalock.transient.ddl.tables.sql:
   - transient database tables that need to be present when the component is executed
  - sql\ddl\datalock.transient.ddl.views.sql:
   - transient database views that need to be present when the component is executed
  
  - sql\ddl\datalock.transient.reference.ddl.tables.sql:
   - transient database tables that need to be present when the component is executed
  
  - sql\ddl\datalock.deds.ddl.tables.sql:
   - deds database tables that need to be present when the component is executed
  
  - for a post ILR Submission run:
   - sql\dml\01 datalock.populate.reference.provider.submission.sql:
    - populate reference providers data script
   - sql\dml\02 datalock.populate.reference.input.submission.sql:
    - populate reference input data script (ILR data lock results and price episode data and DAS Commitments data)
   - sql\dml\03 datalock.populate.reference.history.sql:
    - populate reference historical data (previous data lock events)
  
  - for a post DAS Period End run:
   - sql\dml\01 datalock.populate.reference.provider.periodend.sql:
    - populate reference providers data script
   - sql\dml\02 datalock.populate.reference.input.periodend.sql:
    - populate reference input data script (DAS Period End data lock results, ILR price episode data and DAS Commitments data)
   - sql\dml\03 datalock.populate.reference.history.sql:
    - populate reference historical data (previous data lock events)
 
 1.3 Copy to deds mapping xml:
  - copy mappings\DasDataLockEventsCopyToDedsMapping.xml:
   - sql bulk copy binary task configuration file that copies data lock events from transient to deds
   - SourceConnectionString: transient connection string
   - DestinationConnectionString: deds das provider events connection string
 
 1.4 Test results:
  - test-results\TestResult.xml
  - test-results\TestResult-Integration.xml
  
-------------------------------------------------------------------------------------
2. Expected context properties
-------------------------------------------------------------------------------------
 2.1 Transient database connection string:
  - key: TransientDatabaseConnectionString
  - value: DAS Provider Events transient database connection string
 2.2 Log level:
  - key: LogLevel
  - value: one of the following is valid: Fatal, Error, Warn, Info, Debug, Trace, Off
 2.3 ILR Collection Year:
  - key: YearOfCollection
  - value: 4 characters long string representing the academic year of the ILR connection: 1617, 1718, etc.
 2.4 Events Source:
  - key: DataLockEventsSource
  - value: one of the following is valid: Submission, PeriodEnd.

-------------------------------------------------------------------------------------
3. Expected data set keys / properties in the manifest that runs the component
-------------------------------------------------------------------------------------
 3.1 Post ILR Submission run
  3.1.1 ILR Collection: ${ILR_Deds.FQ}
  3.1.2 ILR Collection: ${DataLock_Deds.FQ}
  3.1.3 DAS Commitments Reference Data Collection: ${DAS_Commitments.FQ}
  3.1.4 DAS Provider Events Collection: ${DAS_ProviderEvents.FQ}
 3.2 Post DAS Period End run
  3.2.1 ILR Collection: ${ILR_Deds.FQ}
  3.2.2 DAS Period End Collection: ${DataLock_Deds.FQ}
  3.2.3 DAS Period End Collection: ${DAS_PeriodEnd.FQ}
  3.2.4 DAS Provider Events Collection: ${DAS_ProviderEvents.FQ}
  3.2.5 Academic year of current ILR Collection: ${YearOfCollection}

-------------------------------------------------------------------------------------
4. Expected manifest steps for the post ilr submission process
-------------------------------------------------------------------------------------
 4.1 Build the transient database.
 4.2 Copy reference data to transient using the '01 datalock.populate.reference.provider.submission.sql', '02 datalock.populate.reference.input.submission.sql' and '03 datalock.populate.reference.history.sql' sql scripts.
 4.3 Execute the 'DAS Data Lock Events Component' component
 4.4 Bulk copy the data lock events from transient to deds

-------------------------------------------------------------------------------------
5. Expected manifest steps for the post das period end process
-------------------------------------------------------------------------------------
 5.1 Build the transient database.
 5.2 Copy reference data to transient using the '01 datalock.populate.reference.provider.periodend.sql', '02 datalock.populate.reference.input.periodend.sql' and '03 datalock.populate.reference.history.sql' sql scripts.
 5.3 Execute the 'DAS Data Lock Events Component' component
 5.4 Bulk copy the data lock events from transient to deds


