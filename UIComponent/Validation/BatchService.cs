using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public class BatchService<TMaster, TDetails>: IMasterBatchService<TMaster>, ISimpleCrudService, IDetailsBatchService<TDetails> where TMaster : class where TDetails : class
    {
        IJSRuntime jSRuntime;
        IServiceProvider serviceProvider;
        BatchServiceData batchServiceData;
        BaseComponentService baseComponentService;

        public BatchService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            jSRuntime = serviceProvider.GetService<IJSRuntime>();
            ChangedEntities = new List<ChangedEntity<TDetails>>();
            UpsertData = Activator.CreateInstance<TMaster>();
            batchServiceData = serviceProvider.GetService<BatchServiceData>();
            batchServiceData.MasterType = typeof(TMaster);
            if (batchServiceData.DetailPropertiesInfo == null)
                batchServiceData.DetailPropertiesInfo = new List<PropertyInfo>();
            var detailsproperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetails));
            batchServiceData.DetailPropertiesInfo.Add(detailsproperty);
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
        }

        IServiceScope CreateScope()
        {
            return serviceProvider.CreateScope();
        }

        public int MasterId { get; set; }

        public TMaster UpsertData { get; private set; }

        public DataView<TMaster> MasterDataView { get; set; }

        public Window Window { get; set; }

        public DataView<TDetails> DetailsDataView { get; set; }

        public CaspianForm<TMaster> Form { get; set; }

        public IList<ChangedEntity<TDetails>> ChangedEntities { get; set; }

        public async Task FetchAsync()
        {
            if (MasterId > 0)
            {
                batchServiceData.MasterId = MasterId;
                using var service = CreateScope().GetService<IBaseService<TMaster>>();
                UpsertData = await service.SingleAsync(MasterId);
            }
        }

        public void FormInitialize()
        {
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                DetailsDataView?.ClearSource();
                if (Window != null)
                    await Window?.Close();
                StateHasChanged();
            });

            Form.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context1) =>
            {
                var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(context1.Model));
                using var service = CreateScope().GetService<IMasterDetailsService<TMaster, TDetails>>();
                var result = await service.UpdateDatabaseAsync(UpsertData, ChangedEntities);
                await service.SaveChangesAsync();
                ChangedEntities.Clear();
                if (id == 0)
                {
                    if (DetailsDataView != null)
                        DetailsDataView.ClearSource();
                    UpsertData = Activator.CreateInstance<TMaster>();
                    //await OnMasterEntityCreatedAsync();
                    if (MasterDataView != null && MasterDataView is DataGrid<TMaster>)
                    {
                        var newId = (int)typeof(TMaster).GetPrimaryKey().GetValue(result);
                        await (MasterDataView as DataGrid<TMaster>).SelectRowById(newId);
                    }
                    await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", "Registration was done successfully");
                }
                else
                {
                    if (DetailsDataView != null)
                        await DetailsDataView.ReloadAsync();
                    if (MasterDataView != null)
                        await MasterDataView.ReloadAsync();
                    await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", "Updating was done successfully");
                }
                if (Window != null)
                    await Window?.Close();
                StateHasChanged();
            });
        }

        public void StateHasChanged()
        {
            if (baseComponentService.Target == null)
                throw new CaspianException("You must inherits from BasePage or configure page manioaly");
            typeof(ComponentBase).GetMethod("StateHasChanged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(baseComponentService.Target, null);
        }

        public void MasterGridInitialize()
        {
            MasterDataView.OnInternalUpsert = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                var value = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                if (value != 0)
                {
                    var detailsName = typeof(TMaster).GetDetailsProperty(typeof(TDetails)).Name;
                    using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetails>>();
                    UpsertData = await service.GetAll().Include(detailsName).SingleAsync(value);
                }
                else
                    UpsertData = Activator.CreateInstance<TMaster>();
                await Window.Open();
                StateHasChanged();
            });
            MasterDataView.OnInternalDelete = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetails>>();
                var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                var old = await service.SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    if (!MasterDataView.DeleteMessage.HasValue() || await Confirm(MasterDataView.DeleteMessage))
                    {
                        await service.DeleteMasterAndDetails(old);
                        await service.SaveChangesAsync();
                        await MasterDataView.ReloadAsync();
                    }
                }
                else
                    await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        public void WindowInitialize()
        {
            Window.OnInternalOpen = EventCallback.Factory.Create(this, () =>
            {
                MasterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(UpsertData));
                batchServiceData.MasterId = MasterId;
            });
        }

        async Task<bool> Confirm(string message)
        {
            return await baseComponentService.MessageBox.Confirm(message);
        }
    }

    public interface IMasterBatchService<TMaster> where TMaster : class
    {
        DataView<TMaster> MasterDataView { get; set; }

        CaspianForm<TMaster> Form { get; set; }

        void FormInitialize();

        void MasterGridInitialize();

        Task FetchAsync();

        TMaster UpsertData { get; }
    }

    public interface IDetailsBatchService<TDetails> where TDetails : class
    {
        DataView<TDetails> DetailsDataView { get; set; }

        IList<ChangedEntity<TDetails>> ChangedEntities { get; set; }
    }

    public interface ISimpleCrudService
    {
        Window Window { get; set; }

        void WindowInitialize();
    }
}