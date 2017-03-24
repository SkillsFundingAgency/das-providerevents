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
  - sql\ddl\datalockevents.deds.ddl.tables.sql:
   - deds database tables that need to be present when the component is executed
  
  - for an ILR Submission run:
   - sql\ddl\datalockevents.transient.ddl.tables.sql:
    - transient database tables that need to be present when the component is executed
   - sql\ddl\datalockevents.transient.ddl.views.submission.sql:
    - transient database views that need to be present when the component is executed
   - sql\ddl\datalockevents.transient.reference.ddl.tables.sql:
    - transient database tables that need to be present when the component is executed
   - sql\dml\01 datalock.populate.reference.history.sql:
    - populate reference historical data (previous data lock events)

  - for a DAS Period End run:
   - sql\ddl\datalockevents.transient.ddl.tables.sql:
    - transient database tables that need to be present when the component is executed
   - sql\ddl\datalockevents.transient.ddl.views.periodend.sql:
    - transient database views that need to be present when the component is executed
   - sql\ddl\datalockevents.transient.reference.ddl.tables.sql:
    - transient database tables that need to be present when the component is executed
   - sql\dml\01 datalock.populate.reference.history.sql:
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
 3.1 DAS Provider Events Collection: ${DAS_ProviderEvents.FQ}

-------------------------------------------------------------------------------------
4. Expected manifest steps for the ilr submission process
-------------------------------------------------------------------------------------
 4.1 Build the transient database.
 4.2 Copy reference data to transient using the '01 datalock.populate.reference.history.sql' sql script.
 !!! Run the script only after all other populate reference data scripts related to DAS components have finished executing !!!
 4.3 Execute the 'DAS Data Lock Events Component' component
 4.4 Bulk copy the data lock events from transient to deds

-------------------------------------------------------------------------------------
5. Expected manifest steps for the das period end process
-------------------------------------------------------------------------------------
 5.1 Build the transient database.
 5.2 Copy reference data to transient using the '01 datalock.populate.reference.history.sql' sql script.
 !!! Run the script only after all other populate reference data scripts related to DAS components have finished executing !!!
 5.3 Execute the 'DAS Data Lock Events Component' component
 5.4 Bulk copy the data lock events from transient to deds


