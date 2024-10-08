using Caspian.Common;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class CheckedListbox<TEntity, TDetails> : ComponentBase where TEntity: class 
    {
        bool LoadData;
        IList<SelectListItem> Items;
        IList<TDetails> details;
        IList<object> SelectedIds;
        string filterText;

        async Task UpdateSelectedIds(bool flag, object value)
        {
            if (flag)
                SelectedIds.Add(value);
            else
                SelectedIds.Remove(value);
            if (Service == null)
            {
                if (Values == null)
                    Values = new List<TDetails>();
                Values.Clear();
                foreach (var selectedId in SelectedIds)
                    Values.Add((TDetails)selectedId);
                if (ValuesChanged.HasDelegate)
                    await ValuesChanged.InvokeAsync(Values);
            }
            else
                UpdateChangedEntities();
        }

        internal string SelectedItemsText()
        {
            string str = string.Empty;
            foreach(var id in SelectedIds)
            {
                if (str != string.Empty)
                    str += ", ";
                str += Items.Single(t => t.Value == id.ToString()).Text;
            }

            return str;
        }

        void UpdateChangedEntities()
        {
            var otherInfo = typeof(TDetails).GetForeignKey(typeof(TEntity));
            var masterType = Service.GetType().GenericTypeArguments[0];
            var masterInfo = typeof(TDetails).GetForeignKey(masterType);
            var list = new List<ChangedEntity<TDetails>>();
            /// Added to list
            foreach (var id in SelectedIds)
            {
                var value = Convert.ChangeType(id, otherInfo.PropertyType);
                if (!details.Any(t => otherInfo.GetValue(t).Equals(value)))
                {
                    var entity = Activator.CreateInstance<TDetails>();
                    otherInfo.SetValue(entity, value);
                    masterInfo.SetValue(entity, Convert.ChangeType(Service.MasterId, masterInfo.PropertyType));
                    list.Add(new ChangedEntity<TDetails>()
                    {
                        Entity = entity,
                        ChangeStatus = ChangeStatus.Added
                    });
                }
            }
            /// Removed from list
            var pkeyInfo = typeof(TDetails).GetPrimaryKey();
            foreach (var detail in details)
            {
                var otherId = otherInfo.GetValue(detail);
                if (!SelectedIds.Contains(otherId))
                {
                    var entity = Activator.CreateInstance<TDetails>();
                    otherInfo.SetValue(entity, otherId);
                    masterInfo.SetValue(entity, Convert.ChangeType(Service.MasterId, masterInfo.PropertyType));
                    pkeyInfo.SetValue(entity, pkeyInfo.GetValue(detail));
                    list.Add(new ChangedEntity<TDetails>()
                    {
                        Entity = entity,
                        ChangeStatus = ChangeStatus.Deleted
                    });
                }
            }
            Service.ChangedEntities = list;
        }

        async Task DataBinding()
        {
            if (LoadData)
            {
                using var service = ScopeFactory.CreateScope().GetService<IBaseService<TEntity>>();
                var query = service.GetAll(default(TEntity));
                if (ConditionExpression != null)
                    query = query.Where(ConditionExpression);
                if (OrderByExpression != null)
                    query = query.OrderBy(OrderByExpression);
                var list = new ExpressionSurvey().Survey(TextExpression);
                var type = typeof(TEntity);
                var parameter = Expression.Parameter(type, "t");
                list = list.Select(t => parameter.ReplaceParameter(t)).ToList();
                var pkey = type.GetPrimaryKey();
                var pKeyExpr = Expression.Property(parameter, pkey);
                var pkeyAdded = false;
                foreach (var expr1 in list)
                {
                    if (expr1.Member == pkey)
                        pkeyAdded = true;
                }
                if (!pkeyAdded)
                    list.Add(pKeyExpr);
                var dataList = await query.GetValuesAsync(list);
                var displayFunc = TextExpression.Compile();
                var info = typeof(TEntity).GetPrimaryKey();
                Items = new List<SelectListItem>();
                foreach (var item in dataList)
                {
                    var text = Convert.ToString(displayFunc.DynamicInvoke(item));
                    var value = Convert.ToString(info.GetValue(item));
                    Items.Add(new SelectListItem(value, text));
                }
            }
        }

        protected override void OnInitialized()
        {
            LoadData = true;
            SelectedIds = new List<object>();
            if (Values != null && Service == null)
            {
                foreach (var value in Values)
                    SelectedIds.Add(value);
            }
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (Service != null)
            {
                var detailIdInfo = typeof(TDetails).GetForeignKey(typeof(TEntity));
                if (Service?.MasterId > 0)
                {
                    var masterType = Service.GetType().GetGenericArguments()[0];
                    var masterIdInfo = typeof(TDetails).GetForeignKey(masterType);
                    var param = Expression.Parameter(typeof(TDetails), "t");
                    Expression expr = Expression.Property(param, masterIdInfo);
                    expr = Expression.Equal(expr, Expression.Constant(Service.MasterId));
                    var lambda = Expression.Lambda(expr, param);
                    using var service = Provider.CreateScope().GetService<IBaseService<TDetails>>();
                    var query = service.GetAll().Where(lambda);
                    var exprDetailId = Expression.Property(param, detailIdInfo);
                    var exprPKey = Expression.Property(param, typeof(TDetails).GetPrimaryKey());
                    details = await query.GetValuesAsync(exprDetailId, exprPKey);
                    foreach (var detail in details)
                    {
                        var otherId = detailIdInfo.GetValue(detail);
                        SelectedIds.Add(otherId);
                    }
                }
                else
                    details = new List<TDetails>();
                if (Service.ChangedEntities != null)
                {
                    foreach (var detail in Service.ChangedEntities) 
                    {
                        var otherId = detailIdInfo.GetValue(detail.Entity);
                        if (detail.ChangeStatus == ChangeStatus.Deleted)
                            SelectedIds.Remove(otherId);
                        else
                            SelectedIds.Add(otherId);
                    }
                }
            }
            await base.OnInitializedAsync();
        }

        protected async override Task OnParametersSetAsync()
        {
            await DataBinding();
            await base.OnParametersSetAsync();
        }

        [Parameter]
        public EventCallback<TDetails> AddToChanged { get; set; }

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public ISimpleBatchService<TDetails> Service { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> OrderByExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public bool ShowSelectAll { get; set; }

        [Parameter]
        public string SelectAllText { get; set; } = "انتخاب همه";

        [Parameter]
        public string PlaceHolder { get; set; } = "جستجو";

        [Parameter]
        public IList<TDetails> Values { get; set; }

        [Parameter]
        public EventCallback<IList<TDetails>> ValuesChanged { get; set; }

        async Task SelectAll(bool? selected)
        {
            SelectedIds = new List<object>();
            
            if (Service == null)
            {
                if (Values == null)
                    Values = new List<TDetails>();
                Values.Clear();
                if (selected != false)
                {
                    var type = typeof(TDetails);
                    foreach (var item in Items)
                    {
                        var value = Convert.ChangeType(item.Value, type);
                        SelectedIds.Add(value);
                        Values.Add((TDetails)value);
                    }
                }
                if (ValuesChanged.HasDelegate)
                    await ValuesChanged.InvokeAsync(Values);
            }
            else
            {
                if (selected != false)
                {
                    var type = typeof(TDetails).GetForeignKey(typeof(TEntity)).PropertyType;
                    foreach (var item in Items)
                    {
                        var value = Convert.ChangeType(item.Value, type);
                        SelectedIds.Add(value);

                    }
                }
                UpdateChangedEntities();
            }
        }
    }
}
