using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public class SimpleService<TEntity>: ISimpleCrudService, IMasterBatchService<TEntity> where TEntity : class
    {
        IServiceProvider serviceProvider;
        BaseComponentService baseComponentService;
        IJSRuntime jSRuntime;


        public SimpleService(IServiceProvider serviceProvider) 
        {
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            jSRuntime = serviceProvider.GetService<IJSRuntime>();
            this.serviceProvider = serviceProvider;
            Search = Activator.CreateInstance<TEntity>();
            UpsertData = Activator.CreateInstance<TEntity>();
        }

        public Window Window { get; set; }

        public DataView<TEntity> MasterDataView { get; set; }

        public CaspianForm<TEntity> Form { get; set; }

        public TEntity UpsertData { get; private set; }

        public TEntity Search { get; private set; }

        public async Task FetchAsync()
        {
            
        }

        public Func<TEntity, Task<bool>> OnUpsert { get; set; }

        public void FormInitialize()
        {
            Form.Model = UpsertData;
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                if (Window != null)
                    await Window?.Close();
                StateHasChanged();
            });
            Form.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async context =>
            {
                var result = true;
                if (OnUpsert != null)
                    result = await OnUpsert.Invoke(UpsertData);
                if (!result)
                    return;
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(context.Model));
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                string message = null;
                if (id == 0)
                {
                    await service.AddAsync(UpsertData);
                    message = "Registration was done successfully";
                }
                else
                {
                    await service.UpdateAsync(UpsertData);
                    message = "Updating was done successfully";
                }
                await service.SaveChangesAsync();
                if (id == 0)
                {
                    id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(UpsertData));
                    await (MasterDataView as DataGrid<TEntity>).SelectRowById(id);
                }
                else
                    await MasterDataView.ReloadAsync();
                await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", message);
                if (Window == null)
                    await Form.ResetAsync();
                else
                    await Window.Close();
            });
        }

        public void StateHasChanged()
        {
            if (baseComponentService.Target == null)
                throw new CaspianException("You must inherits from BasePage or configure page manioaly");
            typeof(ComponentBase).GetMethod("StateHasChanged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(baseComponentService.Target, null);
        }

        IServiceScope CreateScope()
        {
            return serviceProvider.CreateScope();
        }

        public void MasterGridInitialize()
        {
            (MasterDataView as DataGrid<TEntity>).Search = Search;
            MasterDataView.OnInternalUpsert = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                var value = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                if (value != 0)
                {
                    using var service = CreateScope().GetService<BaseService<TEntity>>();
                    UpsertData = await service.GetAll().SingleAsync(value);
                }
                else
                    UpsertData = Activator.CreateInstance<TEntity>();
                if (Form != null)
                    Form.Model = UpsertData;
                if (Window != null)
                    await Window.Open();
                StateHasChanged();
            });

            MasterDataView.OnInternalDelete = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                var old = await service.SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    if (!MasterDataView.DeleteMessage.HasValue() || await Confirm(MasterDataView.DeleteMessage))
                    {
                        await service.RemoveAsync(old);
                        await service.SaveChangesAsync();
                        await MasterDataView.ReloadAsync();
                    }
                }
                else
                    await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        async Task<bool> Confirm(string message)
        {
            return await baseComponentService.MessageBox.Confirm(message);
        }

        public void WindowInitialize()
        {
            Window.OnInternalClose = EventCallback.Factory.Create(this, () => StateHasChanged());
        }
    }
}
