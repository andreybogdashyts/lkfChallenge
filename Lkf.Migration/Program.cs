using Autofac;
using Lkf.Migration.Infrastructure;
using Lkf.Migration.Infrastructure.IoC;
using Lkf.Migration.Infrastructure.Settings;
using Microsoft.Extensions.Logging;

namespace Lkf.Migration;
public class Program
{
    static int Main(string[] args)
    {
        ProcessResultType processResult = ProcessResultType.Success;
        Console.WriteLine("Migration to search engine has started");
        var container = ContainerConfig.Configure();
        using (var scope = container.BeginLifetimeScope())
        {
            try
            {
                var settings = scope.Resolve<ISettings>();
                
                //foreach (var ext in settings.SupportedExtensions)
                //{
                //    var watcher = scope.Resolve<IFileWatcher>();
                //    watcher.Initialize(ext);
                //}
                //new AutoResetEvent(false).WaitOne();
            }
            catch (Exception ex)
            {
                var logger = scope.Resolve<ILogger<Program>>();
                var error = $"Process has stopped with error: {ex}";
                logger.LogError(error);
                Console.Error.WriteLine(error);
                processResult = ProcessResultType.Fail;
            }
        }
        return (int)processResult;
    }
}
