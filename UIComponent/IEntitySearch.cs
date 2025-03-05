
using Caspian.Common;
using System.Collections;

namespace Caspian.UI
{
    internal interface IEntitySearch
    {
        void EnableLoadData();

        Type EntityType { get; }

        void SetSearchKind(string path, SearchType searchType);

        Task<bool> UpsertEnumValues(string path, ICollection values);

        ICollection GetFieldValues(string propertyPath);
    }
}

