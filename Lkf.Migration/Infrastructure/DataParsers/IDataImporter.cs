using Lkf.Migration.Models;

namespace Lkf.Migration.Infrastructure.DataParsers
{
    public interface IDataImporter
    {
        IEnumerable<Collection> Import();
    }
}
