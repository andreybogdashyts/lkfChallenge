using Lkf.Migration.Infrastructure.Settings;
using Lkf.Migration.Models;
using Nest;

namespace Lkf.Migration.Infrastructure.SearchEngine
{
    public class ElasticSearchEngine : ISearchEngine
    {
        private readonly ElasticClient _client;
        private readonly ISettings _settings;
        public ElasticSearchEngine(ISettings settings)
        {
            _settings = settings;
            _client = new ElasticClient(new ConnectionSettings(new Uri(_settings.SearchEngineUrl)));
        }

        public async Task<bool> CollectionExists(string name)
        {
            var r = await _client.Indices.ExistsAsync(name);
            if (r.OriginalException != null)
            {
                throw r.OriginalException;
            }
            return r.Exists;
        }

        public async Task CreateCollection(string name)
        {
            var cr = await _client.Indices.CreateAsync(name, index => index.Map<Collection>(x => x.AutoMap()));
            if (!cr.IsValid)
            {
                throw cr.OriginalException;
            }
        }

        public async Task SendBatch(string name, List<Collection> collections)
        {
            var br = await _client.BulkAsync(b => b
               .Index(name)
               .IndexMany(collections));
            if (!br.IsValid)
            {
                throw br.OriginalException;
            }
        }
    }
}
