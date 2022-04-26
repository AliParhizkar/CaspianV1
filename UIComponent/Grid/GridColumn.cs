using System;
using System.Linq;
using Caspian.Common;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class GridColumn<TEntity> where TEntity: class
    {
        object value;

        int? AggregateIndex;

        bool isAggregate;

        public bool IsCheckBox { get; private set; }

        public string Width { get; private set; }

        [CascadingParameter(Name = "Grid")]
        public DataGrid<TEntity> Grid { get; set; }

        [Parameter]
        public Expression<Func<TEntity, object>> Field { get; set; }

        [Parameter]
        public Expression<Func<IGrouping<TEntity, TEntity>, object>> AggregateField { get; set; }

        [Parameter]
        public RenderFragment Template { get; set; }

        [Parameter]
        public RenderFragment EditTemplate { get; set; }

        [CascadingParameter(Name = "GridEditableList")]
        public bool GridEditableList { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributs { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public object From { get; set; }

        [Parameter]
        public EventCallback<object> FromChanged { get; set; }

        [Parameter]
        public Expression<Func<object>> FromExpression { get; set; }

        [Parameter]
        public object To { get; set; }

        [Parameter]
        public EventCallback<object> ToChanged { get; set; }

        [Parameter]
        public Expression<Func<object>> ToExpression { get; set; }

        [Parameter]
        public OrderType? OrderType { get; set; }

        [Parameter]
        public bool ForTest { get; set; }

        [CascadingParameter(Name = "GridRowData")]
        public RowData<TEntity> RowData { get; set; }

        protected override void OnInitialized()
        {
            if (Attributs != null && Attributs.ContainsKey("style"))
            {
                var array = Attributs["style"].ToString().Split(";");
                Width = "";
                foreach (var item in array)
                {
                    var temp = item.Trim();
                    if (temp.StartsWith("width") || temp.StartsWith("min-width") || temp.StartsWith("max-width"))
                        Width += item + ';';
                }

            }
            if (AggregateField != null)
            {
                var tempExpr = AggregateField.Body;
                if (tempExpr.NodeType == ExpressionType.Convert)
                    tempExpr = (tempExpr as UnaryExpression).Operand;
                isAggregate = false;
                if (tempExpr.NodeType == ExpressionType.Call)
                {
                    var type = (tempExpr as MethodCallExpression).Method.DeclaringType;
                    isAggregate = type == typeof(Enumerable);
                }
                var param = Expression.Parameter(typeof(TEntity), "t");
                if (isAggregate)
                    AggregateIndex = Grid.GetAggregateColumnIndex();
                else
                {
                    var result = new ExpressionSurvey().ReduceAggregate(AggregateField.Body, param);
                    if (result.Type == typeof(string))
                        result = Expression.Convert(result, typeof(object));
                    result = Expression.Lambda(result, param);
                    Field = (Expression<Func<TEntity, object>>)result;
                }
            }
            if (Field != null)
            {
                Expression expr = Field.Body;
                if (expr.NodeType == ExpressionType.Call)
                {
                    var args = (expr as MethodCallExpression).Arguments;
                    if (args.Count == 0)
                        expr = (expr as MethodCallExpression).Object;
                    else
                        expr = (expr as MethodCallExpression).Arguments[0];
                }
                if (expr.NodeType == ExpressionType.Convert)
                    expr = (expr as UnaryExpression).Operand;
                if (expr.NodeType == ExpressionType.MemberAccess)
                {
                    var propertyExpr = expr as MemberExpression;
                    var type = propertyExpr.Type;
                    IsCheckBox = type == typeof(bool) || type == typeof(bool?);

                    var attr = propertyExpr.Member.GetCustomAttribute<DisplayNameAttribute>();
                    if (attr == null)
                        Title = Title ?? propertyExpr.Member.Name;
                    else
                        Title = Title ?? attr.DisplayName;
                }
            }
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (Template == null && !GridEditableList && RowData != null && RowData.Data != null)
            {
                if (Field != null)
                    value = Field.Compile().Invoke(RowData.Data);
                else if (RowData.DynamicData != null && AggregateIndex.HasValue)
                {
                    var properties = RowData.DynamicData.GetType().GetProperties().Where(t => t.Name.StartsWith("Info__"));
                    var index = AggregateIndex.Value % properties.Count();
                    var info = properties.Single(t => t.Name == "Info__" + index);
                    if (info != null)
                        value = info.GetValue(RowData.DynamicData);
                }
            }
            base.OnParametersSet();
        }
    }
}
