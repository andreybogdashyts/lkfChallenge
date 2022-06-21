namespace Lkf.Migration.Infrastructure.Readers
{
    public interface IDataReader
    {
        IEnumerable<string> ReadLines(string path, int skip, int take = 0);
    }
}
