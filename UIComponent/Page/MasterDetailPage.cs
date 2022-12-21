using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using Caspian.Common.Service;
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (MasterForm != null)
            {
                MasterForm.OnInternalSubmit = EventCallback.Factory.Create<EditContext>(this, (EditContext context) =>
                {
                    foreach (var info in typeof(TMaster).GetProperties())
                    {
                        if (info.PropertyType.IsCollectionType() && info.PropertyType.GetGenericArguments()[0] == typeof(TDetail))
                            info.SetValue(UpsertData, Grid.AllRecords());
                    }
                });
                MasterForm.OnInternalReset = EventCallback.Factory.Create(this, () =>
                {
                    Grid.ClearSource();
                });
                MasterForm.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context) =>
                {
                    await InsertMaster();
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        async Task InsertMaster()
        {
            var detailsInfo = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsCollectionType() && t.PropertyType.GetGenericArguments()[0] == typeof(TDetail));
            var list = detailsInfo.GetValue(UpsertData) as IList<TDetail>;
            using var scope = CreateScope();
            var provider = scope.ServiceProvider;
            var masterServiceType = provider.GetService(typeof(ISimpleService<TMaster>)).GetType();
            var masterService = Activator.CreateInstance(masterServiceType, scope) as SimpleService<TMaster>;
            using var transaction = masterService.Context.Database.BeginTransaction();
            await masterService.AddAsync(UpsertData);
            await masterService.SaveChangesAsync();
            var masterId = typeof(TMaster).GetPrimaryKey().GetValue(UpsertData);
            //masterId = 12;
            var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            foreach (var item in list)
                masterInfo.SetValue(item, masterId);
            var detailServiceType = provider.GetService(typeof(ISimpleService<TDetail>)).GetType();
            var detailService = Activator.CreateInstance(detailServiceType, scope) as SimpleService<TDetail>;
            await detailService.AddRangeAsync(list);
            await detailService.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
