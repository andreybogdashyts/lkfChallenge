namespace Lkf.Migration.Infrastructure.Processors
{
    public interface IMigrationProcess
    {
        Task<bool> Migrate();
    }
}
