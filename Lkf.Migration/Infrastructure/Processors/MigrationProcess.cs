
using Lkf.Migration.Infrastructure.Settings;
using Lkf.Migration.Models;
using Nest;
using System.Globalization;

namespace Lkf.Migration.Infrastructure.Processors
{
    public class MigrationProcess : IMigrationProcess
    {
        private readonly ISettings _settings;

        public MigrationProcess(ISettings settings)
        {
            _settings = settings;
        }

        public bool Migrate()
        {
            // artist
            var artists = new Dictionary<string, string>();
            var lines = File.ReadAllLines($"{_settings.FilesPath}/artist");
            for (var i = 3; i < lines.Count() - 1; i++)
            {
                var values = lines[i].Split('\u0001');
                artists.Add(values[1], values[2]);
            }

            // collectionMatch
            var collectionMatch = new Dictionary<string, string>();
            var linesv = File.ReadAllLines($"{_settings.FilesPath}/collection_match");
            for (var i = 3; i < linesv.Count() - 1; i++)
            {
                var values = linesv[i].Split('\u0001');
                collectionMatch.Add(values[1], values[2]);
            }

            // artistCollection
            var linesAc = File.ReadAllLines($"{_settings.FilesPath}/artist_collection");
            var rows = new KeyValuePair<string, string>[linesAc.Count() - 4];
            for (var i = 3; i < linesAc.Count() - 1; i++)
            {
                var values = linesAc[i].Split('\u0001');
                rows[i - 3] = new KeyValuePair<string, string>(values[1], values[2]);
            }
            var artistCollection = rows.GroupBy(m => m.Key).Select(s => new { s.Key, a = s.ToList() }).ToList();


            // collection
            var collection = new List<Collection>();
            var linegg = File.ReadAllLines($"{_settings.FilesPath}/collection");
            for (var i = 3; i < linegg.Count() - 1; i++)
            {
                var values = linegg[i].Split('\u0001');
                collectionMatch.Add(values[1], values[2]);
            }

            var settings = new ConnectionSettings(new Uri(_settings.SearchEngineUrl));
            var client = new ElasticClient(settings);

            var er = client.Indices.Exists(Constants.INDEX_NAME);


            if (!er.Exists)
            {
                var cr = client.Indices.Create(Constants.INDEX_NAME, index => index.Map<Collection>(x =>x.AutoMap()));
                if (cr.IsValid)
                {
                    throw cr.OriginalException;
                }
            }

            return true;
        }
    }
}
