namespace Lkf.Migration.Infrastructure.SearchEngine
{
    public interface ISearchEngine
    {
        bool CollectionExists(string name);
        void CreateCollection(string name);
    }
}
