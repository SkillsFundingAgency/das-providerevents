// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Infrastructure.Mapping;
using StructureMap;
using StructureMap.TypeRules;

namespace SFA.DAS.Provider.Events.DataLockEventWorker
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Provider.Events";

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            
            RegisterMapper();
            RegisterMediator();
            ConfigureLogging();
        }

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        private void ConfigureLogging()
        {
            For<ILog>().Use(x => new NLogLogger(x.ParentType, null, null)).AlwaysUnique();
        }

        private void RegisterMapper()
        {
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("SFA.DAS.Data"));

            var mappingProfiles = new List<Profile>();

            foreach (var assembly in assemblies)
            {
                var profiles = Assembly.Load(assembly.FullName).GetTypes()
                                       .Where(t => typeof(Profile).IsAssignableFrom(t))
                                       .Where(t => t.IsConcrete() && t.HasConstructors())
                                       .Select(t => (Profile)Activator.CreateInstance(t));

                mappingProfiles.AddRange(profiles);
            }

            var config = new MapperConfiguration(cfg =>
            {
                mappingProfiles.ForEach(cfg.AddProfile);
                
            });

            
//            For<IConfigurationProvider>().Use(config).Singleton();
            For<IMapper>().Use(config.CreateMapper()).Singleton();
            For<MapperConfiguration>().Use(cfg => new MapperConfiguration(DomainAutoMapperConfiguration.AddDomainMappings));
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;
//using MediatR;
//using Microsoft.Azure;
//using SFA.DAS.NLog.Logger;
//using StructureMap;
//using StructureMap.TypeRules;

//namespace SFA.DAS.Provider.Events.DataLockEventWorker
//{
//    public class DefaultRegistry : Registry
//    {
//        private const string ServiceName = "SFA.DAS.Provider.Events";
//        //private string ServiceName = CloudConfigurationManager.GetSetting("ServiceName");
//        private const string Version = "1.0";

//        public DefaultRegistry()
//        {
//            Scan(scan =>
//            {
//                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
//                scan.RegisterConcreteTypesAgainstTheFirstInterface();
//            });

//            //var config = GetConfiguration();

//            //For<IDataConfiguration>().Use(config);
//            //RegisterRepositories(config.DatabaseConnectionString);
//            //RegisterApis(config);

//            RegisterEventCollectors();
//            RegisterEventHandlers();
//            RegisterEventProcessors();

//            RegisterMapper();

//            AddMediatrRegistrations();

//        }
        
//        private void RegisterEventHandlers()
//        {
//            //For<IEventHandler<GenericEvent<AccountCreatedEvent>>>().Use<AccountCreatedEventHandler>();
//            //For<IEventHandler<GenericEvent<AccountRenamedEvent>>>().Use<AccountRenamedEventHandler>();
//            //For<IEventHandler<ApprenticeshipEventView>>().Use<ApprenticeshipEventHandler>();
//            //For<IEventHandler<GenericEvent<LegalEntityCreatedEvent>>>().Use<LegalEntityCreatedEventHandler>();
//            //For<IEventHandler<GenericEvent<PayeSchemeAddedEvent>>>().Use<PayeSchemeAddedEventHandler>();
//            //For<IEventHandler<GenericEvent<PayeSchemeRemovedEvent>>>().Use<PayeSchemeRemovedEventHandler>();
//            //For<IEventHandler<GenericEvent<LevyDeclarationUpdatedEvent>>>().Use<LevyDeclarationUpdatedEventHandler>();
//            //For<IEventHandler<GenericEvent<AgreementSignedEvent>>>().Use<AgreementSignedEventHandler>();
//            //For<IEventHandler<PeriodEnd>>().Use<PeriodEndEventHandler>();
//            //For<IEventHandler<GenericEvent<EmploymentCheckCompleteEvent>>>().Use<EmploymentCheckCompleteEventHandler>();

//            ////Legacy support
//            //For<IEventHandler<AccountEventView>>().Use<AccountEventHandler>();
//        }

//        private void RegisterEventCollectors()
//        {
//            //For<IEventsCollector<GenericEvent<AccountCreatedEvent>>>().Use<GenericEventCollector<AccountCreatedEvent>>();
//            //For<IEventsCollector<GenericEvent<AccountRenamedEvent>>>().Use<GenericEventCollector<AccountRenamedEvent>>();
//            //For<IEventsCollector<ApprenticeshipEventView>>().Use<ApprenticeshipEventsCollector>();
//            //For<IEventsCollector<GenericEvent<LegalEntityCreatedEvent>>>().Use<GenericEventCollector<LegalEntityCreatedEvent>>();
//            //For<IEventsCollector<GenericEvent<PayeSchemeAddedEvent>>>().Use<GenericEventCollector<PayeSchemeAddedEvent>>();
//            //For<IEventsCollector<GenericEvent<PayeSchemeRemovedEvent>>>().Use<GenericEventCollector<PayeSchemeRemovedEvent>>();
//            //For<IEventsCollector<GenericEvent<LevyDeclarationUpdatedEvent>>>().Use<GenericEventCollector<LevyDeclarationUpdatedEvent>>();
//            //For<IEventsCollector<GenericEvent<AgreementSignedEvent>>>().Use<GenericEventCollector<AgreementSignedEvent>>();
//            //For<IEventsCollector<PeriodEnd>>().Use<PaymentEventsCollector>();
//            //For<IEventsCollector<GenericEvent<EmploymentCheckCompleteEvent>>>().Use<GenericEventCollector<EmploymentCheckCompleteEvent>>();

//            ////Legacy support
//            //For<IEventsCollector<AccountEventView>>().Use<AccountEventCollector>();
//        }

//        private void RegisterEventProcessors()
//        {
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<AccountCreatedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<AccountRenamedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<ApprenticeshipEventView>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<LegalEntityCreatedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<PayeSchemeAddedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<PayeSchemeRemovedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<LevyDeclarationUpdatedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<AgreementSignedEvent>>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<PeriodEnd>>();
//            //For<IEventsProcessor>().Use<EventsProcessor<GenericEvent<EmploymentCheckCompleteEvent>>>();

//            ////Legacy support
//            //For<IEventsProcessor>().Use<EventsProcessor<AccountEventView>>();
//        }

//        //private void RegisterApis(DataConfiguration config)
//        //{
//        //    For<IEventsApi>().Use(new EventsApi(config.EventsApi));
//        //    For<IPaymentsEventsApiClient>().Use(new PaymentsEventsApiClient(config.PaymentsEvents));
//        //    For<IAccountApiClient>().Use<AccountApiClient>().Ctor<IAccountApiConfiguration>().Is(config.AccountsApi);
//        //}

//        private void RegisterRepositories(string connectionString)
//        {
//            //For<IEventRepository>().Use<EventRepository>().Ctor<string>().Is(connectionString);
//            //For<IAccountRepository>().Use<AccountRepository>().Ctor<string>().Is(connectionString);
//            //For<ILegalEntityRepository>().Use<LegalEntityRepository>().Ctor<string>().Is(connectionString);
//            //For<IPayeSchemeRepository>().Use<PayeSchemeRepository>().Ctor<string>().Is(connectionString);
//            //For<IApprenticeshipRepository>().Use<ApprenticeshipRepository>().Ctor<string>().Is(connectionString);
//            //For<IPaymentRepository>().Use<PaymentRepository>().Ctor<string>().Is(connectionString);
//            //For<ILevyDeclarationRepository>().Use<LevyDeclarationRepository>().Ctor<string>().Is(connectionString);
//            //For<IEmployerAgreementRepository>().Use<EmployerAgreementRepository>().Ctor<string>().Is(connectionString);
//            //For<IEmploymentCheckRepository>().Use<EmploymentCheckRepository>().Ctor<string>().Is(connectionString);
//        }

//        private void AddMediatrRegistrations()
//        {
//            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
//            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

//            For<IMediator>().Use<Mediator>();
//        }

//        private void RegisterMapper()
//        {
//            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("SFA.DAS.Data"));

//            var mappingProfiles = new List<Profile>();

//            foreach (var assembly in assemblies)
//            {
//                var profiles = Assembly.Load(assembly.FullName).GetTypes()
//                                       .Where(t => typeof(Profile).IsAssignableFrom(t))
//                                       .Where(t => t.IsConcrete() && t.HasConstructors())
//                                       .Select(t => (Profile)Activator.CreateInstance(t));

//                mappingProfiles.AddRange(profiles);
//            }

//            var config = new MapperConfiguration(cfg =>
//            {
//                mappingProfiles.ForEach(cfg.AddProfile);
//            });

//            var mapper = config.CreateMapper();

//            For<IConfigurationProvider>().Use(config).Singleton();
//            For<IMapper>().Use(mapper).Singleton();
//        }

//        //private DataConfiguration GetConfiguration()
//        //{
//        //    var environment = CloudConfigurationManager.GetSetting("EnvironmentName");

//        //    var configurationRepository = GetConfigurationRepository();
//        //    var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(ServiceName, environment, Version));

//        //    return configurationService.Get<DataConfiguration>();
//        //}

//        //private static IConfigurationRepository GetConfigurationRepository()
//        //{
//        //    return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
//        //}

//    }
//}
