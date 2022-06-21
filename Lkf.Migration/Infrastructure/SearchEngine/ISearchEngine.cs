using Lkf.Migration.Models;

namespace Lkf.Migration.Infrastructure.SearchEngine
{
    public interface ISearchEngine
    {
        Task<bool> CollectionExists(string name);
        Task<bool> CreateCollection(string name);
        Task<bool> SendBatch(string name, List<Collection> collections);
    }
}
