
using Caspian.Common;

namespace Caspian.UI
{
    internal interface IEntitySearch
    {
        void EnableLoadData();

        Type EntityType { get; }

        void SetSearchKind(string path, SearchType searchType);
    }
}

