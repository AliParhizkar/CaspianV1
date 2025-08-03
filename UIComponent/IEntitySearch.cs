using Caspian.Common;
using System.Collections;

namespace Caspian.UI
{
    internal interface IEntitySearch
    {
        void EnableLoadData();

        Type EntityType { get; }

        void SetSearchKind(string path, SearchType searchType);

        bool ChangEnumValues(string path, ICollection values);

        Task SearchAsync();

        bool IsLookup { get; }

        Task SelectNextRow();

        Task SelectPreRow();

        Task SelectRow();

        ICollection GetFieldValues(string propertyPath);
    }
}

