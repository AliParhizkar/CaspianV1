﻿using System.Data;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public class BatchService<TMaster, TDetail>: ISimpleService, ISimpleService<TMaster>, IDetailBatchService<TDetail> where TMaster : class where TDetail : class
    {
        IServiceProvider serviceProvider;
        BaseComponentService baseComponentService;
        protected IJSRuntime jSRuntime;
        protected BatchServiceData batchServiceData;

        public BatchService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            jSRuntime = serviceProvider.GetService<IJSRuntime>();
            ChangedEntities = new List<ChangedEntity<TDetail>>();
            UpsertData = Activator.CreateInstance<TMaster>();
            batchServiceData = serviceProvider.GetService<BatchServiceData>();
            batchServiceData.MasterType = typeof(TMaster);
            if (batchServiceData.DetailPropertiesInfo == null)
                batchServiceData.DetailPropertiesInfo = new List<PropertyInfo>();
            var detailsproperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            batchServiceData.DetailPropertiesInfo.Add(detailsproperty);
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            Search = Activator.CreateInstance<TMaster>();
        }

        protected IServiceScope CreateScope()
        {
            return serviceProvider.CreateScope();
        }

        public int MasterId { get; set; }

        public TMaster UpsertData { get; protected set; }

        public TMaster Search {  get; set; }

        public DataView<TMaster> DataView { get; set; }

        public Window Window { get; set; }

        public DataView<TDetail> DetailDataView { get; set; }

        public CaspianForm<TMaster> Form { get; set; }

        public IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        public async Task FetchAsync()
        {
            if (MasterId > 0)
            {
                batchServiceData.MasterId = MasterId;
                using var service = CreateScope().GetService<IBaseService<TMaster>>();
                UpsertData = await service.SingleAsync(MasterId);
                Form.SetModel(UpsertData);
            }
        }

        public Action<TMaster> OnCreate { get; set; }

        protected virtual async Task UpdateDatabaseAsync(EditContext context1)
        {
            var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(context1.Model));
            using var service = CreateScope().GetService<IMasterDetailsService<TMaster, TDetail>>();
            var result = await service.UpdateDatabaseAsync(UpsertData, ChangedEntities);
            await service.SaveChangesAsync();
            ChangedEntities.Clear();
            if (id == 0)
            {
                DetailDataView?.ClearSource();
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
                if (DataView != null)
                    await DataView.ReloadAsync();
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "Updating was done successfully");
            }
            if (DetailDataView != null)
                DetailDataView.CancelInternalUpdate();
            if (Window != null)
                await Window?.Close();
            StateHasChanged();
        }

        public void FormInitialize()
        {
            if (UpsertData == null)
                UpsertData = Activator.CreateInstance<TMaster>();
            if (OnCreate != null)
                OnCreate.Invoke(UpsertData);   
            Form.SetModel(UpsertData);
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                DetailDataView?.ClearSource();
                if (Window != null)
                {
                    await Window?.Close();
                    StateHasChanged();
                }
            });

            Form.OnInternalValidSubmit = EventCallback.Factory.Create<EditContext>(this, UpdateDatabaseAsync);
        }

        public void StateHasChanged()
        {
            if (baseComponentService.Target == null)
                throw new CaspianException("You must inherits from BasePage or configure page manioaly");
            typeof(ComponentBase).GetMethod("StateHasChanged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(baseComponentService.Target, null);
        }

        public void DataViewInitialize()
        {
            DataView.Search = Search;
            DataView.OnInternalUpsert = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                if (Window != null)
                {
                    var value = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                    if (value != 0)
                    {
                        var detailsName = typeof(TMaster).GetDetailsProperty(typeof(TDetail)).Name;
                        using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
                        UpsertData = await service.GetAll().Include(detailsName).SingleAsync(value);
                    }
                    else
                        UpsertData = Activator.CreateInstance<TMaster>();
                    MasterId = value;
                    await Window.Open();
                    StateHasChanged();
                }
            });
            DataView.OnInternalDelete = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
                var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                var old = await service.SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    if (!DataView.DeleteMessage.HasValue() || await Confirm(DataView.DeleteMessage))
                    {
                        await service.DeleteMasterAndDetails(old);
                        await service.SaveChangesAsync();
                        await DataView.ReloadAsync();
                    }
                }
                else
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        public virtual void DetailDataViewInitialize()
        {
            DetailDataView.Batch = true;
            var param = Expression.Parameter(typeof(TDetail), "t");
            var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            Expression expr = Expression.Property(param, masterInfo);
            var masterId = Convert.ChangeType(MasterId, masterInfo.PropertyType);
            expr = Expression.Equal(expr, Expression.Constant(masterId));
            DetailDataView.InternalConditionExpr = expr;
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

        public void ClearForm()
        {
            //Form = null;
            //UpsertData = null;
            //ChangedEntities = null;
        }
    }

    public interface ISimpleService<TEntity>: ISearchService<TEntity> where TEntity : class
    {
        CaspianForm<TEntity> Form { get; set; }

        void FormInitialize();

        Task FetchAsync();

        TEntity UpsertData { get; }

        void ClearForm();
    }

    public interface ISearchService<TEntity> where TEntity:class
    {
        DataView<TEntity> DataView { get; set; }

        void DataViewInitialize();
    }

    public interface ISimpleBatchService<TDetail>
    {
        int MasterId { get; }

        IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        void DetailDataViewInitialize();
    }

    public interface IDetailBatchService<TDetail>: ISimpleBatchService<TDetail> where TDetail : class
    {
        DataView<TDetail> DetailDataView { get; set; }
    }

    public interface ISimpleService
    {
        Window Window { get; set; }

        void WindowInitialize();

        void ClearWindow()
        {
            Window = null;
        }
    }
}