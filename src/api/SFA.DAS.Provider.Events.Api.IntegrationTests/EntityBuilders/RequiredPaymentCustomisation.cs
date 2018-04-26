using Ploeh.AutoFixture;
using SFA.DAS.Provider.Events.Api.IntegrationTests.RawEntities;

namespace SFA.DAS.Provider.Events.Api.IntegrationTests.EntityBuilders
{
    class RequiredPaymentCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
            {
                var requiredPayment = fixture
                    .Build<ItRequiredPayment>()
                    .Create();
                foreach (var payment in requiredPayment.Payments)
                {
                    payment.RequiredPaymentId = requiredPayment.Id;
                    foreach (var earning in payment.Earnings)
                    {
                        earning.RequiredPaymentId = payment.RequiredPaymentId;
                    }
                }

                foreach (var transfer in requiredPayment.Transfers)
                {
                    transfer.RequiredPaymentId = requiredPayment.Id;
                    transfer.Amount = requiredPayment.AmountDue;
                    transfer.ReceivingAccountId = requiredPayment.AccountId;
                    transfer.CommitmentId = requiredPayment.CommitmentId;
                    transfer.CollectionPeriodName = requiredPayment.CollectionPeriodName;
                }
                return requiredPayment;
            });
        }
    }
}
