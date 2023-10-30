using System;
using System.Linq;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Caspian.Common.RowNumber;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity>: DataView<TEntity>, IEnableLoadData, IGridRowSelect where TEntity: class
    {
        string jsonOldSearch;
        bool mustRender = true;
        bool commandColumnAdded;
        ElementReference mainDiv;
        int aggregateColumnIndex;
        IList<int> selectedIds;
        IList<object> DynamicData;
        IList<ColumnData> columnsData;
        IList<ColumnData> RangeFilterColumnsData;
        IDictionary<string, object> tableAttrs;
        IList<MemberExpression> SelectExpressions;

        [CascadingParameter]
        public CrudComponent<TEntity> CrudComponent { get; set; }

        [Parameter]
        public bool HidePageSize { get; set; }

        internal void AddColumnData(GridColumn<TEntity> column)
        {
            var columnData = new ColumnData()
            {
                Title = column.Title,
                Expression = column.Field?.Body,
                AggregateExpression = column.AggregateField?.Body,
                FromExpression = column.FromExpression,
                ToExpression = column.ToExpression,
                Sortable = column.Field?.Body as BinaryExpression == null,
                Width = column.Width,
                SortType = column.SortType,
                Resizeable = column.Template == null && !column.IsCheckBox
            };
            columnsData.Add(columnData);
            if (column.FromExpression != null || column.ToExpression != null)
                RangeFilterColumnsData.Add(columnData);
            StateHasChanged();
        }

        public override async Task DataBind()
        {
            if (columnsData.Count > 0 && shouldFetchData)
            {
                shouldFetchData = false;
                using var scope = ServiceScopeFactory.CreateScope();
                var query = GetQuery(scope);
                var exprList = new List<MemberExpression>();
                if (columnsData.Any(t => t.AggregateExpression != null))
                {
                    var aggregateExprList = columnsData.Where(t => t.AggregateExpression != null).Select(t => t.AggregateExpression).ToList();
                    Total = await query.CreateAggregateQuery(aggregateExprList).OfType<object>().CountAsync();
                    var tuple = await query.AggregateValuesAsync(aggregateExprList, pageNumber, PageSize);
                    items = tuple.Item1;
                    DynamicData = tuple.Item2;
                }
                else
                {
                    Total = await query.CountAsync();
                    query = GetOrderByQuery(query);
                    if (pageNumber > 1 && !Batch)
                    {
                        var skip = (pageNumber - 1) * PageSize;
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
                    if (Batch)
                    {
                        foreach (var info in typeof(TEntity).GetProperties())
                        {
                            if (info.PropertyType.IsValueType || info.PropertyType.IsNullableType())
                            {
                                var str = parameterExpr.Name + "." + info.Name;
                                if (!exprList.Any(t => t.ToString() == str))
                                    exprList.Add(Expression.Property(parameterExpr, info));
                            }
                        }
                    }
                    else
                    {
                        var expr1 = Expression.Property(parameterExpr, pKey);
                        if (!exprList.Any(t => t.ToString() == expr1.ToString()))
                            exprList.Add(expr1);
                    }
                    if (Batch)
                    {
                        source = (await query.GetValuesAsync<TEntity>(exprList)).ToList();
                        if (pageNumber == 1)
                            items = source.Take(PageSize).ToList();
                        else
                        {
                            var skip = (pageNumber - 1) * PageSize;
                            items = source.Skip(skip).Take(PageSize).ToList();
                        }
                        ManageExpressionForUpsert(exprList);
                    }
                    else
                        items = await query.Take(PageSize).GetValuesAsync<TEntity>(exprList);
                }
                await SetStateGridData();
            }
        }


        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await DataBind();
                StateHasChanged();
                await jsRuntime.InvokeVoidAsync("$.caspian.dadaGridBind", mainDiv);
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

        public IQueryable<TEntity> GetAllEntities(IServiceScope scope)
        {
            return GetQuery(scope);
        }

        IQueryable<TEntity> GetQuery(IServiceScope scope)
        {
            var service = scope.GetService<BaseService<TEntity>>();
            var query = service.GetAll(Search);
            Expression expr = null;
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            Expression expression = null;
            foreach (var col in columnsData.Where(t => t.FromExpression != null || t.ToExpression != null))
            {
                var colExpr = col.Expression;
                if (colExpr.NodeType == ExpressionType.Call) 
                    colExpr = (colExpr as MethodCallExpression).Arguments[0];
                Expression memberExpr = parameter.ReplaceParameter(colExpr);
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
            //expr = UpdateExpressionForMasterForm(parameter, expr);
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
                if (col.SortType != null && col.Sortable)
                {
                    Expression orderbyExpr = col.Expression;
                    ParameterExpression param = null; ;
                    if (orderbyExpr.NodeType == ExpressionType.Conditional)
                    {
                        foreach(var exp in new ExpressionSurvey().Survey(orderbyExpr))
                        {
                            param = exp.GetParameter();
                            if (param != null)
                                break;
                        }
                    }
                    else
                    {
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
                        param = Expression.Parameter(typeof(TEntity), "t");
                        orderbyExpr = param.ReplaceParameter(orderbyExpr as MemberExpression);
                    }

                    var lambdaExpression = Expression.Lambda(orderbyExpr, param);
                    if (col.SortType == SortType.Asc)
                        query = query.OrderBy(lambdaExpression).OfType<TEntity>();
                    else if (col.SortType == SortType.Decs)
                        query = query.OrderByDescending(lambdaExpression).OfType<TEntity>();
                }
                index++;
            }
            return query;
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
                if (items != null && SelectedRowIndex.HasValue && SelectedRowIndex >= 0 && SelectedRowIndex < items.Count)
                {
                    var value = typeof(TEntity).GetPrimaryKey().GetValue(items[SelectedRowIndex.Value]);
                    return Convert.ToInt32(value);
                }
                return null;
            }
        }

        internal Expression InternalConditionExpr { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

        public Window Control { get; set; }

        [Parameter]
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter]
        public int? TableWidth { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnRowSelect { get; set; }

        public EventCallback<int> OnInternalRowSelect { get; set; }

        [Parameter]
        public TEntity Search { get; set; }

        [Parameter]
        public SelectType SelectType { get; set; } = SelectType.Single;

        [Parameter]
        public RenderFragment<RowData<TEntity>> Columns { get; set; }

        [Parameter]
        public string DeleteMessage { get; set; }

        [Parameter]
        public RenderFragment ToolsBar { get; set; }

        [Parameter]
        public RenderFragment SearchTemplate { get; set; }

        async Task UpdateOrder()
        {
            shouldFetchData = true;
            await OnParametersSetAsync();
            StateHasChanged();
        }

        public TEntity GetSelectedData()
        {
            if (SelectedRowIndex == null || items == null || items.Count < SelectedRowIndex.Value || SelectedRowIndex == -1)
                return null;
            return items.ElementAt(SelectedRowIndex.Value);
        }

        public async Task ResetGrid()
        {
            SelectedRowIndex = 0;
            shouldFetchData = true;
            await ChangePageNumber(1);
        }

        public void SelectRow(int rowIndex)
        {
            SelectedRowIndex = rowIndex;
        }

        public async Task SelectFirstPage()
        {
            if (this.pageNumber != 1)
                await ChangePageNumber(1);
        }

        

        async Task ChangePageSize(int pageSize)
        {
            if (pageSize != PageSize)
            {
                pageNumber = 1;
                PageSize = pageSize;
                if (Batch)
                    ShowItemsForBatch();
                else
                {
                    shouldFetchData = true;
                    await DataBind();
                }
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
                    if (pageNumber < PageCount)
                    {
                        SelectRow(0);
                        await ChangePageNumber(pageNumber + 1);
                        StateHasChanged();
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
                    if (pageNumber < 1)
                    {
                        await ChangePageNumber(pageNumber - 1);
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
            var rowId = await query.GetRowNumber(scope.GetService<BaseService<TEntity>>().Context, id);
            if (rowId.HasValue)
            {
                var pageNumber = (rowId.Value - 1) / PageSize + 1;
                await this.ChangePageNumber(pageNumber);
                var rowIndex = (rowId.Value - 1) % PageSize;
                SelectRow(rowIndex);
                return items[rowIndex];
            }
            return default(TEntity);
        }

        protected override void OnInitialized()
        {
            if (CaspianDataService.Language == Language.Fa)
                DeleteMessage = "آیا با حذف موافقید؟";
            else
                DeleteMessage = "Do you agree to delete?";
            if (SelectType == SelectType.Multi)
                selectedIds = new List<int>();
            tableAttrs = new Dictionary<string, object>();
            commandColumnAdded = false;
            if (CrudComponent != null)
                CrudComponent.CrudGrid = this;
            RangeFilterColumnsData = new List<ColumnData>();
            ContentHeight = 250;
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
            OnParameterSetInint();
            tableAttrs["cellpadding"] = "0";
            tableAttrs["cellspacing"] = "0";
            if (SelectType == SelectType.None)
                tableAttrs["class"] = "t-selectable";
            else
                tableAttrs.Remove("class");
            if (TableWidth.HasValue)
                tableAttrs["style"] = "width:" + TableWidth + "px";
            foreach(var col in RangeFilterColumnsData)
            {
                if (col.FromExpression != null)
                {
                    var fromValue = GetFromToValue(col.FromExpression);
                    if (fromValue == null && col.FromValue != null || fromValue != null && !fromValue.Equals(col.FromValue))
                    {
                        EnableLoading();
                        col.FromValue = fromValue;
                    }
                }
                if (col.ToExpression != null)
                {
                    var toValue = GetFromToValue(col.ToExpression);
                    if (toValue == null && col.ToValue != null || toValue != null && !toValue.Equals(col.ToValue))
                    {
                        EnableLoading();
                        col.ToValue = toValue;
                    }
                }
            }
            base.OnParametersSet();
        }

        protected async override Task OnParametersSetAsync()
        {
            var jsonSearch = Search == null ? "{}" : JsonSerializer.Serialize(Search, Search.GetType());
            if (jsonOldSearch != jsonSearch)
            {
                shouldFetchData = true;
                jsonOldSearch = jsonSearch;
                if (columnsData != null && !Batch)
                    await DataBind();
            }
            else if (shouldFetchData && columnsData != null)
                await DataBind();
            await base.OnParametersSetAsync();
        }
    }
}
