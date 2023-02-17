using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface IGridRowSelect
    {
        void SelectFirstRow();

        Task SelectNextRow();

        Task SelectPrevRow();

        Task SelectFirstPage();

        int? SelectedRowId { get; }

        EventCallback<int> OnInternalRowSelect { get; set; }

        Task ResetGrid();
    }

    public interface IRadioList
    {
        Task ChangeValueAsync(object value);

        object GetValue();
    }
}
