using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Infrastructure.Mapping
{
    public static class DomainAutoMapperConfiguration
    {
        public static void AddDomainMappings(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageOfResults<PaymentEntity>, PageOfResults<Payment>>();

            cfg.CreateMap<PaymentsDueEarningEntity, Earning>();

            cfg.CreateMap<PaymentEntity, Payment>()
                .ForMember(dst => dst.CollectionPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.FundingSource, opt => opt.Ignore())
                .ForMember(dst => dst.TransactionType, opt => opt.Ignore())
                .ForMember(dst => dst.EarningDetails, opt => opt.MapFrom(x => x.PaymentsDueEarningEntities))
                .AfterMap((src, dst) =>
                {
                    dst.CollectionPeriod = new NamedCalendarPeriod
                    {
                        Id = src.CollectionPeriodId.Trim(),
                        Month = src.CollectionPeriodMonth,
                        Year = src.CollectionPeriodYear
                    };
                    dst.DeliveryPeriod = new CalendarPeriod
                    {
                        Month = src.DeliveryPeriodMonth,
                        Year = src.DeliveryPeriodYear
                    };
                    dst.FundingSource = (FundingSource)src.FundingSource;
                    dst.TransactionType = (TransactionType)src.TransactionType;
                });

            cfg.CreateMap<PeriodEntity, Period>();


            cfg.CreateMap<PageOfResults<SubmissionEventEntity>, PageOfResults<SubmissionEvent>>();

            cfg.CreateMap<SubmissionEventEntity, SubmissionEvent>();

            cfg.CreateMap<DataLock, DataLockEntity>()
                .ForMember(dst => dst.ErrorCodes, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    if (src.ErrorCodes == null || src.ErrorCodes.Count == 0)
                        dst.ErrorCodes = null;
                    else
                        dst.ErrorCodes = JsonConvert.SerializeObject(src.ErrorCodes);
                });

            cfg.CreateMap<DataLockEntity, DataLock>()
                .ForMember(dst => dst.ErrorCodes, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    if (string.IsNullOrEmpty(src.ErrorCodes))
                        dst.ErrorCodes = null;
                    else
                        dst.ErrorCodes = JsonConvert.DeserializeObject<List<string>>(src.ErrorCodes);
                });

            cfg.CreateMap<PageOfResults<DataLockEventEntity>, PageOfResults<DataLockEvent>>();

            cfg.CreateMap<DataLockEventEntity, DataLockEvent>()
                .ForMember(dst => dst.ApprenticeshipId, o => o.MapFrom(src => src.CommitmentId))
                .ForMember(dst => dst.Errors, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    if (!string.IsNullOrEmpty(src.ErrorCodes))
                    {
                        dst.Errors = JsonConvert.DeserializeObject<List<string>>(src.ErrorCodes).Select(s => new DataLockEventError
                        {
                            ErrorCode = s,
                            SystemDescription = GetErrorDescription(s)
                        }).ToArray();
                    }
                    else
                    {
                        dst.Errors = null;
                    }
                });


            cfg.CreateMap<DataLockEvent, DataLockEventEntity>()
                .ForMember(dst => dst.CommitmentId, o => o.MapFrom(src => src.ApprenticeshipId))
                .ForMember(dst => dst.ErrorCodes, o => o.Ignore())
                .AfterMap((src, dst) =>
                {
                    if (src.Errors == null || src.Errors.Length == 0)
                        dst.ErrorCodes = null;
                    else
                        dst.ErrorCodes = JsonConvert.SerializeObject(src.Errors.Select(e => e.ErrorCode).ToList());
                });
        }

        private static string GetErrorDescription(string errorCode)
        {
            switch (errorCode)
            {
                case "DLOCK_01":
                    return "No matching record found in an employer digital account for the UKPRN";
                case "DLOCK_02":
                    return "No matching record found in the employer digital account for the ULN";
                case "DLOCK_03":
                    return "No matching record found in the employer digital account for the standard code";
                case "DLOCK_04":
                    return "No matching record found in the employer digital account for the framework code";
                case "DLOCK_05":
                    return "No matching record found in the employer digital account for the programme type";
                case "DLOCK_06":
                    return "No matching record found in the employer digital account for the pathway code";
                case "DLOCK_07":
                    return "No matching record found in the employer digital account for the negotiated cost of training";
                case "DLOCK_08":
                    return "Multiple matching records found in the employer digital account";
                case "DLOCK_09":
                    return "The start date for this negotiated price is before the corresponding price start date in the employer digital account";
                case "DLOCK_10":
                    return "The employer has stopped payments for this apprentice";
                case "DLOCK_11":
                    return "The employer is not currently a levy payer";
                default:
                    return errorCode;
            }
        }
    }
}
