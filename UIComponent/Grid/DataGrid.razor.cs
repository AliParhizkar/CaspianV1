using Caspian.Common;
using System.Text.Json;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Caspian.Common.RowNumber;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity>: DataView<TEntity>, IEnableLoadData, IGridRowSelect where TEntity: class
    {
        bool mustRender = true;
        bool commandColumnAdded;
        int aggregateColumnIndex;
        IList<int> selectedIds;
        IList<object> DynamicData;
        IList<ColumnData> columnsData;
        IDictionary<string, object> tableAttrs;

        [Parameter]
        public bool HidePageSize { get; set; }

        [Parameter]
        public Func<TEntity, string> RowDataBindingCssClass { get; set; }

        internal void UpdateColumnData(string id, bool hidden)
        {
            var columnData = columnsData.SingleOrDefault(t => t.Id == id);
            if (columnData != null)
                columnData.Hidden = hidden;
        }

        internal void AddColumnData(GridColumn<TEntity> column)
        {
            if (column.Hidden && !column.Id.HasValue())
                throw new CaspianException("For dynamic column (Hidden is true) is must specify");
            var columnData = new ColumnData();
            columnData.Expression = column.Field?.Body;
            columnData.DataField = column.DataField;
            columnData.Id = column.Id;
            if (!column.DataField)
            {
                columnData.Width = column.Width;
                columnData.Title = column.Title;
                columnData.AggregateExpression = column.AggregateField?.Body;
                columnData.Sortable = column.Field?.Body as BinaryExpression == null;
                columnData.SortType = column.SortType;
                columnData.Resizable = column.Template == null && !column.IsCheckBox;
            }
            columnsData.Add(columnData);
            StateHasChanged();
        }

        public override async Task DataBind()
        {
            if (columnsData.Count > 0 && shouldFetchData)
            {
                shouldFetchData = false;
                using var scope = ServiceScopeFactory.CreateScope();
                scope.SetUserId(PageData);
                var query = GetQuery(scope);
                var exprList = new List<MemberExpression>();
                if (columnsData.Any(t => t.AggregateExpression != null))
                {
                    var aggregateExprList = columnsData.Where(t => t.AggregateExpression != null).Select(t => t.AggregateExpression).ToList();
                    var aggregateQuery = query.CreateAggregateQuery(aggregateExprList);
                    Total = await aggregateQuery.OfType<object>().CountAsync();
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
                        if (DetailsService != null)
                        {
                            foreach(var item in DetailsService.ChangedEntities)
                            {
                                if (item.ChangeStatus == ChangeStatus.Added)
                                    source.Add(item.Entity);
                                else 
                                {
                                    TEntity old = default(TEntity);
                                    var id = Convert.ToInt32(pKey.GetValue(item.Entity));
                                    foreach(var entity in source)
                                    {
                                        if (Convert.ToInt32(pKey.GetValue(entity)) == id)
                                        {
                                            old = entity;
                                            break;
                                        }
                                    }
                                    if (item.ChangeStatus == ChangeStatus.Deleted)
                                        source.Remove(old);
                                    else if (item.ChangeStatus == ChangeStatus.Updated)
                                    {
                                        var index = source.IndexOf(old);
                                        source.RemoveAt(index);
                                        source.Insert(index, item.Entity);
                                    }
                                }
                            }
                        }
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

                if (OnLoaded != null)
                    OnLoaded();
                await SetStateGridData();
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                shouldFetchData = true;
                await DataBind();
                await jsRuntime.InvokeVoidAsync("caspian.common.bindDataGrid", mainDiv);
                StateHasChanged();
            }
            if (insertContainerHoldHasFocus && insertContainer != null)
            {
                insertContainerHoldHasFocus = false;
                await insertContainer.FocusAsync();
            }
            if (FormAppState.Control != null)
            {
                if (FormAppState.Control.InputElement.HasValue && Inline)
                    await FormAppState.Control.FocusAsync();
                FormAppState.Control = null;
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
                    Resizable = false
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

        internal override IQueryable<TEntity> GetQuery(IServiceScope scope)
        {
            var service = scope.GetService<BaseService<TEntity>>();
            var service1 = Service as IInternalSearchService<TEntity>;
            var query = service.Search(Search, service1?.SearchData, service1?.EnumFields, service1?.ValueTypes);
            Expression expr = null;
            var parameter = Expression.Parameter(typeof(TEntity), "t");
            Expression expression = null;
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
            bool isThenBy = false;
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
                    if (isThenBy)
                    {
                        if (col.SortType == SortType.Asc)
                            query = query.ThenBy(lambdaExpression);
                        else if (col.SortType == SortType.Decs)
                            query = query.ThenByDescending(lambdaExpression);
                    }
                    else
                    {
                        isThenBy = true;
                        if (col.SortType == SortType.Asc)
                            query = query.OrderBy(lambdaExpression);
                        else if (col.SortType == SortType.Decs)
                            query = query.OrderByDescending(lambdaExpression);
                    }
                }
            }
            return query;
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

        [Parameter]
        public Func<IQueryable<TEntity>, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter]
        public Func<IServiceScope, IQueryable<TEntity>, IQueryable<TEntity>> OnDataBinding1 { get; set; } 

        [Parameter]
        public int? TableWidth { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnRowSelect { get; set; }

        public EventCallback<TEntity> OnInternalRowSelect { get; set; }

        [Parameter]
        public RenderFragment<RowData<TEntity>> Columns { get; set; }

        [Parameter]
        public RenderFragment ToolsBar { get; set; }

        [Parameter]
        public RenderFragment SearchTemplate { get; set; }

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
                if (SelectedRowIndex.Value + 1 < PageSize && SelectedRowIndex.Value + 1 < items.Count)
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

        public override async Task<TEntity> SelectRowById(int id)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            scope.SetUserId(PageData);
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
            return default;
        }

        protected override void OnInitialized()
        {
            if (SelectType == SelectType.Multi)
                selectedIds = new List<int>();
            tableAttrs = new Dictionary<string, object>();
            commandColumnAdded = false;
            base.OnInitialized();
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
            base.OnParametersSet();
        }

        protected async override Task OnParametersSetAsync()
        {
            if (columnsData != null && !Batch)
                await DataBind();
            if (shouldFetchData && columnsData?.Count > 0 && Batch)
            {
                /// Set InternalConditionExpr to Filter DataGrid
                var param = Expression.Parameter(typeof(TEntity), "t");
                var pKey = typeof(TEntity).GetPrimaryKey();
                Expression expr = Expression.Property(param, pKey);
                var masterId = Convert.ChangeType(DetailsService.MasterId, pKey.PropertyType);
                expr = Expression.Equal(expr, Expression.Constant(masterId));
                InternalConditionExpr = (DetailsService as IInternalBatchService<TEntity>).GetDetailsFilterExpression();
                /// -----------------------
                await DataBind();
                if (DetailsService.ChangedEntities == null)
                    throw new CaspianException($"Caspian Exception: please specify ChangedEntities parameter in DataGrid<{typeof(TEntity).Name}>");
                foreach(var entity in DetailsService.ChangedEntities)
                {
                    if (entity.ChangeStatus == ChangeStatus.Added)
                        source.Add(entity.Entity);
                    else 
                    {
                        var id = Convert.ToInt32(pKey.GetValue(entity.Entity));
                        if (id > 0)
                        {
                            foreach(var temp in source)
                            {
                                var newId = Convert.ToInt32(pKey.GetValue(temp));
                                if (newId == id)
                                {
                                    if (entity.ChangeStatus == ChangeStatus.Deleted)
                                        source.Remove(temp);
                                    else
                                    {
                                        var index = source.IndexOf(temp);
                                        source.Insert(index, entity.Entity);
                                        source.Remove(temp);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                items = source.Take(PageSize).ToList();
            }
            await base.OnParametersSetAsync();
        }
    }
}
