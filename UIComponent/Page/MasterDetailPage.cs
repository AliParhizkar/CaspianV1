using System;
using System.Linq;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public partial class MasterDetailPage<TMaster, TDetail> : BasePage where TMaster: class where TDetail: class
    {
        [Parameter]
        public int MasterId { get; set; }

        protected CaspianForm<TMaster> MasterForm { get; set; }

        protected DataGrid<TDetail> Grid { get; set; }

        protected TMaster UpsertData { get; set; } = Activator.CreateInstance<TMaster>();

        [Inject]
        public BatchService BatchService { get; set; }

        protected override void OnInitialized()
        {
            BatchService.MasterId = MasterId;
            BatchService.IgnorePropertyInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (MasterId > 0)
            {
                using var scope = CreateScope();
                var masterService = scope.ServiceProvider.GetService(typeof(ISimpleService<TMaster>)) as SimpleService<TMaster>;
                UpsertData = await masterService.SingleAsync(MasterId);
            }
            await OnMasterEntityCreatedAsync();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (MasterForm != null)
            {
                MasterForm.OnInternalSubmit = EventCallback.Factory.Create<EditContext>(this, (EditContext context1) =>
                {
                    foreach (var info in typeof(TMaster).GetProperties())
                    {
                        if (info.PropertyType.IsCollectionType() && info.PropertyType.GetGenericArguments()[0] == typeof(TDetail))
                            info.SetValue(UpsertData, Grid.AllRecords().AsEnumerable());
                    }
                });
                MasterForm.OnInternalReset = EventCallback.Factory.Create(this, () =>
                {
                    Grid.ClearSource();
                });
                MasterForm.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context1) =>
                {
                    if (MasterId == 0)
                        await InsertMaster(context1);
                    else
                        await UpdateMaster(context1);

                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected virtual async Task OnMasterEntityCreatedAsync()
        {

        }

        async Task UpdateMaster(EditContext context)
        {
            using var scope = CreateScope();
            var provider = scope.ServiceProvider;
            var masterService = provider.GetService(typeof(ISimpleService<TMaster>)) as SimpleService<TMaster>;
            await masterService.UpdateAsync(UpsertData);
            var detailService = provider.GetService(typeof(ISimpleService<TDetail>)) as SimpleService<TDetail>;
            var insertedEntities = Grid.GetInsertedEntities();
            if (insertedEntities.Count > 0)
                await detailService.AddRangeAsync(insertedEntities);
            var updatedEntities = Grid.GetUpdatedEntities();
            if (updatedEntities.Count > 0)
                detailService.UpdateRange(updatedEntities);
            var deletedEntities = Grid.GetDeletedEntities();
            if (deletedEntities.Count > 0)
                detailService.RemoveRange(deletedEntities);
            masterService.SaveChanges();
            await Grid.Reload();
            ShowMessage("بروزرسانی با موفقیت انجام شد");
        }

        async Task InsertMaster(EditContext context1)
        {
            var list = Grid.GetInsertedEntities();
            using var scope = CreateScope();
            var provider = scope.ServiceProvider;
            var masterService = provider.GetService(typeof(ISimpleService<TMaster>)) as SimpleService<TMaster>;
            using var transaction = masterService.Context.Database.BeginTransaction();
            await masterService.AddAsync(UpsertData);
            masterService.SaveChanges();
            var masterId = typeof(TMaster).GetPrimaryKey().GetValue(UpsertData);
            var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            foreach (var item in list)
                masterInfo.SetValue(item, masterId);
            var detailService = provider.GetService(typeof(ISimpleService<TDetail>)) as SimpleService<TDetail>;
            await detailService.AddRangeAsync(list);
            await detailService.SaveChangesAsync();
            transaction.Commit();
            Grid.ClearSource();
            UpsertData = Activator.CreateInstance<TMaster>();
            await OnMasterEntityCreatedAsync();
            ShowMessage("ثبت با موفقیت انجام شد");
        }
    }
}
