﻿using System;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

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
        IList<int> selectedIds;
        IList<object> DynamicData;
        IList<ColumnData> columnsData;
        IList<ColumnData> RangeFilterColumnsData;
        IDictionary<string, object> tableAttrs;
        IList<MemberExpression> SelectExpressions;


        public int? SelectedRowIndex { get; private set; }

        [Inject]
        IServiceProvider ServiceProvider { get; set; }

        [Inject]
        public FormAppState FormAppState { get; set; }

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

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            await OnAfterRenderOperation();
            
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
                    var tuple = await query.AggregateValuesAsync(aggregateExprList, PageNumber, PageSize);
                    Items = tuple.Item1;
                    DynamicData = tuple.Item2;
                }
                else
                {
                    Total = await query.CountAsync();
                    query = GetOrderByQuery(query);
                    if (PageNumber > 1 && !Batch)
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
                        if (PageNumber == 1)
                            Items = source.Take(PageSize).ToList();
                        else
                        {
                            var skip = (PageNumber - 1) * PageSize;
                            Items = source.Skip(skip).Take(PageSize).ToList();
                        }
                        ManageExpressionForUpsert(exprList);
                    }
                    else
                        Items = await query.Take(PageSize).GetValuesAsync<TEntity>(exprList);
                }
                await SetStateGridData();
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
                if (Items != null && SelectedRowIndex.HasValue && SelectedRowIndex >= 0 && SelectedRowIndex < Items.Count)
                {
                    var value = typeof(TEntity).GetPrimaryKey().GetValue(Items[SelectedRowIndex.Value]);
                    return Convert.ToInt32(value);
                }
                return null;
            }
        }

        internal Expression InternalConditionExpr { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpr { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

        public Window Control { get; set; }

        [Parameter]
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter]
        public int TableHeight { get; set; } = 250;

        [Parameter]
        public int? TableWidth { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnUpsert { get; set; }

        public EventCallback<TEntity> OnInternalUpsert { get; set; }

        [Parameter]
        public Func<TEntity, Task<bool>> OnDelete { get; set; }

        [Parameter]
        public EventCallback OnPageChanged { get; set; }

        internal EventCallback<TEntity> OnInternalDelete { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnRowSelect { get; set; }

        public EventCallback<int> OnInternalRowSelect { get; set; }

        public IList<TEntity> Items { get; set; }

        public int Total { get; set; }

        [Parameter]
        public int PageNumber { get; set; } = 1;

        [Parameter]
        public int PageSize { get; set; } = 5;

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

        [Parameter]
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

        public async Task ReloadAsync()
        {
            EnableLoading();

            await OnParametersSetAsync();
            var pageCount = (Total - 1) / PageSize + 1;
            if (pageCount < PageNumber)
            {
                await ChangePageNumber(pageCount);
                SelectedRowIndex = Items.Count - 1;
            }
            else if (SelectedRowIndex != null && SelectedRowIndex.Value >= Items.Count)
                SelectedRowIndex = Items.Count - 1;
            StateHasChanged();
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
            if (Batch)
                ShowItemsForBatch();
            else
            {
                SholdRendered = true;
                await DataBind();
            }
            if (OnPageChanged.HasDelegate)
                await OnPageChanged.InvokeAsync();
        }

        async Task ChangePageSize(int pageSize)
        {
            if (pageSize != PageSize)
            {
                PageNumber = 1;
                PageSize = pageSize;
                if (Batch)
                    ShowItemsForBatch();
                else
                {
                    SholdRendered = true;
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
                    if (PageNumber < PageCount)
                    {
                        SelectRow(0);
                        await ChangePageNumber(PageNumber + 1);
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
            var rowId = await query.GetRowNumber(scope.GetService<BaseService<TEntity>>().Context, id);
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
            OnInitializedOperation();
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
                SholdRendered = true;
                jsonOldSearch = jsonSearch;
                if (columnsData != null && !Batch)
                    await DataBind();
            }
            else if (SholdRendered && columnsData != null)
                await DataBind();
            await base.OnParametersSetAsync();
        }
    }
}
