using Dapper;
using SFA.DAS.Provider.Events.Submission.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Provider.Events.Submission.IntegrationTests.Helpers
{
    public class TestDataHelper
    {
        internal static void PopulateLastSeen(SqlConnection dedsConnection, IlrDetails ilrDetails)
        {
            var command = "INSERT INTO Submissions.LastSeenVersion " +
                      "(IlrFileName,FileDateTime,SubmittedDateTime,ComponentVersionNumber,UKPRN,ULN,LearnRefNumber,AimSeqNumber,PriceEpisodeIdentifier," +
                      "StandardCode,ActualStartDate,PlannedEndDate,OnProgrammeTotalPrice,CompletionTotalPrice,AcademicYear) " +
                      "VALUES " +
                      "(@IlrFileName,@FileDateTime,@SubmittedDateTime,@ComponentVersionNumber,@UKPRN,@ULN,@LearnRefNumber,@AimSeqNumber,@PriceEpisodeIdentifier," +
                      "@StandardCode,@ActualStartDate,@PlannedEndDate,@OnProgrammeTotalPrice,@CompletionTotalPrice,@AcademicYear)";

            dedsConnection.Execute(command, new
            {
                IlrFileName = ilrDetails.IlrFileName,
                FileDateTime = ilrDetails.FileDateTime,
                SubmittedDateTime = ilrDetails.SubmittedDateTime,
                ComponentVersionNumber = 1,
                UKPRN = ilrDetails.Ukprn,
                ULN = ilrDetails.Uln,
                LearnRefNumber = ilrDetails.LearnRefNumber,
                AimSeqNumber = 1,
                PriceEpisodeIdentifier = ilrDetails.PriceEpisodeIdentifier,
                StandardCode = 34,
                ActualStartDate = ilrDetails.ActualStartDate,
                PlannedEndDate = ilrDetails.PlannedEndDate,
                OnProgrammeTotalPrice = ilrDetails.OnProgrammeTotalPrice,
                CompletionTotalPrice = ilrDetails.CompletionTotalPrice ,
                AcademicYear = ilrDetails.AcademicYear
            });
        }


    }

    
}
