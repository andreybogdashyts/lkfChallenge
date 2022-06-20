using Lkf.Migration.Models;

namespace Lkf.Migration.Infrastructure.SearchEngine
{
    public interface ISearchEngine
    {
        Task<bool> CollectionExists(string name);
        Task CreateCollection(string name);
        Task SendBatch(string name, List<Collection> collections);
    }
}
