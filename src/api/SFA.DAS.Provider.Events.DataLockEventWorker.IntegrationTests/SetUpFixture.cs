using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;
using NUnit.Framework;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.AcceptanceTests
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestDataHelperDeds.CreateDatabase();
            TestDataHelperDataLockEventStorage.CreateDatabase();
        }
    }
}
