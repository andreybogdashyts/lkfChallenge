
using Lkf.Migration.Infrastructure.DataParsers;
using Lkf.Migration.Infrastructure.SearchEngine;

namespace Lkf.Migration.Infrastructure.Processors
{
    public class MigrationProcess : IMigrationProcess
    {
        private readonly ISearchEngine _searchEngine;
        private readonly IDataImporter _dataImporter;

        public MigrationProcess(ISearchEngine searchEngine, IDataImporter dataImporter)
        {
            _searchEngine = searchEngine;
            _dataImporter = dataImporter;
        }

        public bool Migrate()
        {        
            var exists = _searchEngine.CollectionExists(Constants.INDEX_NAME);
            if (!exists)
            {
                _searchEngine.CreateCollection(Constants.INDEX_NAME);
            }

            var data = _dataImporter.Import();

            return true;
        }
    }
}
