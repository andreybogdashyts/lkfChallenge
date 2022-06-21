using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lkf.Migration.Infrastructure.DataManager;
using Lkf.Migration.Infrastructure.Processors;
using Lkf.Migration.Infrastructure.Readers;
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
            builder.RegisterType<ElasticSearchEngine>().As<ISearchEngine>();
            builder.RegisterType<FileDataReader>().As<IDataReader>();
            builder.RegisterType<MusicCollectionDataManager>().As<IDataManager>();
            return builder.Build();
        }
    }
}
