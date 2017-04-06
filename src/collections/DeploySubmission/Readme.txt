-------------------------------------------------------------------------------------
DAS Submission Events Component
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
  - component\SFA.DAS.Provider.Events.Submission.dll
  - component\StructureMap.dll
 
 1.2 SQL scripts:
  - sql\ddl\submissions.transient.ddl.tables.sql:
   - transient database tables that need to be present when the component is executed
  - sql\ddl\submissions.transient.ddl.views.sql:
   - transient database views that need to be present when the component is executed
  - sql\ddl\submissions.transient.ddl.functions.sql:
   - transient database functions that need to be present when the component is executed
  
  - sql\ddl\submissions.deds.ddl.tables.sql:
   - deds database tables that need to be present when the component is executed
  
  - sql\dml\01 submissions.populate.submissions.sql:
   - populate latest existing events script
 
 1.3 Copy to deds mapping xml:
  - copy mappings\DasSubmissionEventsCopyToDedsMapping.xml:
   - sql bulk copy binary task configuration file that copies submission events from transient to deds
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

-------------------------------------------------------------------------------------
3. Expected data set keys / properties in the manifest that runs the component
-------------------------------------------------------------------------------------
 3.1 ILR Collection: ${ILR_Deds.FQ}
 3.2 DAS Provider Events Collection: ${DAS_ProviderEvents.FQ}

-------------------------------------------------------------------------------------
4. Expected manifest steps for the post ilr submission process
-------------------------------------------------------------------------------------
 4.1 Build the transient database.
 4.2 Copy reference data to transient using the '01 submissions.populate.submissions.sql' sql script.
 4.3 Execute the 'DAS Submission Events Component' component
 4.4 Cleanup the deds data lock results using the '02 Submissions.Cleanup.Deds.DML.sql' sql script
 4.5 Bulk copy the submission events from transient to deds

