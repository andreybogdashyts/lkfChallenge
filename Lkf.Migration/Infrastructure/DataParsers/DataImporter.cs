
using Lkf.Migration.Infrastructure.Settings;
using Lkf.Migration.Models;

namespace Lkf.Migration.Infrastructure.DataParsers
{
    public class DataImporter : IDataImporter
    {
        private const char DELIMITER = '\u0001';

        private readonly ISettings _settings;
        public DataImporter(ISettings settings)
        {
            _settings = settings;
        }
        public IEnumerable<Collection> Import()
        {
            var artists = new Dictionary<string, string>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/artist"))
            {
                AddToCollection(artists, l);
            }

            var collectionMatch = new Dictionary<string, string>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/collection_match"))
            {
                AddToCollection(collectionMatch, l);
            }

            var rows = new List<KeyValuePair<string, string>>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/artist_collection"))
            {
                AddToCollection(rows, l);
            }
            var artistCollection = rows.GroupBy(m => m.Key).ToDictionary(m => m.Key, m => m.ToList());


            var collections = new List<Collection>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/collection"))
            {
                var values = l.Split(DELIMITER);
                collections.Add(new Collection
                {
                   
                });
            }

            return new List<Collection>();
        }

        private void AddToCollection<T>(T list, string line) where T : ICollection<KeyValuePair<string, string>>
        {
            var values = line.Split(DELIMITER);
            if (values.Count() >= 2)
            {
                //TODO: find indeces by name instead of hardcoded numbers
                list.Add(new KeyValuePair<string, string>(values[1], values[2]));
            }
        }
    }
}
