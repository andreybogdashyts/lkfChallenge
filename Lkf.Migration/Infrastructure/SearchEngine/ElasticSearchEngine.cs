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
            return r.IsValid;
        }

        public async Task<bool> CreateCollection(string name)
        {
            var cr = await _client.Indices.CreateAsync(name, index => index.Map<Collection>(x => x.AutoMap()));
            if (cr.OriginalException != null)
            {
                throw cr.OriginalException;
            }
            return cr.IsValid;
        }

        public async Task<bool> SendBatch(string name, List<Collection> collections)
        {
            var br = await _client.BulkAsync(b => b
               .Index(name)
               .IndexMany(collections));
            if (br.OriginalException != null)
            {
                throw br.OriginalException;
            }
            return br.IsValid;
        }
    }
}
