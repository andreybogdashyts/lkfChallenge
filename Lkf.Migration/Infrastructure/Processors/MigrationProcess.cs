
using Lkf.Migration.Infrastructure.SearchEngine;
using Lkf.Migration.Infrastructure.Settings;
using Lkf.Migration.Models;
using System.Diagnostics;

namespace Lkf.Migration.Infrastructure.Processors
{
    public class MigrationProcess : IMigrationProcess
    {
        private const char DELIMITER = '\u0001';
        private const int HEADER_RECORD_COUNT = 3;
        private const int RECORDS_PER_BATCH = 10000;

        private readonly ISettings _settings;

        private readonly ISearchEngine _searchEngine;

        public MigrationProcess(ISettings settings, ISearchEngine searchEngine)
        {
            _settings = settings;
            _searchEngine = searchEngine;
        }

        public class Batch
        {
            public int Skip { get; set; }
            public int TakeCount { get; set; }
        }

        public IEnumerable<Batch> SplitToBatches(int rowCount)
        {
            var count = rowCount / RECORDS_PER_BATCH;
            var reminder = rowCount % RECORDS_PER_BATCH;
            var results = new List<Batch>(count + reminder > 0 ? 1 : 0);
            var skip = 0;
            while(skip < count * RECORDS_PER_BATCH)
            {
                results.Add(new Batch { Skip = skip, TakeCount = RECORDS_PER_BATCH});
                skip += RECORDS_PER_BATCH;
            }
            if (reminder > 0)
            {
                results.Add(new Batch
                {
                    Skip = skip,
                    TakeCount = reminder
                });
            }
            return results;
        }

        public async Task<bool> Migrate()
        {
            var exists = await _searchEngine.CollectionExists(Constants.INDEX_NAME);
            if (!exists)
            {
                await _searchEngine.CreateCollection(Constants.INDEX_NAME);
            }

            var artistsDict = new Dictionary<string, string>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/artist").Skip(HEADER_RECORD_COUNT))
            {
                AddToCollection(artistsDict, l, m => m[1], m => m[2]);
            }

            var collectionMatchDict = new Dictionary<string, string>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/collection_match").Skip(HEADER_RECORD_COUNT))
            {
                AddToCollection(collectionMatchDict, l, m => m[1], m => m[2]);
            }

            var rows = new List<KeyValuePair<string, string>>();
            foreach (var l in File.ReadLines($"{_settings.FilesPath}/artist_collection").Skip(HEADER_RECORD_COUNT))
            {
                AddToCollection(rows, l, m => m[2], m => m[1]);
            }
            var artistCollectionDict = rows.GroupBy(m => m.Key).ToDictionary(m => m.Key, m => m.Select(s => s.Value));

            String lastline = "";
            String filedata;

            // Open file to read
            var fullfiledata = new FileStream($"{_settings.FilesPath}/collection", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fullfiledata);

            long offset = sr.BaseStream.Length - 400;
            sr.BaseStream.Seek(offset, SeekOrigin.Begin);

            //From there read lines, not whole file
            while (!sr.EndOfStream)
            {
                filedata = sr.ReadLine();
                // Interate to see last line
                if (sr.Peek() == -1)
                {
                    lastline = filedata;
                }
            }

            var r = lastline.Split(':')[1].Split('\u0002')[0];
            var t = int.Parse(r);

            var batches = SplitToBatches(t);

            var timer = new Stopwatch();
            timer.Start();
            Parallel.ForEach(batches, async (b) =>
            {
                var collections = new List<Collection>();
                foreach (var l in File.ReadLines($"{_settings.FilesPath}/collection").Skip(b.Skip).Take(b.TakeCount))
                {

                    var values = l.Split(DELIMITER);
                    //TODO: come up with better validation
                    if (values.Count() <= 16)
                    {
                        Console.WriteLine(l);
                        continue;
                    }
                    var collectionId = values[1];
                    collectionMatchDict.TryGetValue(collectionId, out var upc);
                    artistCollectionDict.TryGetValue(collectionId, out var artists);

                    var arts = artists?.Select(s =>
                    {
                        artistsDict.TryGetValue(s, out var name);
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
                await _searchEngine.SendBatch(Constants.INDEX_NAME, collections);
            });

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);
            //var count = File.Read($"{_settings.FilesPath}/collection").Length;

            return true;
        }

        private void AddToCollection<T>(T list, string line, Func<string[], string> funcKey, Func<string[], string> funcValue) where T : ICollection<KeyValuePair<string, string>>
        {
            var values = line.Split(DELIMITER);
            if (values.Count() >= 2)
            {
                //TODO: find indeces by name instead of hardcoded numbers
                list.Add(new KeyValuePair<string, string>(funcKey(values), funcValue(values)));
            }
        }
    }
}
