using System.Data;
using Caspian.Common;
using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public class MembershipService<TMaster, TAccess, TOther> :UIService<TAccess>, IInternalSearchService<TOther> where TAccess : class where TOther : class
    {
        bool onlyForSearch;
        protected PropertyInfo masterProperty;

        public MembershipService(IServiceProvider provider)
            :base(provider)
        {
            var properties = typeof(TAccess).GetProperties().Where(t => t.PropertyType == typeof(TOther));
            if (properties.Count() == 0)
                throw new CaspianException($"In type {typeof(TAccess).Name} There is no property of type {typeof(TOther).Name}");
            if (properties.Count() > 1)
                throw new CaspianException($"In type {typeof(TAccess).Name} There is more than 1 property of type {typeof(TOther).Name}. please specify master property");
            var property = properties.Single();
            property.SetValue(base.Search, Activator.CreateInstance<TOther>());
            Search = Activator.CreateInstance<TOther>();
            HideInsertIcon = true;
        }

        protected override void DataViewInitializer()
        {
            if (MasterId > 0)
            {
                var param = Expression.Parameter(typeof(TAccess), "t");
                Expression expr = null;
                PropertyInfo otherProperty = null;
                if (masterProperty == null)
                    otherProperty = typeof(TAccess).GetForeignKey(typeof(TMaster));
                else
                {
                    var name = masterProperty.GetCustomAttribute<ForeignKeyAttribute>().Name;
                    otherProperty = typeof(TAccess).GetProperty(name);
                }
                expr = Expression.Property(param, otherProperty);
                var foreignKeyType = otherProperty.PropertyType;
                if (foreignKeyType.IsNullableType())
                {
                    expr = Expression.Property(expr, "Value");
                    foreignKeyType = foreignKeyType.GetUnderlyingType();
                }
                expr = Expression.Equal(expr, Expression.Constant(Convert.ChangeType(MasterId, foreignKeyType)));
                base.DataView.InternalConditionExpr = expr;
            }
            base.DataViewInitializer();
        }

        public MembershipService(IServiceProvider provider, Expression<Func<TAccess, TMaster>> masterPropertyExpression)
            :base(provider)
        {
            masterProperty = (masterPropertyExpression.Body as MemberExpression).Member as PropertyInfo;
            masterProperty.SetValue(base.Search, Activator.CreateInstance<TOther>());
            Search = Activator.CreateInstance<TOther>();
            HideInsertIcon = true;
        }

        /// <summary>
        /// In Membership-Service We don't want the Upsert-Message to be displayed.
        /// </summary>
        protected override string GetUpsertMessage(UpsertMode upsertMode)
        {
            return null;
        }

        public IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TOther, TValue>> lambda) where TValue : Enum
        {
            return new EnumSearch<TValue>(lambda.Body, (this as IInternalSearchService<TOther>).EnumFields);
        }

        IDictionary<string, ICollection> IInternalSearchService<TOther>.EnumFields { get; set; }

        void IInternalSearchService<TOther>.HideFooter() 
        {
            hideFooter = true;
        }

        void IInternalSearchService<TOther>.LookupInitializer(ILookup<TOther> lookup)
        {
            throw new NotImplementedException();
        }

        bool IInternalSearchService<TOther>.IsLookup()
        {
            return false;
        }

        IList<ValueTypeContainer> IInternalSearchService<TOther>.ValueTypes { get; set; }

        Task IInternalSearchService<TOther>.SelectItemOnLookup()
        {
            throw new NotImplementedException();
        }

        DataView<TOther> ISearchService<TOther>.DataView { get { return OtherDataView; } }

        public DataView<TOther> OtherDataView { get; private set; }

        public TOther Search { get; set; }

        void IInternalSearchService<TOther>.OnlyForSearch()
        {
            onlyForSearch = true;
        }

        public IQueryable<TOther> GetFilteredOthers(IServiceScope scope) => OtherDataView.GetQuery(scope);

        public IQueryable<TAccess> GetFilteredAccess(IServiceScope scope) => base.DataView.GetQuery(scope);

        void IInternalSearchService<TOther>.DataViewInitializer(DataView<TOther> dataView)
        {
            OtherDataView = dataView;
            if (dataView == null)
                return;
            ///We don't want the Deletion-Message to be displayed.
            OtherDataView.DeleteMessage = "";
            OtherDataView.Search = Search;
            OtherDataView.ShowInsertIcon = false;
            OtherDataView.InsertIconState(!onlyForSearch);
            if (typeof(TOther) == typeof(User))
                return;
            PropertyInfo masterIdInfo = null;
            if (masterProperty == null)
                masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
            else
            {
                var name = masterProperty.GetCustomAttribute<ForeignKeyAttribute>().Name;
                masterIdInfo = typeof(TAccess).GetProperty(name);
            }
            var u = Expression.Parameter(typeof(TAccess), "u");
            Expression innerExpr = Expression.Property(u, masterIdInfo);
            if (innerExpr.Type.IsNullableType())
                innerExpr = Expression.Property(innerExpr, "Value");
            innerExpr = Expression.Equal(innerExpr, Expression.Constant(Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType())));
            innerExpr = Expression.Lambda(innerExpr, u);
            var accessListProperties = typeof(TOther).GetProperties().Where(t => typeof(IEnumerable<TAccess>).IsAssignableFrom(t.PropertyType));
            if (masterProperty != null)
                accessListProperties = accessListProperties.Where(t => t.GetCustomAttribute<InversePropertyAttribute>().Property == masterProperty.Name);
            if (accessListProperties.Count() == 0)
                throw new CaspianException("Error: Type " + typeof(TOther).Name + " Must has a Property of Type IEnumerable<" + typeof(TAccess).Name + ">");
            Expression expression = Expression.Property(Expression.Parameter(typeof(TOther), "t"), accessListProperties.Single());
            var method = typeof(Enumerable).GetMethods().Where(t => t.Name == "Any").LastOrDefault().MakeGenericMethod(typeof(TAccess));
            expression = Expression.Call(method, expression, innerExpr);
            expression = Expression.Not(expression);
            OtherDataView.InternalConditionExpr = expression;
        }

        #region Methods overrided from base class
        /// <summary>
        /// This method Execute before form submit to validate. It set MasterId and OtherId (For example GroupId and CustomerId in Customers-Groups Membership)
        /// </summary>
        //protected override async Task InitializeBeforeValidate(TAccess entity)
        //{
        //    await base.InitializeBeforeValidate(entity);
        //    var other = DataView.GetSelectedData();
        //    if (other != null)
        //    {
        //        var otherKey = typeof(TAccess).GetForeignKey(typeof(TOther));
        //        var masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
        //        typeof(TAccess).GetPrimaryKey().SetValue(entity, 0);
        //        var id = typeof(TOther).GetPrimaryKey().GetValue(other);
        //        otherKey.SetValue(entity, id);
        //        masterIdInfo.SetValue(entity, Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType()));
        //    }
        //}

        protected override void InitializeBeforeValidation(TAccess entity)
        {
            var other = OtherDataView.GetSelectedData();
            if (other != null)
            {
                PropertyInfo masterIdInfo, otherKey;
                if (masterProperty == null)
                {
                    otherKey = typeof(TAccess).GetForeignKey(typeof(TOther));
                    masterIdInfo = typeof(TAccess).GetForeignKey(typeof(TMaster));
                }
                else
                {
                    var name = masterProperty.GetCustomAttribute<ForeignKeyAttribute>().Name;
                    masterIdInfo = typeof(TAccess).GetProperty(name);
                    name = typeof(TAccess).GetProperties().Single(t => t.PropertyType == typeof(TOther) && t != masterProperty)
                        .GetCustomAttribute<ForeignKeyAttribute>().Name;
                    otherKey = typeof(TAccess).GetProperty(name);
                }
                typeof(TAccess).GetPrimaryKey().SetValue(entity, 0);
                var id = typeof(TOther).GetPrimaryKey().GetValue(other);
                otherKey.SetValue(entity, id);
                masterIdInfo.SetValue(entity, Convert.ChangeType(MasterId, masterIdInfo.PropertyType.GetUnderlyingType()));
            }
            base.InitializeBeforeValidation(entity);
        }

        protected override async Task UpsertAndInitializeAfterValidate(TAccess entity)
        {
            await base.UpsertAndInitializeAfterValidate(entity);
            await OtherDataView.ReloadAsync();
        }

        protected override async Task InitializeAfterRemove(TAccess entity)
        {
            await base.InitializeAfterRemove(entity);
            PropertyInfo otherIdInfo;
            if(masterProperty == null)
                otherIdInfo = typeof(TAccess).GetForeignKey(typeof(TOther));
            else
            {
                var name = typeof(TAccess).GetProperties().Single(t => t.PropertyType == typeof(TOther) && t != masterProperty)
                        .GetCustomAttribute<ForeignKeyAttribute>().Name;
                otherIdInfo = typeof(TAccess).GetProperty(name);
            }
            
            var otherKey = otherIdInfo.GetValue(entity);
            await OtherDataView.SelectRowById(Convert.ToInt32(otherKey));
            StateHasChanged();
        }

        #endregion

        public async Task AddAsync()
        {
            var item = OtherDataView.GetSelectedData();
            if (item != null)
            {
                var id = Convert.ToInt32(typeof(TOther).GetPrimaryKey().GetValue(item));
                using var service = CreateScope().GetService<IBaseService<TAccess>>();
                
            }
            else
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "لطفا یک ردیف را انتخاب نمائید.");
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
                    service.Remove(old);
                    await service.SaveChangesAsync();
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
                    await base.DataView.ReloadAsync();
                    PropertyInfo otherIdInfo;
                    if (masterProperty == null)
                        otherIdInfo = typeof(TAccess).GetForeignKey(typeof(TOther));
                    else
                    {
                        var name = typeof(TAccess).GetProperties().Single(t => t.PropertyType == typeof(TOther) && t != masterProperty)
                            .GetCustomAttribute<ForeignKeyAttribute>().Name;
                        otherIdInfo = typeof(TAccess).GetProperty(name);
                    }
                    var otherKey = otherIdInfo.GetValue(old);
                    await OtherDataView.SelectRowById(Convert.ToInt32(otherKey));
                    StateHasChanged();
                }
            }
            else
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "لطفا یک ردیف را انتخاب نمائید.");
        }
        IDictionary<string, SearchType> IInternalSearchService<TOther>.SearchData { get; set; }
    }
}
