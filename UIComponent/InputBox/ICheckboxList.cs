using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface ICheckboxList<TValue>
    {
        TValue Value { get; set; }
     
        EventCallback<TValue> ValueChanged { get; set; }

        IList<TValue> Values { get; set; }

        EventCallback<IList<TValue>> ValuesChanged { get; set; }

        Task UpdateValue(TValue value);
    }

    public interface IControl
    {
        Task FocusAsync();

        void Focus();

        Task ResetAsync();
    }
}
