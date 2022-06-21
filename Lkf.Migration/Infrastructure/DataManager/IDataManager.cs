
using Lkf.Migration.Models;

namespace Lkf.Migration.Infrastructure.DataManager
{
    public interface IDataManager
    {
        List<BatchOptions> BatchOptions { get; set; }
        Task PrepareDictionariesAsync();
        List<Collection> GetCollections(BatchOptions options);
    }
}
