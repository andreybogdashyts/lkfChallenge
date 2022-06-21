
using Lkf.Migration.Infrastructure.DataManager;
using Lkf.Migration.Infrastructure.SearchEngine;
using Lkf.Migration.Infrastructure.Settings;
using Lkf.Migration.Models;
using System.Diagnostics;

namespace Lkf.Migration.Infrastructure.Processors
{
    public class MigrationProcess : IMigrationProcess
    {
        private readonly ISettings _settings;

        private readonly ISearchEngine _searchEngine;
        private readonly IDataManager _dataManager;

        public MigrationProcess(ISettings settings, ISearchEngine searchEngine, IDataManager dataManager)
        {
            _settings = settings;
            _searchEngine = searchEngine;
            _dataManager = dataManager;
        }

        public async Task<bool> MigrateAsync()
        {
            var result = true;
            var exists = await _searchEngine.CollectionExists(Constants.INDEX_NAME);
            if (!exists)
            {
                if (! await _searchEngine.CreateCollection(Constants.INDEX_NAME))
                {
                    return false;
                }
            }
            
            await _dataManager.PrepareDictionariesAsync();

            Parallel.ForEach(_dataManager.BatchOptions, async (b, state) =>
            {
                var collections = _dataManager.GetCollections(b);
                result = await _searchEngine.SendBatch(Constants.INDEX_NAME, collections);
                if (!result)
                {
                    state.Break();
                }
            });
            return result;
        }
    }
}
