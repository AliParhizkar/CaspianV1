
using Caspian.Common;

namespace Caspian.UI
{
    internal interface IEntitySearch
    {
        void EnableLoadData();

        Type EntityType { get; }

        void SetSearchKind(string path, SearchType searchType);

        Task UpsertEnumValues(string path, object[] values);

        object[] GetFieldValues(string propertyPath);
    }
}

