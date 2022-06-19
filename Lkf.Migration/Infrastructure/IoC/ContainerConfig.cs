﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lkf.Migration.Infrastructure.DataParsers;
using Lkf.Migration.Infrastructure.Processors;
using Lkf.Migration.Infrastructure.SearchEngine;
using Lkf.Migration.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Lkf.Migration.Infrastructure.IoC
{
    public class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            var services = new ServiceCollection();
            services.AddLogging();

            builder.Populate(services);
            builder.RegisterType<Settings.Settings>().As<ISettings>();
            builder.RegisterType<MigrationProcess>().As<IMigrationProcess>();
            builder.RegisterType<DataImporter>().As<IDataImporter>();
            builder.RegisterType<ElasticSearchEngine>().As<ISearchEngine>();
            return builder.Build();
        }
    }
}
