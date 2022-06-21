namespace Lkf.Migration.Infrastructure.Readers
{
    public class FileDataReader : IDataReader
    {
        public IEnumerable<string> ReadLines(string path, int skip, int take = 0)
        {
            return take == 0 ? File.ReadLines(path).Skip(skip) : File.ReadLines(path).Skip(skip).Take(take);
        }
    }
}
