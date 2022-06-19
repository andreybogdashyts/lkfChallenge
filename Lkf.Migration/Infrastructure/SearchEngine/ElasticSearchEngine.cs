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

        public bool CollectionExists(string name)
        {
            var r = _client.Indices.Exists(name);
            if (r.IsValid)
            {
                throw r.OriginalException;
            }
            return r.Exists;
        }

        public void CreateCollection(string name)
        {
            var cr = _client.Indices.Create(name, index => index.Map<Collection>(x => x.AutoMap()));
            if (cr.IsValid)
            {
                throw cr.OriginalException;
            }
        }
    }
}
