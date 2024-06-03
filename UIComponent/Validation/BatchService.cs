using Caspian.Common;
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

        public BatchService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            jSRuntime = serviceProvider.GetService<IJSRuntime>();
            ChangedEntities = new List<ChangedEntity<TDetails>>();
            UpsertData = Activator.CreateInstance<TMaster>();
        }

        IServiceScope CreateScope()
        {
            return serviceProvider.CreateScope();
        }

        public int MasterId { get; set; }

        public TMaster UpsertData {  get; private set; }

        public DataView<TMaster> MasterDataView { get; set; }

        public Window Window { get; set; }

        public DataView<TDetails> DetailsDataView { get; set; }

        public CaspianForm<TMaster> Form { get; set; }

        public IList<ChangedEntity<TDetails>> ChangedEntities { get; set; }

        public void FormInitialize()
        {
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                DetailsDataView?.ClearSource();
                await Window?.Close();
            });

            Form.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context1) =>
            {
                var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(context1.Model));
                using var service = CreateScope().GetService<IMasterDetailsService<TMaster, TDetails>>();
                await service.UpdateDatabaseAsync(UpsertData, ChangedEntities);
                await service.SaveChangesAsync();
                ChangedEntities.Clear();
                if (id == 0)
                {
                    if (DetailsDataView != null)
                        DetailsDataView.ClearSource();
                    UpsertData = Activator.CreateInstance<TMaster>();
                    //await OnMasterEntityCreatedAsync();
                    await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", "Registration was done successfully");
                }
                else
                {
                    if (DetailsDataView != null)
                        await DetailsDataView.ReloadAsync();
                    await jSRuntime.InvokeVoidAsync("$.caspian.showMessage", "Updating was done successfully");
                }
                await Window?.Close();
            });
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
            });
            MasterDataView.OnInternalDelete = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetails>>();
                var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                var old = await service.SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    if (MasterDataView.DeleteMessage.HasValue() /*|| await Confirm(MasterDataView.DeleteMessage)*/)
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
                //BatchServiceData.MasterId = MasterId;
            });
        }
    }

    public interface IMasterBatchService<TMaster> where TMaster : class
    {
        DataView<TMaster> MasterDataView { get; set; }

        CaspianForm<TMaster> Form { get; set; }

        void FormInitialize();

        void MasterGridInitialize();
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