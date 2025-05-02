using Caspian.Common;
using Caspian.Common.Extension;
using Caspian.Common.Service;
using Microsoft.JSInterop;
using System.Collections;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public class MembershipService<TMaster, TAccess, TOther>:UIService<TAccess>, IInternalSearchService<TOther> where TAccess : class where TOther : class
    {
        protected IDictionary<string, SearchType> searchData;
        protected IDictionary<string, ICollection> enumValues;
        bool onlyForSearch;
        public MembershipService(IServiceProvider provider)
            :base(provider)
        {
            var properties = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TOther));
            if (properties.Count() == 0)
                throw new CaspianException($"In type {typeof(TAccess).Name} There is no property of type {typeof(TOther).Name}");
            if (properties.Count() > 1)
                throw new CaspianException($"In type {typeof(TAccess).Name} There is more than 1 property of type {typeof(TOther).Name}");
            var property = properties.Single();
            property.SetValue(base.Search, Activator.CreateInstance<TOther>());
            Search = Activator.CreateInstance<TOther>();
            MasterType = typeof(TMaster);
            base.HideInsertIcon = true;
        }

        public IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TOther, TValue>> expression) where TValue : Enum
        {
            return new EnumSearch<TValue>(expression.Body, enumValues);
        }

        public void SetEnumFields(IDictionary<string, ICollection> enumFields)
        {
            this.enumValues = enumFields;
        }

        public IDictionary<string, ICollection> GetEnumFields()
        {
            return enumValues;
        }

        public DataView<TOther> DataView { get; set; }

        public TOther Search { get; set; }

        public void OnlyForSearch()
        {
            onlyForSearch = true;
        }

        void IInternalSearchService<TOther>.DataViewInitialize(DataView<TOther> dataView)
        {
            DataView.Search = Search;
            DataView.ShowInsertIcon = false;
            DataView.InsertIconState(!onlyForSearch);
            var masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
            var u = Expression.Parameter(typeof(TAccess), "u");
            Expression innerExpr = Expression.Property(u, masterIdInfo);
            if (innerExpr.Type.IsNullableType())
                innerExpr = Expression.Property(innerExpr, "Value");
            innerExpr = Expression.Equal(innerExpr, Expression.Constant(Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType())));
            innerExpr = Expression.Lambda(innerExpr, u);
            var accessListInf = typeof(TOther).GetProperties().Where(t => typeof(IEnumerable<TAccess>).IsAssignableFrom(t.PropertyType));
            if (accessListInf.Count() != 1)
                throw new CaspianException("Error: Type " + typeof(TOther).Name + " Must has a Property of Type IEnumerable<" + typeof(TAccess).Name + ">");
            Expression expression = Expression.Property(Expression.Parameter(typeof(TOther), "t"), accessListInf.Single());
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Any").LastOrDefault().MakeGenericMethod(typeof(TAccess));
            expression = Expression.Call(method, expression, innerExpr);
            expression = Expression.Not(expression);
            DataView.InternalConditionExpr = expression;
            
            OnFormSubmit = entity =>
            {
                var other = DataView.GetSelectedData();
                if (other != null)
                {
                    var otherKey = typeof(TAccess).GetForeignKey(typeof(TOther));
                    typeof(TAccess).GetPrimaryKey().SetValue(entity, 0);
                    var id = typeof(TOther).GetPrimaryKey().GetValue(other);
                    otherKey.SetValue(entity, id);
                    masterIdInfo.SetValue(entity, Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType()));
                }
            };
            OnFormValidSubmit = async entity => { await DataView.ReloadAsync(); };
            OnAfterDelete = async entity => 
            {
                var otherIdInfo = typeof(TAccess).GetForeignKey(typeof(TOther));
                var otherKey = otherIdInfo.GetValue(entity);
                await DataView.SelectRowById(Convert.ToInt32(otherKey));
                StateHasChanged();
            };
        }

        public async Task RemoveAsync()
        {
            var item = base.DataView.GetSelectedData();
            if (item != null)
            {
                var id = Convert.ToInt32(typeof(TAccess).GetPrimaryKey().GetValue(item));
                using var service = CreateScope().GetService<IBaseService<TAccess>>();
                var old = await service.SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    await service.RemoveAsync(old);
                    await service.SaveChangesAsync();
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
                    await base.DataView.ReloadAsync();
                    var otherIdInfo = typeof(TAccess).GetForeignKey(typeof(TOther));
                    var otherKey = otherIdInfo.GetValue(old);
                    await DataView.SelectRowById(Convert.ToInt32(otherKey));
                    StateHasChanged();
                }
            }
            else
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "لطفا یک ردیف را انتخاب نمائید.");
        }

        public void SetSearchType(IDictionary<string, SearchType> types)
        {
            searchData = new Dictionary<string, SearchType>();
        }

        public IDictionary<string, SearchType> GetSearchData()
        {
            return searchData;
        }
    }
}
