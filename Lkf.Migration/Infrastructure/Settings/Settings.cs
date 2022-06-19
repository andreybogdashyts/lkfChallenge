using System.Configuration;

namespace Lkf.Migration.Infrastructure.Settings
{
    public class Settings : ISettings
    {
        public string SearchEngineUrl => ConfigurationManager.AppSettings["SearchEngineUrl"];
        public string FilesPath => ConfigurationManager.AppSettings["FilesPath"];
    }
}
