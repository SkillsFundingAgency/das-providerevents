﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.Api.Client.UnitTests.PaymentsEventsApiClient
{
    public class WhenGettingPayments : ClientUnitTestBase
    {
        private const string ExpectedApiBaseUrl = "http://test.local.url/";
        private const string ClientToken = "super_secure_token";
        private Mock<IPaymentsEventsApiConfiguration> _configuration;
        private Payment _dasPayment;
        private Payment _nonDasPayment;
        private Client.PaymentsEventsApiClient _client;

        [SetUp]
        public void Arrange()
        {
            _configuration = new Mock<IPaymentsEventsApiConfiguration>();
            _configuration.Setup(m => m.ApiBaseUrl).Returns(ExpectedApiBaseUrl);
            _configuration.Setup(m => m.ClientToken).Returns(ClientToken);

            _dasPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 123456,
                Uln = 987654,
                EmployerAccountId = "ACC001",
                ApprenticeshipId = 918273,
                DeliveryPeriod = new CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Month = 9,
                    Year = 2017
                },
                EvidenceSubmittedOn = new DateTime(2017, 10, 1),
                EmployerAccountVersion = "A",
                ApprenticeshipVersion = "B",
                FundingSource = FundingSource.Levy,
                FundingAccountId = 69,
                TransactionType = TransactionType.Learning,
                Amount = 1234.56m,
                StandardCode = 25,
                ContractType = ContractType.ContractWithEmployer
            };

            _nonDasPayment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 654321,
                Uln = 987654,
                DeliveryPeriod = new CalendarPeriod
                {
                    Month = 8,
                    Year = 2017
                },
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Month = 9,
                    Year = 2017
                },
                EvidenceSubmittedOn = new DateTime(2017, 10, 1),
                FundingSource = FundingSource.CoInvestedSfa,
                TransactionType = TransactionType.Learning,
                Amount = 987.65m,
                FrameworkCode = 550,
                ProgrammeType = 20,
                PathwayCode = 6,
                ContractType = ContractType.ContractWithSfa
            };

            _httpMessageHandlerMock = SetupHttpMessageHandler(JsonConvert.SerializeObject(
                new PageOfResults<Payment>
                {
                    PageNumber = 1,
                    TotalNumberOfPages = 2,
                    Items = new[]
                    {
                        _dasPayment,
                        _nonDasPayment
                    }
                }));

            // use real http client with mocked handler
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _client = new Client.PaymentsEventsApiClient(_configuration.Object, httpClient);
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrl()
        {
            // Act
            await _client.GetPayments("XXX", "YYY", 2);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/payments?page=2&periodId=XXX&employerAccountId=YYY&ukprn=");
        }

        [Test]
        public async Task ThenItShouldCallTheCorrectUrlForUkprnFilter()
        {
            // Act
            await _client.GetPayments("XXX", "YYY", 2, 100);

            // Assert
            VerifyExpectedUrlCalled("http://test.local.url/api/payments?page=2&periodId=XXX&employerAccountId=YYY&ukprn=100");
        }


        [Test]
        public async Task ThenItShouldReturnResultFromApi()
        {
            // Act
            var actual = await _client.GetPayments();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(PaymentsMatch(_dasPayment, actual.Items[0]));
            Assert.IsTrue(PaymentsMatch(_nonDasPayment, actual.Items[1]));
        }

        private bool PaymentsMatch(Payment original, Payment client)
        {
            return original.Id == client.Id
                   && original.Ukprn == client.Ukprn
                   && original.Uln == client.Uln
                   && original.EmployerAccountId == client.EmployerAccountId
                   && original.ApprenticeshipId == client.ApprenticeshipId
                   && original.DeliveryPeriod.Month == client.DeliveryPeriod.Month
                   && original.DeliveryPeriod.Year == client.DeliveryPeriod.Year
                   && original.CollectionPeriod.Month == client.CollectionPeriod.Month
                   && original.CollectionPeriod.Year == client.CollectionPeriod.Year
                   && original.EvidenceSubmittedOn == client.EvidenceSubmittedOn
                   && original.EmployerAccountVersion == client.EmployerAccountVersion
                   && original.ApprenticeshipVersion == client.ApprenticeshipVersion
                   && original.FundingSource == client.FundingSource
                   && original.FundingAccountId == client.FundingAccountId
                   && original.TransactionType == client.TransactionType
                   && original.Amount == client.Amount
                   && original.StandardCode == client.StandardCode
                   && original.FrameworkCode == client.FrameworkCode
                   && original.ProgrammeType == client.ProgrammeType
                   && original.PathwayCode == client.PathwayCode
                   && original.ContractType == client.ContractType;
        }
    }
}
