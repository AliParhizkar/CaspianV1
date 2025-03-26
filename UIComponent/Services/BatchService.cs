using System.Data;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Caspian.UI
{
    public class BatchService<TMaster, TDetail>: IUIService, IUIService<TMaster>, IDetailBatchService<TDetail> where TMaster : class where TDetail : class
    {
        IServiceProvider serviceProvider;
        BaseComponentService baseComponentService;
        protected IJSRuntime jSRuntime;
        protected BatchServiceData batchServiceData;
        protected IDictionary<string, SearchType> searchData;
        protected IDictionary<string, ICollection> enumValues;
        bool onlyForSearch, hideFooter;


        public async Task UpdateChildOfModelAsync(Type type)
        {
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

        public void OnlyForSearch()
        {
            onlyForSearch = true;
        }

        public void HideFooter()
        {
            hideFooter = true;
        }

        public IDictionary<string, SearchType> GetSearchData()
        {
            return searchData;
        }

        public IDictionary<string, ICollection> GetEnumFields()
        {
            return enumValues;
        }

        public void SetEnumFields(IDictionary<string, ICollection> enumFields)
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
            var detailsproperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            batchServiceData.DetailPropertiesInfo.Add(detailsproperty);
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            Search = Activator.CreateInstance<TMaster>();
        }

        public void SetSearchType(IDictionary<string, SearchType> types)
        {
            searchData = types;
        }

        protected IServiceScope CreateScope()
        {
            return serviceProvider.CreateScope();
        }

        public IEntityTabPanel EntityTabPanel { get; set; }

        public void TabPanelInitialize()
        {
            throw new NotImplementedException();
        }

        public void ChildrenTabPanelItemInitialize(Type masterType)
        {
            throw new NotImplementedException();
        }

        public Type DetailType { get; private set; }

        public int MasterId { get; set; }

        public TMaster UpsertData { get; protected set; }

        public TMaster Search {  get; set; }

        public DataView<TMaster> DataView { get; set; }

        public Window Window { get; set; }

        public DataView<TDetail> DetailDataView { get; set; }

        public TypeWindow<TDetail> TypeWindow { get; set; }

        public CaspianForm<TMaster> Form { get; set; }

        public CaspianForm<TDetail> DetailForm { get; set; }

        public void Dispose()
        {
            DetailType = default;
            /// On Master Details Service We Set MasterId on OnInitialized but it reset here because page disposed 
            /// on another page created
            
            //MasterId = default;
            Window = default;
            EntityTabPanel = default;
            DataView = default;
            DetailDataView = default;
            Form = default;
            Search = Activator.CreateInstance<TMaster>();
            UpsertData = Activator.CreateInstance<TMaster>();
            batchServiceData.DetailPropertiesInfo.Clear();
        }

        public void DetailFormInitialize()
        {
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
                    StateHasChanged();
                });
            }
;        }

        public IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        public async Task FetchAsync()
        {
            batchServiceData.MasterId = MasterId;
            if (MasterId > 0)
            {
                using var service = CreateScope().GetService<IBaseService<TMaster>>();
                UpsertData = await service.SingleAsync(MasterId);
                Form.SetModel(UpsertData);
            }
        }

        public Action<TMaster> OnCreate { get; set; }

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
            batchServiceData.MasterType = typeof(TMaster);
            var detailsproperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
            if (!batchServiceData.DetailPropertiesInfo.Contains(detailsproperty))
                batchServiceData.DetailPropertiesInfo.Add(detailsproperty);
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

            Form.OnInternalValidSubmit = EventCallback.Factory.Create<TMaster>(this, UpdateDatabaseAsync);
        }

        public void StateHasChanged()
        {
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            if (baseComponentService.Target == null)
                throw new CaspianException("You must inherits from BasePage or configure page manioaly");
            (baseComponentService.Target as BasePage).ChangeState();
        }

        public void DataViewInitialize()
        {
            DataView.Search = Search;
            DataView.HideFooter = DataView.HideFooter ?? hideFooter;
            DataView.InsertIconState(!onlyForSearch);
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
                    {
                        UpsertData = Activator.CreateInstance<TMaster>();
                        ChangedEntities.Clear();
                    }
                    MasterId = value;
                    await Window.Open();
                    StateHasChanged();
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
                        }
                    }
                    else
                        await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", errorMessage);
                }
                else
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        public void DetailTypwWindowInitialize()
        {
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
                StateHasChanged();
                await Task.Delay(100);
                if (DetailForm != null)
                    await DetailForm.FocusAsync();
            });
        }

        public virtual void DetailDataViewInitialize()
        {
            if (DetailDataView.Inline)
                DetailDataView.Batch = true;
            DetailDataView.InsertIconState(true);
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
}