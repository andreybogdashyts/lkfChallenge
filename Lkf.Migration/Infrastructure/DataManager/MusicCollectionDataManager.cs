using Lkf.Migration.Infrastructure.Readers;
using Lkf.Migration.Infrastructure.Settings;
using Lkf.Migration.Models;
namespace Lkf.Migration.Infrastructure.DataManager
{
    public class MusicCollectionDataManager : IDataManager
    {
        private const char DELIMITER = '\u0001';
        private const int HEADER_RECORD_COUNT = 3;
        private const int RECORDS_PER_BATCH = 10000;

        private readonly ISettings _settings;
        private readonly IDataReader _dataReader;
        private Dictionary<string, string> _artistsDict;
        private Dictionary<string, string> _collectionMatchDict;
        private Dictionary<string, IEnumerable<string>> _artistCollectionDict;

        public MusicCollectionDataManager(ISettings settings, IDataReader dataReader)
        {
            _settings = settings;
            _dataReader = dataReader;
            _artistsDict = new Dictionary<string, string>();
            _collectionMatchDict = new Dictionary<string, string>();
            _artistCollectionDict = new Dictionary<string, IEnumerable<string>>();

            BatchOptions = new List<BatchOptions>();
        }

        public List<BatchOptions> BatchOptions { get; set; }

        public List<Collection> GetCollections(BatchOptions options)
        {
            var path = $"{_settings.FilesPath}/collection";
            var collections = new List<Collection>();
            foreach (var l in _dataReader.ReadLines(path, options.Skip , options.TakeCount))
            {
                var values = l.Split(DELIMITER);
                //TODO: come up with better validation
                if (values.Count() <= 16)
                {
                    Console.WriteLine(l);
                    continue;
                }
                var collectionId = values[1];
                _collectionMatchDict.TryGetValue(collectionId, out var upc);
                _artistCollectionDict.TryGetValue(collectionId, out var artists);

                var arts = artists?.Select(s =>
                {
                    _artistsDict.TryGetValue(s, out var name);
                    var a = new Artist
                    {
                        Id = s,
                        Name = name
                    };
                    return a;
                }).ToList();

                //TODO: find indeces by name instead of hardcoded numbers
                collections.Add(new Collection
                {
                    Id = collectionId,
                    Name = values[2],
                    Url = values[3],
                    Upc = upc,
                    ReleaseDate = values[9],
                    IsCompilation = values[16],
                    Label = values[11],
                    ImageUrl = values[7],
                    Artists = arts
                });
            }
            return collections;
        }

        public Task PrepareDictionariesAsync()
        {
            var tasks = new List<Task>();
            tasks.Add(Task.Run(() =>
            {
                var path = $"{_settings.FilesPath}/artist";
                var lines = GetLinesCount(path);
                // to prevent extra allocations create dictionary with capacity
                _artistsDict = new Dictionary<string, string>(lines - HEADER_RECORD_COUNT);
                foreach (var l in _dataReader.ReadLines(path, skip: HEADER_RECORD_COUNT))
                {
                    //TODO: find indeces by name instead of hardcoded numbers
                    AddToCollection(_artistsDict, l, m => m[1], m => m[2]);
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                var path = $"{_settings.FilesPath}/collection_match";
                var lines = GetLinesCount(path);
                _collectionMatchDict = new Dictionary<string, string>(lines - HEADER_RECORD_COUNT);
                foreach (var l in _dataReader.ReadLines(path, skip: HEADER_RECORD_COUNT))
                {
                    AddToCollection(_collectionMatchDict, l, m => m[1], m => m[2]);
                }
            }));
            tasks.Add(Task.Run(() =>
            {
                var path = $"{_settings.FilesPath}/artist_collection";
                var lines = GetLinesCount(path);
                var rows = new List<KeyValuePair<string, string>>(lines - HEADER_RECORD_COUNT);
                foreach (var l in _dataReader.ReadLines(path, skip: HEADER_RECORD_COUNT))
                {
                    AddToCollection(rows, l, m => m[2], m => m[1]);
                }
                _artistCollectionDict = rows.GroupBy(m => m.Key).ToDictionary(m => m.Key, m => m.Select(s => s.Value));
            }));
            tasks.Add(Task.Run(() =>
            {
                var path = $"{_settings.FilesPath}/collection";
                var lines = GetLinesCount(path);
                BatchOptions = SplitToBatchOptions(lines);
            }));
            return Task.WhenAll(tasks);
        }

        private void AddToCollection<T>(T list, string line, Func<string[], string> funcKey, Func<string[], string> funcValue) where T : ICollection<KeyValuePair<string, string>>
        {
            var values = line.Split(DELIMITER);
            if (values.Count() >= 2)
            {
                list.Add(new KeyValuePair<string, string>(funcKey(values), funcValue(values)));
            }
        }

        private int GetLinesCount(string path)
        {
            var lastline = string.Empty;
            using (var fullfiledata = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fullfiledata))
                {
                    long offset = sr.BaseStream.Length - 400;
                    // Skip whole file assumeing that the last line is not more than 400 characters
                    sr.BaseStream.Seek(offset, SeekOrigin.Begin);
                    while (!sr.EndOfStream)
                    {
                        var filedata = sr.ReadLine();
                        if (sr.Peek() == -1)
                        {
                            lastline = filedata;
                        }
                    }
                }
            }

            var r = lastline?.Split(':')[1].Split('\u0002')[0];
            return int.Parse(r);
        }

        private List<BatchOptions> SplitToBatchOptions(int rowCount)
        {
            var count = rowCount / RECORDS_PER_BATCH;
            var reminder = rowCount % RECORDS_PER_BATCH;
            var results = new List<BatchOptions>(count + reminder > 0 ? 1 : 0);
            var skip = 0;
            while (skip < count * RECORDS_PER_BATCH)
            {
                results.Add(new BatchOptions { Skip = skip, TakeCount = RECORDS_PER_BATCH });
                skip += RECORDS_PER_BATCH;
            }
            if (reminder > 0)
            {
                results.Add(new BatchOptions
                {
                    Skip = skip,
                    TakeCount = reminder
                });
            }
            return results;
        }
    }
}
