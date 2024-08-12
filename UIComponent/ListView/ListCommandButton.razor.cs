using System.Data;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class ListCommandButton<TEntity> : DataCommand<TEntity> where TEntity : class
    {
        string errorMessage;
        MessageBox MessageBox;
        bool disabled;

        [Parameter]
        public CommandButtonType? ButtonType { get; set; }

        [CascadingParameter]
        public CaspianContainer Container { get; set; }

        protected override async Task OpenForm()
        {
            if (DataView != null)
            {
                DataView.Inline = true;
                await base.OpenForm();
            }
        }

        protected override void OnParametersSet()
        {
            disabled = Container?.Disabled == true;
            base.OnParametersSet();
        }

        async Task RemoveAsync()
        {
            using var scope = Factory.CreateScope();
            var service = scope.GetService<IBaseService<TEntity>>();
            var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(RowData.Data));
            var old = await service.SingleAsync(id);
            var result = await service.ValidateRemoveAsync(old);
            if (result.IsValid)
            {
                if (await MessageBox.Confirm(DataView.DeleteMessage))
                {
                    await service.RemoveAsync(old);
                    try
                    {
                        await service.SaveChangesAsync();
                        await DataView.ReloadAsync();
                    }
                    catch (DbUpdateConcurrencyException exp)
                    {
                        errorMessage = "این آیتم قبلا حذف شده است";
                    }
                }
            }
            else
                errorMessage = result.Errors.First().ErrorMessage;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (errorMessage != null)
            {
                await jsRuntime.InvokeVoidAsync("$.caspian.showMessage", errorMessage);
                errorMessage = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
