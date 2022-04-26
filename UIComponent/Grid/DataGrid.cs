using System;
using System.Linq;
using Caspian.Common;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Caspian.Common.RowNumber;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity>: ComponentBase, IEnableLoadData, IGridRowSelect where TEntity: class
    {
        string jsonOldSearch;
        bool mustRender = true;
        bool commandColumnAdded;
        ElementReference mainDiv;
        int aggregateColumnIndex;
        bool SholdRendered = true;
        IList<object> DynamicData;
        IList<ColumnData> columnsData;
        IDictionary<string, object> tableAttrs;
        IList<MemberExpression> SelectExpressions;

        public int? SelectedRowIndex { get; private set; }

        [Inject, JsonIgnore]
        IServiceProvider ServiceProvider { get; set; }

        [Inject, JsonIgnore]
        public FormAppState FormAppState { get; set; }

        [Parameter, JsonIgnore]
        public bool HidePageSize { get; set; }

        internal void AddColumnData(GridColumn<TEntity> column)
        {
            columnsData.Add(new ColumnData()
            {
                Title = column.Title,
                Expression = column.Field?.Body,
                AggregateExpression = column.AggregateField?.Body,
                FromExpression = column.FromExpression,
                ToExpression = column.ToExpression,
                Orderable = column.Field?.Body as BinaryExpression == null,
                Width = column.Width,
                OrderType = column.OrderType,
                Resizeable = column.Template == null && !column.IsCheckBox
            });
            StateHasChanged();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await AfterRenderInitialAsync();
            if (FormAppState.Element.HasValue)
            {
                await jsRuntime.InvokeVoidAsync("$.telerik.focusAndShowErrorMessage", FormAppState.Element, FormAppState.ErrorMessage);
                FormAppState.Element = null;
                FormAppState.ErrorMessage = null;
            }
            if (firstRender)
            {
                await DataBind();
                StateHasChanged();
                await jsRuntime.InvokeVoidAsync("$.telerik.dadaGridBind", mainDiv);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal int GetAggregateColumnIndex()
        {
            return aggregateColumnIndex++;
        }

        internal void AddCommandColumn(string title, string width)
        {
            if (!commandColumnAdded)
            {
                columnsData.Add(new ColumnData()
                {
                    Title = title,
                    Width = width,
                    Resizeable = false
                });
                commandColumnAdded = true;
                StateHasChanged();
            }
        }

        protected override bool ShouldRender()
        {
            return mustRender;
        }

        IQueryable<TEntity> GetQuery(IServiceScope scope)
        {
            var service = new SimpleService<TEntity>(scope);
            var query = service.GetAll(Search);
            Expression expr = null;
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            Expression expression = null;
            foreach (var col in columnsData.Where(t => t.FromExpression != null || t.ToExpression != null))
            {
                Expression memberExpr = parameter.ReplaceParameter(col.Expression);
                if (memberExpr.Type.IsNullableType())
                    memberExpr = Expression.Property(memberExpr, "Value");
                if (col.FromExpression != null)
                {
                    var fromValue = GetFromToValue(col.FromExpression);
                    if (fromValue != null)
                    {
                        var covertedValue = Convert.ChangeType(fromValue, memberExpr.Type.GetUnderlyingType());
                        var compareExpr = Expression.GreaterThanOrEqual(memberExpr, Expression.Constant(covertedValue));
                        if (expression == null)
                            expression = compareExpr;
                        else
                            expression = Expression.And(expression, compareExpr);
                    }
                }
                if (col.ToExpression != null)
                {
                    var toValue = GetFromToValue(col.ToExpression);
                    if (toValue != null)
                    {
                        var covertedValue = Convert.ChangeType(toValue, memberExpr.Type.GetUnderlyingType());
                        var compareExpr = Expression.LessThanOrEqual(memberExpr, Expression.Constant(covertedValue));
                        if (expression == null)
                            expression = compareExpr;
                        else
                            expression = Expression.And(expression, compareExpr);
                    }
                }
            }
            if (expression != null)
            {
                if (expr == null)
                    expr = expression;
                else
                    expr = Expression.And(expr, expression);
            }
            expr = UpdateExpressionForMasterForm(parameter, expr);
            if (expr != null)
            {
                var lambda = Expression.Lambda(expr, parameter);
                query = query.Where(lambda).OfType<TEntity>();
            }
            if (ConditionExpr != null)
                query = query.Where(ConditionExpr);
            if (InternalConditionExpr != null)
            {
                if (expr == null)
                    expr = parameter.ReplaceParameter(InternalConditionExpr);
                else
                    expr = Expression.And(expr, parameter.ReplaceParameter(ConditionExpr));
            }
            if (expr != null)
            {
                var lambda = Expression.Lambda(expr, parameter);
                query = query.Where(lambda).OfType<TEntity>();
            }
            if (OnDataBinding != null)
                query = OnDataBinding.Invoke(query);
            return query;
        }

        IQueryable<TEntity> GetOrderByQuery(IQueryable<TEntity> query)
        {
            var index = 0;
            foreach (var col in columnsData)
            {
                if (col.OrderType != null && col.Orderable)
                {
                    Expression orderbyExpr = col.Expression;
                    if (orderbyExpr.NodeType == ExpressionType.Call)
                    {
                        var callExpr = orderbyExpr as MethodCallExpression;
                        if (callExpr.Arguments.Count > 0)
                            orderbyExpr = callExpr.Arguments[0];
                        else
                            orderbyExpr = callExpr.Object;
                    }
                    if (orderbyExpr.NodeType == ExpressionType.Convert)
                        orderbyExpr = (orderbyExpr as UnaryExpression).Operand;
                    var param = Expression.Parameter(typeof(TEntity), "t");
                    orderbyExpr = param.ReplaceParameter(orderbyExpr as MemberExpression);
                    var lambdaExpression = Expression.Lambda(orderbyExpr, param);
                    if (col.OrderType == OrderType.Asc)
                        query = query.OrderBy(lambdaExpression).OfType<TEntity>();
                    else if (col.OrderType == OrderType.Decs)
                        query = query.OrderByDescending(lambdaExpression).OfType<TEntity>();
                }
                index++;
            }
            return query;
        }

        public async Task DataBind()
        {
            if (columnsData.Count > 0 && SholdRendered)
            {
                SholdRendered = false;
                using var scope = ServiceScopeFactory.CreateScope();
                var query = GetQuery(scope);
                var exprList = new List<MemberExpression>();
                if (columnsData.Any(t => t.AggregateExpression != null))
                {
                    var aggregateExprList = columnsData.Where(t => t.AggregateExpression != null).Select(t => t.AggregateExpression).ToList();
                    Total = await query.CreateAggregateQuery(aggregateExprList).OfType<object>().CountAsync();
                    var tuple = await query.AggregateValuesAsync(aggregateExprList);
                    Items = tuple.Item1;
                    DynamicData = tuple.Item2;
                }
                else
                {
                    Total = await query.CountAsync();
                    query = GetOrderByQuery(query);
                    if (PageNumber > 1)
                    {
                        var skip = (PageNumber - 1) * PageSize;
                        query = query.Skip(skip);
                    }
                    foreach (var item in columnsData.Where(t => t.Expression != null))
                    {
                        var tempList = new ExpressionSurvey().Survey(item.Expression);
                        foreach (var expr2 in tempList)
                            if (!exprList.Any(t => t.ToString() == expr2.ToString()))
                                exprList.Add(expr2);
                    }
                    var parameterExpr = Expression.Parameter(typeof(TEntity), "t");
                    var pKey = typeof(TEntity).GetPrimaryKey();
                    var expr1 = Expression.Property(parameterExpr, pKey);
                    exprList.Add(expr1);
                    if (Inline)
                    {
                        foreach (var info in typeof(TEntity).GetProperties())
                        {
                            var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
                            if (attr != null)
                            {
                                var expr2 = Expression.Property(parameterExpr, attr.Name);
                                exprList.Add(expr2);
                            }
                        }
                        SelectExpressions = exprList;
                    }
                    Items = await query.Take(PageSize).GetValuesAsync<TEntity>(exprList);
                }

            }
        }

        IList<Expression> ConvertExpressionForGroupBy(IList<Expression> list, ParameterExpression parameter)
        {
            var exprList = new List<Expression>();
            foreach(var item in list)
            {
                var path = item.ToString();
                var index = path.IndexOf('.');
                path = "Key." + path.Substring(index + 1);
                exprList.Add(parameter.CreateMemberExpresion(path));
            }
            return exprList;
        }

        internal void SetDeleteMessage(string message)
        {
            DeleteMessage = message;
        }

        public int? SelectedRowId
        {
            get
            {
                if (SelectedRowIndex.HasValue && SelectedRowIndex >= 0 && SelectedRowIndex < Items.Count)
                {
                    var value = typeof(TEntity).GetPrimaryKey().GetValue(Items[SelectedRowIndex.Value]);
                    return Convert.ToInt32(value);
                }
                return null;
            }
        }

        internal Expression InternalConditionExpr { get; set; }

        [Inject, JsonIgnore]
        protected IJSRuntime jsRuntime { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpr { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

        [JsonIgnore]
        public Window Control { get; set; }

        [Parameter, JsonIgnore]
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter, JsonIgnore]
        public int TableHeight { get; set; } = 400;

        [Parameter, JsonIgnore]
        public int? TableWidth { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback<TEntity> OnUpsert { get; set; }

        [JsonIgnore]
        public EventCallback<TEntity> OnInternalUpsert { get; set; }

        [Parameter, JsonIgnore]
        public Func<TEntity, Task<bool>> OnDelete { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback OnPageChanged { get; set; }

        [JsonIgnore]
        internal EventCallback<TEntity> OnInternalDelete { get; set; }

        [Parameter, JsonIgnore]
        public Func<TEntity, Task> OnRowSelect { get; set; }

        [JsonIgnore]
        public EventCallback<int> OnInternalRowSelect { get; set; }

        [JsonIgnore]
        public IList<TEntity> Items { get; set; }

        [JsonIgnore]
        public int Total { get; set; }

        [Parameter, JsonIgnore]
        public int PageNumber { get; set; } = 1;

        [JsonIgnore]
        public int PageSize { get; set; } = 5;

        [Parameter, JsonIgnore]
        public TEntity Search { get; set; }

        [Parameter, JsonIgnore]
        public SelectType SelectType { get; set; } = SelectType.Single;

        [Parameter, JsonIgnore]
        public RenderFragment<RowData<TEntity>> Columns { get; set; }

        [Parameter, JsonIgnore]
        public string DeleteMessage { get; set; }

        [Parameter, JsonIgnore]
        public RenderFragment ToolsBar { get; set; }

        [JsonIgnore, Parameter]
        public bool Inline { get; set; }

        [JsonIgnore, Parameter]
        public RenderFragment SearchTemplate { get; set; }

        [JsonIgnore, Parameter]
        public bool HideInsertIcon { get; set; }

        async Task UpdateOrder()
        {
            SholdRendered = true;
            await OnParametersSetAsync();
            StateHasChanged();
        }

        public TEntity GetSelectedData()
        {
            if (SelectedRowIndex == null || Items == null || Items.Count < SelectedRowIndex.Value || SelectedRowIndex == -1)
                return null;
            return Items.ElementAt(SelectedRowIndex.Value);
        }

        public void EnableLoading()
        {
            SholdRendered = true;
        }

        public async Task Reload()
        {
            EnableLoading();

            await OnParametersSetAsync();
            var pageCount = (Total - 1) / PageSize + 1;
            if (pageCount < PageNumber)
            {
                await ChangePageNumber(pageCount);
                SelectedRowIndex = Items.Count - 1;
            }
            else if (SelectedRowIndex.Value >= Items.Count)
                SelectedRowIndex = Items.Count - 1;
        }

        public async Task ResetGrid()
        {
            SelectedRowIndex = 0;
            SholdRendered = true;
            await ChangePageNumber(1);
        }

        public void SelectRow(int rowIndex)
        {
            SelectedRowIndex = rowIndex;
        }

        public async Task SelectFirstPage()
        {
            if (this.PageNumber != 1)
                await ChangePageNumber(1);
        }

        async Task ChangePageNumber(int pageNumber)
        {
            PageNumber = pageNumber;
            SholdRendered = true;
            await DataBind();
            if (OnPageChanged.HasDelegate)
                await OnPageChanged.InvokeAsync();
        }

        async Task ChangePageSize(int pageSize)
        {
            if (pageSize != PageSize)
            {
                PageNumber = 1;
                PageSize = pageSize;
                SholdRendered = true;
                await DataBind();
            }
        }

        public void SelectFirstRow()
        {
            if (SelectType == SelectType.Single)
                SelectRow(0);
        }

        public async Task SelectNextRow()
        {
            if (SelectType == SelectType.Single && SelectedRowIndex.HasValue)
            {
                if (SelectedRowIndex.Value + 1 < PageSize)
                    SelectRow(SelectedRowIndex.Value + 1);
                else
                {
                    if (PageNumber < PageCount)
                    {
                        await ChangePageNumber(PageNumber + 1);
                        SelectRow(0);
                    }
                }
            }
        }

        public async Task SelectPrevRow()
        {
            if (SelectType == SelectType.Single && SelectedRowIndex.HasValue)
            {
                if (SelectedRowIndex.Value > 0)
                    SelectRow(SelectedRowIndex.Value - 1);
                else
                {
                    if (PageNumber < 1)
                    {
                        await ChangePageNumber(PageNumber - 1);
                        SelectRow(PageSize - 1);
                    }
                }
            }
        }

        public int PageCount
        {
            get
            {
                return (Total - 1) / PageSize + 1;
            }
        }

        public async Task<TEntity> SelectRowById(int id)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var query = GetQuery(scope);
            query = GetOrderByQuery(query);
            var rowId = await query.GetRowNumber(id);
            if (rowId.HasValue)
            {
                var pageNumber = (rowId.Value - 1) / PageSize + 1;
                await this.ChangePageNumber(pageNumber);
                var rowIndex = (rowId.Value - 1) % PageSize;
                SelectRow(rowIndex);
                return Items[rowIndex];
            }
            return default(TEntity);
        }

        protected string ConvertToJson()
        {
            var setting = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, setting);
        }

        protected override void OnInitialized()
        {
            DeleteMessage = "آیا با حذف موافقید؟";
            tableAttrs = new Dictionary<string, object>();
            commandColumnAdded = false;
            base.OnInitialized();
        }

        private object GetFromToValue(LambdaExpression lambda)
        {
            var expr = (lambda.Body as MemberExpression);
            var member = expr.Member;
            var value = (expr.Expression as ConstantExpression).Value;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (member as FieldInfo).GetValue(value);
                case MemberTypes.Property:
                    return (member as PropertyInfo).GetValue(value);
                default:
                    throw new NotImplementedException("");
            };
        }

        protected override void OnParametersSet()
        {
            tableAttrs["cellpadding"] = "0";
            tableAttrs["cellspacing"] = "0";
            if (SelectType == SelectType.None)
                tableAttrs["class"] = "t-selectable";
            else
                tableAttrs.Remove("class");
            if (TableWidth.HasValue)
                tableAttrs["style"] = "width:" + TableWidth + "px";
            if (MultiInsert || AutoInsert)
                Inline = true;

            base.OnParametersSet();
        }

        protected async override Task OnParametersSetAsync()
        {
            var jsonSearch = Search == null ? "{}" : JsonConvert.SerializeObject(Search, new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
            if (jsonOldSearch != jsonSearch)
            {
                SholdRendered = true;
                jsonOldSearch = jsonSearch;
                if (columnsData != null)
                    await DataBind();
            }
            else if (SholdRendered && columnsData != null)
                await DataBind();
            await ParametersSetInitialAsync();
            await base.OnParametersSetAsync();
        }
    }
}
