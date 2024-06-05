using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class MasterDetailPage<TMaster, TDetail> : BasePage where TMaster: class where TDetail: class
    {
        CaspianForm<TMaster> oldForm;
        CaspianForm<TMaster> form;

        [Parameter]
        public int MasterId { get; set; }

        public DataView<TDetail> DataView { get; set; }

        public DataGrid<TMaster> MasterGrid { get; set; }

        public Window Window { get; set; }

        public TMaster UpsertData { get; set; } = Activator.CreateInstance<TMaster>();

        //protected override async Task OnInitializedAsync()
        //{
        //    if (MasterId > 0)
        //    {
        //        using var masterService = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
        //        UpsertData = await masterService.SingleAsync(MasterId);
        //    }
        //    else
        //        UpsertData = Activator.CreateInstance<TMaster>();

        //    await OnMasterEntityCreatedAsync();

        //    await base.OnInitializedAsync();
        //}

        async Task UpsertMaster()
        {
            var pKey = typeof(TMaster).GetPrimaryKey();
            var id = Convert.ToInt32(pKey.GetValue(UpsertData));
            using var masterService = CreateScope().GetService<BaseService<TMaster>>();
            if (id == 0)
            {
                var result = await masterService.AddAsync(UpsertData);
                await masterService.SaveChangesAsync();
                id = Convert.ToInt32(pKey.GetValue(result));
                await Window.Close();
                await MasterGrid.SelectRowById(id);
            }
            else
            {
                var detailsInfo = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
                var old = await masterService.GetAll().Include(detailsInfo.Name).SingleAsync(id);
                old.CopyEntity(UpsertData);
                var oldDetails = detailsInfo.GetValue(old) as IEnumerable<TDetail>;
                var otherforeignKeyName = typeof(TDetail).GetProperties().Single(t => t.PropertyType != typeof(TMaster) && 
                    t.GetCustomAttribute<ForeignKeyAttribute>() != null).GetCustomAttribute<ForeignKeyAttribute>().Name;
                var otherforeignKey = typeof(TDetail).GetProperty(otherforeignKeyName);
                var curentMembersId = new List<int>();
                var curentDetails = detailsInfo.GetValue(UpsertData) as IEnumerable<TDetail>;
                if (curentDetails != null)
                {
                    foreach (var member in curentDetails)
                    {
                        var value = otherforeignKey.GetValue(member);
                        curentMembersId.Add(Convert.ToInt32(value));
                    }
                }
                var oldMembersId = new List<int>();
                var list = new List<TDetail>();
                foreach(var member in oldDetails)
                {
                    var value = Convert.ToInt32(otherforeignKey.GetValue(member));
                    if (curentMembersId.Contains(value))
                        list.Add(member);
                    oldMembersId.Add(value);
                }
                foreach(var member in curentDetails)
                {
                    var memberId = Convert.ToInt32(otherforeignKey.GetValue(member));
                    if (!oldMembersId.Contains(memberId))
                        list.Add(member);
                }
                detailsInfo.SetValue(old, list);
                await masterService.SaveChangesAsync();
                await Window.Close();
                await MasterGrid.ReloadAsync();
            }
        }

        void InitializeForm()
        {
            //Form.OnInternalSubmit = EventCallback.Factory.Create<EditContext>(this, (EditContext context1) =>
            //{
            //    if (DataView != null)
            //    {
            //        //foreach (var info in typeof(TMaster).GetProperties())
            //        //{
            //        //    var type = info.PropertyType;
            //        //    if (type.IsCollectionType() && type.IsGenericType && type.GetGenericArguments()[0] == typeof(TDetail))
            //        //        throw new NotFiniteNumberException();
            //        //        //info.SetValue(UpsertData, DataView.GetUpsertedEntities().AsEnumerable());
            //        //}
            //    }

            //});
            //Form.OnInternalReset = EventCallback.Factory.Create(this, () =>
            //{
            //    DataView?.ClearSource();
            //    Window?.Close();
            //});
            //if (Window != null)
            //{
            //    Window.OnInternalOpen = EventCallback.Factory.Create(this, () =>
            //    {
            //        MasterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(UpsertData));
            //        BatchServiceData.MasterId = MasterId;
            //    });
            //}
            //Form.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, async (EditContext context1) =>
            //{
            //    return;
            //    var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(context1.Model));
            //    using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
            //    await service.UpdateDatabaseAsync(UpsertData, ChangedEntities);
            //    await service.SaveChangesAsync();
            //    ChangedEntities.Clear();
            //    if (id == 0)
            //    {
            //        if (DataView != null)
            //            DataView.ClearSource();
            //        UpsertData = Activator.CreateInstance<TMaster>();
            //        await OnMasterEntityCreatedAsync();
            //ShowMessage("Registration was done successfully");
            //    }
            //    else
            //    {
            //        if (DataView != null)
            //            await DataView.ReloadAsync();
            //        ShowMessage("Updating was done successfully");
            //    }
            //    Window?.Close();
            //});
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (MasterGrid != null)
            //{
            //    MasterGrid.OnInternalUpsert = EventCallback.Factory.Create<TMaster>(this, async master => 
            //    {
            //        var value = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
            //        if (value != 0)
            //        {
            //            var detailsName = typeof(TMaster).GetDetailsProperty(typeof(TDetail)).Name;
            //            using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
            //            UpsertData = await service.GetAll().Include(detailsName).SingleAsync(value);
            //        }
            //        else
            //            UpsertData = Activator.CreateInstance<TMaster>();
            //        await Window.Open();
            //    });
            //    MasterGrid.OnInternalDelete = EventCallback.Factory.Create<TMaster>(this, async master=>
            //    {
            //        using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
            //        var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
            //        var old = await service.SingleAsync(id);
            //        var result = await service.ValidateRemoveAsync(old);
            //        if (result.IsValid)
            //        {
            //            if (!MasterGrid.DeleteMessage.HasValue() || await Confirm(MasterGrid.DeleteMessage))
            //            {
            //                await service.DeleteMasterAndDetails(old);
            //                await service.SaveChangesAsync();
            //            }
            //        }
            //        else
            //            ShowMessage(result.Errors[0].ErrorMessage);
            //    });
            //}
            await base.OnAfterRenderAsync(firstRender);
        }

        protected virtual async Task OnMasterEntityCreatedAsync()
        {

        }

        protected virtual async Task OnUpsertAsync(IServiceScope scope, IList<TDetail> insertedEntities2)
        {

        }

        async Task InsertMaster(EditContext context)
        {
            //var list = DataView.GetInsertedEntities();
            //using var scope = CreateScope();
            //var service = scope.GetService<MasterDetailsService<TMaster, TDetail>>();
            //await service.AddAsync(UpsertData);
            //service.SaveChanges();
            //await OnUpsertAsync(scope, list);
            //DataView.ClearSource();
            //UpsertData = Activator.CreateInstance<TMaster>();
            //await OnMasterEntityCreatedAsync();
            //ShowMessage("Registration was done successfully");
        }
    }
}
