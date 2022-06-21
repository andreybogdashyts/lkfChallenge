using Autofac;
using Lkf.Migration.Infrastructure;
using Lkf.Migration.Infrastructure.IoC;
using Lkf.Migration.Infrastructure.Processors;
using Microsoft.Extensions.Logging;

namespace Lkf.Migration;
public class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Search engine migration has started");
        var processResult = ProcessResultType.Success;

        using (var scope = ContainerConfig.Configure().BeginLifetimeScope())
        {
            try
            {
                var mp = scope.Resolve<IMigrationProcess>();
                var r = await mp.MigrateAsync();
                processResult =  r ? ProcessResultType.Success : ProcessResultType.Fail;
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
