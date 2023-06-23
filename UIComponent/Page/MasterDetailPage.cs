using System;
using System.Linq;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Caspian.UI
{
    public partial class MasterDetailPage<TMaster, TDetail> : BasePage where TMaster: class where TDetail: class
    {
        [Parameter]
        public int MasterId { get; set; }

        protected CaspianForm<TMaster> Form { get; set; }

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
                var masterService = scope.ServiceProvider.GetService(typeof(IBaseService<TMaster>)) as BaseService<TMaster>;
                UpsertData = await masterService.SingleAsync(MasterId);
            }
            await OnMasterEntityCreatedAsync();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Form != null)
            {
                Form.OnInternalSubmit = EventCallback.Factory.Create<EditContext>(this, (EditContext context1) =>
                {
                    foreach (var info in typeof(TMaster).GetProperties())
                    {
                        var type = info.PropertyType;
                        if (type.IsCollectionType() && type.IsGenericType && type.GetGenericArguments()[0] == typeof(TDetail))
                            info.SetValue(UpsertData, Grid.GetUpsertedEntities().AsEnumerable());
                    }
                });
                Form.OnInternalReset = EventCallback.Factory.Create(this, () =>
                {
                    Grid.ClearSource();
                });
                Form.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context1) =>
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
            var service = provider.GetService(typeof(IMasterDetailsService<TMaster, TDetail>)) as MasterDetailsService<TMaster, TDetail>;
            var key = typeof(TDetail).GetPrimaryKey();
            await service.UpdateAsync(UpsertData, Grid.GetDeletedEntities().Select(t => Convert.ToInt32(key.GetValue(t))));
            var detailsInfo = typeof(TMaster).GetProperties().Single(t => t.PropertyType.GetInterfaces().Contains(typeof(IEnumerable<TDetail>)));
            //var detailService = provider.GetService(typeof(IBaseService<TDetail>)) as BaseService<TDetail>;
            //var insertedEntities = Grid.GetInsertedEntities();
            //if (insertedEntities.Count > 0)
            //    await detailService.AddRangeAsync(insertedEntities);
            //var updatedEntities = Grid.GetUpdatedEntities();
            //if (updatedEntities.Count > 0)
            //    detailService.UpdateRange(updatedEntities);
            //var deletedEntities = Grid.GetDeletedEntities();
            //if (deletedEntities.Count > 0)
            //    detailService.RemoveRange(deletedEntities);
            service.SaveChanges();
            await Grid.ReloadAsync();
            ShowMessage("بروزرسانی با موفقیت انجام شد");
        }

        protected virtual async Task OnUpsertAsync(IServiceScope scope, IList<TDetail> insertedEntities2)
        {

        }

        async Task InsertMaster(EditContext context1)
        {
            var list = Grid.GetInsertedEntities();
            using var scope = CreateScope();
            var provider = scope.ServiceProvider;
            var service = provider.GetService(typeof(IMasterDetailsService<TMaster, TDetail>)) as MasterDetailsService<TMaster, TDetail>;
            
            await service.AddAsync(UpsertData);
            service.SaveChanges();
            await OnUpsertAsync(scope, list);
            Grid.ClearSource();
            UpsertData = Activator.CreateInstance<TMaster>();
            await OnMasterEntityCreatedAsync();
            ShowMessage("Registration was done successfully");
        }
    }
}
