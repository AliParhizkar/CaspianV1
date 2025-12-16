using Caspian.Common;
using System.Collections;

namespace Caspian.UI
{
    internal interface IEntitySearch
    {
        void EnableLoadData();

        Task ReloadData();

        Type EntityType { get; }

        void SetSearchKind(string path, SearchType searchType);

        bool ChangEnumValues(string path, ICollection values);

        void SetFromValue(string propertyPath, object fromValue);

        void SetToValue(string propertyPath, object toValue);

        void SetValue(string propertyPath, bool? value);

        Task SearchAsync();

        bool IsLookup { get; }

        //string GetId(string memberName);

        Task SelectNextRow();

        Task SelectPreRow();

        Task SelectRow();

        ICollection GetFieldValues(string propertyPath);
    }
}

