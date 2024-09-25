using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public class BatchService<TMaster, TDetail, TDetail1> : BatchService<TMaster, TDetail>, IDetailBatchService<TDetail1>
        where TMaster : class where TDetail : class where TDetail1 : class
    {
        public BatchService(IServiceProvider provider):
            base(provider)
        {
            var detailsproperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail1));
            batchServiceData.DetailPropertiesInfo.Add(detailsproperty);
            ChangedEntities = new List<ChangedEntity<TDetail1>>();
        }

        public DataView<TDetail1> DetailDataView { get; set; }
        public IList<ChangedEntity<TDetail1>> ChangedEntities { get; set; }

        public override void DetailDataViewInitialize()
        {
            if (DetailDataView ==  null)
                base.DetailDataViewInitialize();
            else
            {
                DetailDataView.Batch = true;
                var param = Expression.Parameter(typeof(TDetail), "t");
                var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
                Expression expr = Expression.Property(param, masterInfo);
                var masterId = Convert.ChangeType(MasterId, masterInfo.PropertyType);
                expr = Expression.Equal(expr, Expression.Constant(masterId));
                DetailDataView.InternalConditionExpr = expr;
            }
        }

        protected override async Task UpdateDatabaseAsync(EditContext context1)
        {
            var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(context1.Model));
            using var service = CreateScope().GetService<IMasterDetailsService<TMaster, TDetail, TDetail1>>();
            var result = await service.UpdateDatabaseAsync(UpsertData, base.ChangedEntities, ChangedEntities);
            await service.SaveChangesAsync();
            ChangedEntities.Clear();
            if (id == 0)
            {
                DetailDataView?.ClearSource();
                base.DetailDataView?.ClearSource();
                UpsertData = Activator.CreateInstance<TMaster>();
                Form.SetModel(UpsertData);
                if (OnCreate != null)
                    OnCreate.Invoke(UpsertData);
                if (DataView != null && DataView is DataGrid<TMaster>)
                {
                    var newId = (int)typeof(TMaster).GetPrimaryKey().GetValue(result);
                    await (DataView as DataGrid<TMaster>).SelectRowById(newId);
                }
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "Registration was done successfully");
            }
            else
            {
                if (DetailDataView != null)
                    await DetailDataView.ReloadAsync();
                if (base.DetailDataView != null)
                    await base.DetailDataView.ReloadAsync();
                if (DataView != null)
                    await DataView.ReloadAsync();
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "Updating was done successfully");
                DetailDataView?.CancelInternalUpdate();
                base.DetailDataView?.CancelInternalUpdate();
            }
            if (Window != null)
                await Window?.Close();
            StateHasChanged();
        }
    }
}
