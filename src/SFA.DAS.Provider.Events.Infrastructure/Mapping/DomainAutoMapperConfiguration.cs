﻿using System.Collections.Generic;
using AutoMapper;
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

            cfg.CreateMap<PaymentEntity, Payment>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.EventId.ToString()))
                .ForMember(dst => dst.Ukprn, opt => opt.MapFrom(src => src.Ukprn))
                .ForMember(dst => dst.Uln, opt => opt.MapFrom(src => src.LearnerUln))
                .ForMember(dst => dst.EmployerAccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dst => dst.ApprenticeshipId, opt => opt.MapFrom(src => src.ApprenticeshipId))
                .ForMember(dst => dst.EvidenceSubmittedOn, opt => opt.MapFrom(src => src.IlrSubmissionDateTime))
                .ForMember(dst => dst.EmployerAccountVersion, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dst => dst.ApprenticeshipVersion, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dst => dst.FundingSource, opt => opt.MapFrom(src => (FundingSource) src.FundingSource))
                .ForMember(dst => dst.FundingAccountId, opt => opt.Ignore())
                .ForMember(dst => dst.TransactionType, opt => opt.MapFrom(src => (TransactionType) src.TransactionType))
                .ForMember(dst => dst.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dst => dst.StandardCode, opt => opt.MapFrom(src => src.LearningAimStandardCode))
                .ForMember(dst => dst.FrameworkCode, opt => opt.MapFrom(src => src.LearningAimFrameworkCode))
                .ForMember(dst => dst.ProgrammeType, opt => opt.MapFrom(src => src.LearningAimProgrammeType))
                .ForMember(dst => dst.PathwayCode, opt => opt.MapFrom(src => src.LearningAimPathwayCode))
                .ForMember(dst => dst.ContractType, opt => opt.MapFrom(src => (ContractType) src.ContractType))
                .ForMember(dst => dst.CollectionPeriod, opt => opt.MapFrom(src =>
                    new NamedCalendarPeriod
                    {
                        Id = GetCollectionPeriodName(src.AcademicYear, src.CollectionPeriod),
                        Month = GetMonthFromPaymentEntity(src.CollectionPeriod),
                        Year = GetYearFromPaymentEntity(src.AcademicYear, src.CollectionPeriod)
                    }))
                .ForMember(dst => dst.DeliveryPeriod, opt => opt.MapFrom(src =>
                    new CalendarPeriod
                    {
                        Month = GetMonthFromPaymentEntity(src.DeliveryPeriod),
                        Year = GetYearFromPaymentEntity(src.AcademicYear, src.DeliveryPeriod)
                    }))
                .ForMember(dst => dst.EarningDetails, opt => opt.MapFrom(src => new List<Earning>
                {
                    new Earning
                    {
                        StartDate = src.EarningsStartDate,
                        PlannedEndDate = src.EarningsPlannedEndDate.GetValueOrDefault(),
                        ActualEndDate = src.EarningsActualEndDate.GetValueOrDefault(),
                        CompletionStatus = src.EarningsCompletionStatus.GetValueOrDefault(),
                        CompletionAmount = src.EarningsCompletionAmount.GetValueOrDefault(),
                        MonthlyInstallment = src.EarningsInstalmentAmount.GetValueOrDefault(),
                        TotalInstallments = src.EarningsNumberOfInstalments.GetValueOrDefault(),
                        RequiredPaymentId = src.RequiredPaymentEventId
                    }
                }));

            cfg.CreateMap<PeriodEntity, CollectionPeriod>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => GetCollectionPeriodName(src.AcademicYear, src.Period)))
                .ForMember(dst => dst.PeriodName, opt => opt.MapFrom(src => GetCollectionPeriodName(src.AcademicYear, src.Period)))
                .ForMember(dst => dst.AcademicYear, opt => opt.MapFrom(src => src.AcademicYear))
                .ForMember(dst => dst.Period, opt => opt.MapFrom(src => src.Period))
                .ForMember(dst => dst.AccountDataValidAt, opt => opt.MapFrom(src => src.ReferenceDataValidationDate))
                .ForMember(dst => dst.CommitmentDataValidAt, opt => opt.MapFrom(src => src.ReferenceDataValidationDate))
                .ForMember(dst => dst.CompletionDateTime, opt => opt.MapFrom(src => src.CompletionDate))
                .ForMember(dst => dst.CalendarMonth, opt => opt.MapFrom(src => GetMonthFromPaymentEntity(src.Period)))
                .ForMember(dst => dst.CalendarYear, opt => opt.MapFrom(src => GetYearFromPaymentEntity(src.AcademicYear, src.Period)))
                ;

            cfg.CreateMap<PageOfResults<SubmissionEventEntity>, PageOfResults<SubmissionEvent>>();

            cfg.CreateMap<SubmissionEventEntity, SubmissionEvent>();

            cfg.CreateMap<PageOfResults<DataLockEventEntity>, PageOfResults<DataLockEvent>>();
            cfg.CreateMap<DataLockEventEntity, DataLockEvent>();
            cfg.CreateMap<DataLockEventErrorEntity, DataLockEventError>();
            cfg.CreateMap<DataLockEventPeriodEntity, DataLockEventPeriod>()
                .ForMember(dst => dst.Period, opt => opt.Ignore())
                .AfterMap((src, dst) =>
                {
                    dst.Period = new NamedCalendarPeriod
                    {
                        Id = src.CollectionPeriodId.Trim(),
                        Month = src.CollectionPeriodMonth,
                        Year = src.CollectionPeriodYear
                    };
                });

            cfg.CreateMap<DataLockEventApprenticeshipEntity, DataLockEventApprenticeship>();

            cfg.CreateMap<TransferEntity, AccountTransfer>()
                .ForMember(t => t.TransferId, o => o.MapFrom(s => s.Id))
                .ForMember(t => t.SenderAccountId, o => o.MapFrom(s => s.TransferSenderAccountId))
                .ForMember(t => t.ReceiverAccountId, o => o.MapFrom(s => s.AccountId))
                .ForMember(t => t.RequiredPaymentId, o => o.MapFrom(s => s.RequiredPaymentEventId))
                .ForMember(t => t.CommitmentId, o => o.MapFrom(s => s.ApprenticeshipId))
                .ForMember(t => t.Amount, o => o.MapFrom(s => s.Amount))
                .ForMember(t => t.CollectionPeriodName, o => o.MapFrom(s => GetCollectionPeriodName(s.AcademicYear, s.CollectionPeriod)))
                .AfterMap((s, t) => t.Type = TransferType.Levy);

            cfg.CreateMap<PageOfResults<TransferEntity>, PageOfResults<AccountTransfer>>();
        }

        private static string GetCollectionPeriodName(short academicYear, byte period)
        {
            return $"{academicYear}-R{period:D2}";
        }

        private static int GetYearFromPaymentEntity(short academicYear, byte period)
        {
            if (period < 6)
            {
                return int.Parse("20" + academicYear.ToString().Substring(0, 2));
            }
            else
            {
                return int.Parse("20" + academicYear.ToString().Substring(2, 2));
            }
        }

        private static int GetMonthFromPaymentEntity(byte period)
        {
            if (period == 13)
                return 9;
            if (period == 14)
                return 10;

            if (period < 6)
                return period + 7;
            return period - 5;
        }
    }
}