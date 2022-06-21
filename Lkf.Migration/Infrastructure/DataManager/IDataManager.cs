
using Lkf.Migration.Models;

namespace Lkf.Migration.Infrastructure.DataManager
{
    public interface IDataManager
    {
        List<BatchOptions> BatchOptions { get; }
        Task PrepareDictionariesAsync();
        List<Collection> GetCollections(BatchOptions options);
    }
}
