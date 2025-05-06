using System.Data;
using Caspian.Common;
using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Caspian.UI
{
    public class BatchService<TMaster, TDetail>: IInternalUIService, IInternalUIService<TMaster>, IInternalSearchService<TMaster>, IInternalBatchService<TDetail> where TMaster : class where TDetail : class
    {
        IServiceProvider serviceProvider;
        BaseComponentService baseComponentService;
        protected IJSRuntime jSRuntime;
        protected BatchServiceData batchServiceData;
        protected IDictionary<string, SearchType> searchData;
        protected IDictionary<string, ICollection> enumValues;
        bool onlyForSearch, hideFooter;
        CaspianDataService CaspianDataService;

        public async Task CloseWindow()
        {
            
        }

        Expression IInternalBatchService<TDetail>.GetDetailsFilterExpression()
        {
            var param = Expression.Parameter(typeof(TDetail), "t");
            var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            Expression expr = Expression.Property(param, masterInfo);
            var masterId = Convert.ChangeType(MasterId, masterInfo.PropertyType);
            return Expression.Equal(expr, Expression.Constant(masterId));
        }

        void IInternalBatchService<TDetail>.SetDetails(IList<TDetail> details)
        {
            var detailInfo = batchServiceData.DetailPropertiesInfo.First(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            detailInfo.SetValue(UpsertData, details);
        }

        async Task IInternalUIService<TMaster>.UpdateChildOfModelAsync(Type type)
        {
            await Task.Delay(1);
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GetSearchEnumValues<TValue>(Expression<Func<TDetail, TValue>> expression) where TValue : Enum
        {
            throw new NotImplementedException();
        }

        public IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TMaster, TValue>> expression) where TValue : Enum
        {
            return new EnumSearch<TValue>(expression.Body, enumValues);
        }

        public IServiceProvider Provider { get { return serviceProvider; } }

        void IInternalSearchService<TMaster>.OnlyForSearch()
        {
            onlyForSearch = true;
        }

        void IInternalSearchService<TMaster>.HideFooter()
        {
            hideFooter = true;
        }

        IDictionary<string, SearchType> IInternalSearchService<TMaster>.GetSearchData()
        {
            return searchData;
        }

        IDictionary<string, ICollection> IInternalSearchService<TMaster>.GetEnumFields()
        {
            return enumValues;
        }

        void IInternalSearchService<TMaster>.SetEnumFields(IDictionary<string, ICollection> enumFields)
        {
            this.enumValues = enumFields;
        }

        public Func<IServiceProvider, TMaster, Task<bool>> OnUpsert { get; set; }

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
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            batchServiceData.DetailPropertiesInfo.Add(detailsProperty);
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            CaspianDataService = serviceProvider.GetService<CaspianDataService>();
            Search = Activator.CreateInstance<TMaster>();
        }

        void IInternalSearchService<TMaster>.SetSearchType(IDictionary<string, SearchType> types)
        {
            searchData = types;
        }

        PropertyInfo IInternalBatchService<TDetail>.ThirdLevelProperty { get; set; }

        public void ThirdDataLevelToIgnoreOnRemove<TProperty>(Expression<Func<TDetail, ICollection<TProperty>>> expression)
        {
            (this as IInternalBatchService<TDetail>).ThirdLevelProperty = (expression.Body as MemberExpression).Member as PropertyInfo;
        }

        /// <summary>
        /// This method reload data for update
        /// </summary>
        /// <param name="masterId">The id of "TMaster"</param>
        public async Task ReloadForUpdate(int masterId)
        {
            MasterId = masterId;
            await (this as IInternalUIService<TMaster>).FetchAsync();
            if (DetailDataView != null)
            {
                bool dataIsLoaded = false;
                DetailDataView?.EnableLoading();
                DetailDataView.OnLoaded = () =>
                {
                    dataIsLoaded = true;
                };
                if (!dataIsLoaded)
                    await Task.Delay(100);
            }
            ChangedEntities?.Clear();
            
        }

        protected IServiceScope CreateScope()
        {
            return serviceProvider.CreateScope();
        }

        public IEntityTabPanel EntityTabPanel { get; private set; }

        void IInternalUIService<TMaster>.TabPanelInitialize(IEntityTabPanel tabPanel)
        {
            EntityTabPanel = tabPanel;
        }

        //public void ChildrenTabPanelItemInitialize(Type masterType)
        //{
        //    throw new NotImplementedException();
        //}

         Type IInternalUIService<TMaster>.DetailType { get; set; }

        public int MasterId { get; set; }

        public TMaster UpsertData { get; protected set; }

        public TMaster Search {  get; set; }

        public DataView<TMaster> DataView { get; private set; }

        Window IInternalUIService.Window { get; set; }

        /// <summary>
        /// Open Window to validate and upsert the entity. In this case window has form that bind to entity
        /// </summary>
        /// <param name="id">key of entity</param>
        /// <returns>Task</returns>
        public async Task OpenWindow(int? id)
        {
            if (id == null)
                UpsertData = Activator.CreateInstance<TMaster>();
            else
            {
                using var service = CreateScope().GetService<IBaseService<TMaster>>();
                UpsertData = await service.SingleAsync(id.Value);
            }
            await (this as IInternalUIService).Window.Open();
            (this as IInternalUIService<TMaster>).StateHasChanged();
            await Task.Delay(100);
            if (Form != null)
                await Form.FocusAsync();
        }

        public DataView<TDetail> DetailDataView { get; set; }

        public TypeWindow<TDetail> TypeWindow { get; set; }

        public CaspianForm<TMaster> Form { get; private set; }

        public CaspianForm<TDetail> DetailForm { get; set; }

        void IInternalUIService.Dispose()
        {
            (this as IInternalUIService<TMaster>).DetailType = default;
            /// On Master Details Service We Set MasterId on OnInitialized but it reset here because page disposed 
            /// on another page created

            //MasterId = default;
            (this as IInternalUIService).Window = default;
            EntityTabPanel = default;
            DataView = default;
            DetailDataView = default;
            Form = default;
            Search = Activator.CreateInstance<TMaster>();
            UpsertData = Activator.CreateInstance<TMaster>();
            batchServiceData.DetailPropertiesInfo.Clear();
        }

        void IInternalBatchService<TDetail>.DetailFormInitialize(CaspianForm<TDetail> caspianForm)
        {
            DetailForm = caspianForm;
            if (DetailForm != null)
            {
                DetailForm.OnInternalReset = EventCallback.Factory.Create(this, TypeWindow.Close);
                DetailForm.OnInternalValidSubmit = EventCallback.Factory.Create<TDetail>(this, async detail => 
                {
                    TypeWindow.Close();
                    var id = Convert.ToInt32(typeof(TDetail).GetPrimaryKey().GetValue(detail));
                    if (id == 0)
                        await DetailDataView.InsertAsync(detail);   
                    else
                        await DetailDataView.UpdateAsync(detail);
                    (this as IInternalUIService<TMaster>).StateHasChanged();
                });
            }
;        }

        public IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        async Task IInternalUIService<TMaster>.FetchAsync()
        {
            batchServiceData.MasterId = MasterId;
            if (MasterId > 0)
            {
                using var service = CreateScope().GetService<IBaseService<TMaster>>();
                UpsertData = await service.SingleAsync(MasterId);
                Form?.SetModel(UpsertData);
            }
        }

        public Action<TMaster> OnCreate { get; set; }

        public Func<IServiceProvider, TMaster, Task> OnAfterOpsertAsync { get; set; }

        protected virtual async Task UpdateDatabaseAsync(TMaster master)
        {
            var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
            using var scope = CreateScope();
            if (OnUpsert != null)
            {
                if (!await OnUpsert.Invoke(scope.ServiceProvider, master))
                    return;
            }
            var service = scope.GetService<IMasterDetailsService<TMaster, TDetail>>();
            var result = await service.UpdateDatabaseAsync(UpsertData, ChangedEntities);
            await service.SaveChangesAsync();
            if (OnAfterOpsertAsync != null)
                await OnAfterOpsertAsync(scope.ServiceProvider, result);
            ChangedEntities.Clear();
            if (service.Context?.Database?.CurrentTransaction != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: Transaction Rollbacked by Caspian infrastructure");
                Console.ResetColor();
                service.Context.Database.CurrentTransaction.Rollback();
            }
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
                var message = CaspianDataService.Language == Language.Fa ? "ثبت با موفقیت انجام شد." : "Registration was done successfully";
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
            }
            else
            {
                if (DetailDataView != null)
                    await DetailDataView.ReloadAsync();
                if (DataView != null)
                    await DataView.ReloadAsync();
                var message = CaspianDataService.Language == Language.Fa ? "بروزرسانی با موفقیت انجام شد." : "Updating was done successfully";
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
            }
            if (DetailDataView != null)
                DetailDataView.CancelInternalUpdate();
            if ((this as IInternalUIService).Window != null)
                await (this as IInternalUIService).Window?.Close();
            (this as IInternalUIService<TMaster>).StateHasChanged();
        }

        void IInternalUIService<TMaster>.FormInitialize(CaspianForm<TMaster> form)
        {
            Form = form;
            batchServiceData.MasterType = typeof(TMaster);
            var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            if (!batchServiceData.DetailPropertiesInfo.Contains(detailsProperty))
                batchServiceData.DetailPropertiesInfo.Add(detailsProperty);
            if (UpsertData == null)
                UpsertData = Activator.CreateInstance<TMaster>();
            if (OnCreate != null)
                OnCreate.Invoke(UpsertData);   
            Form.SetModel(UpsertData);
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                DetailDataView?.ClearSource();
                if ((this as IInternalUIService).Window != null)
                {
                    await (this as IInternalUIService).Window?.Close();
                    (this as IInternalUIService<TMaster>).StateHasChanged();
                }
            });

            Form.OnInternalValidSubmit = EventCallback.Factory.Create<TMaster>(this, UpdateDatabaseAsync);
        }

        void IInternalUIService<TMaster>.StateHasChanged()
        {
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            if (baseComponentService.Target == null)
                throw new CaspianException("You must inherits from BasePage or configure page manioaly");
            (baseComponentService.Target as BasePage).ChangeState();
        }

        void IInternalSearchService<TMaster>.DataViewInitialize(DataView<TMaster> dataView)
        {
            DataView = dataView;
            if (dataView == null)
                return;
            DataView.Search = Search;
            DataView.HideFooter = DataView.HideFooter ?? hideFooter;
            DataView.InsertIconState(!onlyForSearch);
            DataView.OnInternalUpsert = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                if ((this as IInternalUIService).Window != null)
                {
                    var value = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                    if (value != 0)
                    {
                        var detailsName = typeof(TMaster).GetDetailsProperty(typeof(TDetail)).Name;
                        using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
                        UpsertData = await service.GetAll().Include(detailsName).SingleAsync(value);
                    }
                    else
                    {
                        UpsertData = Activator.CreateInstance<TMaster>();
                        ChangedEntities.Clear();
                    }
                    MasterId = value;
                    await (this as IInternalUIService).Window.Open();
                    (this as IInternalUIService<TMaster>).StateHasChanged();
                    await Task.Delay(100);
                    if (Form != null)
                        await Form.FocusAsync();
                }
            });
            DataView.OnInternalDelete = EventCallback.Factory.Create<TMaster>(this, async master =>
            {
                using var scope = CreateScope();
                using var service = scope.GetService<MasterDetailsService<TMaster, TDetail>>();
                var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
                var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
                var old = await service.GetAll().Include(detailsProperty.Name).SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    var details = detailsProperty.GetValue(old) as IEnumerable<TDetail>;
                    string errorMessage = null;
                    var detailService = scope.GetService<IBaseService<TDetail>>();
                    foreach (var item in details) 
                    {
                        var result1 = await detailService.ValidateRemoveAsync(item);
                        if (!result1.IsValid)
                        {
                            errorMessage = result1.Errors.First().ErrorMessage;
                            break;
                        }
                    }
                    if (errorMessage == null)
                    {
                        if (!DataView.DeleteMessage.HasValue() || await Confirm(DataView.DeleteMessage))
                        {
                            await service.DeleteMasterAndDetails(old);
                            await service.SaveChangesAsync();
                            await DataView.ReloadAsync();
                            await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
                        }
                    }
                    else
                        await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", errorMessage);
                }
                else
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        void IInternalBatchService<TDetail>.DetailTypeWindowInitialize(TypeWindow<TDetail> window)
        {
            TypeWindow = window;
            DetailDataView.Batch = true;
            DetailDataView.OnInternalUpsert = EventCallback.Factory.Create<TDetail>(this, async detail => 
            {
                if (MasterId > 0)
                {
                    var info = typeof(TDetail).GetForeignKey(typeof(TMaster));
                    var value = Convert.ChangeType(MasterId, info.PropertyType.GetUnderlyingType());
                    info.SetValue(detail, value);
                }
                await TypeWindow.OpenAsync(detail, DetailDataView.GetSource().ToList());
                (this as IInternalUIService<TMaster>).StateHasChanged();
                await Task.Delay(100);
                if (DetailForm != null)
                    await DetailForm.FocusAsync();
            });
        }

        void IInternalBatchService<TDetail>.DetailDataViewInitialize(DataView<TDetail> dataView)
        {
            DetailDataView = dataView;
            if (dataView != null)
            {
                if (DetailDataView.Inline)
                    DetailDataView.Batch = true;
                DetailDataView.InsertIconState(true);
                DetailDataView.InternalConditionExpr = (this as IInternalBatchService<TDetail>).GetDetailsFilterExpression();
            }
        }

        void IInternalUIService.WindowInitialize()
        {
            (this as IInternalUIService).Window.OnInternalOpen = EventCallback.Factory.Create(this, () =>
            {
                MasterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(UpsertData));
                batchServiceData.MasterId = MasterId;
            });
        }

        async Task<bool> Confirm(string message)
        {
            return await baseComponentService.MessageBox.Confirm(message);
        }

        void IInternalUIService<TMaster>.ClearForm()
        {
            //Form = null;
            //UpsertData = null;
            //ChangedEntities = null;
        }
    }
}