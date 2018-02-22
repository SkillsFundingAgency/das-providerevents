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

using AutoMapper;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Infrastructure.Mapping;
using StructureMap;

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
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => ctx.GetInstance);
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => ctx.GetAllInstances);
            For<IMediator>().Use<Mediator>();
        }

        private void ConfigureLogging()
        {
            For<ILog>().Use(x => new NLogLogger(x.ParentType, null, null)).AlwaysUnique();
        }

        private void RegisterMapper()
        {
            
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

            
////            For<IConfigurationProvider>().Use(config).Singleton();
//            For<IMapper>().Use(config.CreateMapper()).Singleton();
            For<MapperConfiguration>().Use(cfg => new MapperConfiguration(DomainAutoMapperConfiguration.AddDomainMappings));
        }
    }
}
