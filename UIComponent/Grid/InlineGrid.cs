using System;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : ComponentBase, IEnableLoadData, IGridRowSelect where TEntity : class
    {
        int? selectedId;
        bool shouldSetFocuc;
        CaspianContainer updateContiner;
        Type serviceType;
        EditContext EditContext;
        EditContext InsertContext;
        string errorMessage;
        CaspianValidationValidator validator;
        PropertyInfo ignoreValidateProperty;
        RowData<TEntity> insertedEntity;

        public void OnInitializedOperation()
        {
            var type = typeof(ISimpleService<TEntity>);
            using var scope = ServiceScopeFactory.CreateScope();
            serviceType = scope.ServiceProvider.GetService(type).GetType();
            //insertedEntity = new RowData<TEntity>();
            //insertedEntity.UpsertMode = UpsertMode.Insert;
            //insertedEntity.Data = Activator.CreateInstance<TEntity>();
        }

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public bool AutoInsert { get; set; }

        [Parameter]
        public Expression<Func<TEntity, Object>> IgnoreForeignKeyInfo { get; set; } 

        public void SetSelectedId(TEntity entity)
        {
            var id = typeof(TEntity).GetPrimaryKey().GetValue(entity);
            selectedId = Convert.ToInt32(id);
            shouldSetFocuc = true;
            StateHasChanged();
        }

        void OnParameterSetInint()
        {
            if (IgnoreForeignKeyInfo != null && ignoreValidateProperty == null)
            {
                Expression expr = IgnoreForeignKeyInfo.Body;
                if (expr.NodeType == ExpressionType.Convert)
                    expr = (expr as UnaryExpression).Operand;
                ignoreValidateProperty = (expr as MemberExpression).Member as PropertyInfo;
            }
        }

        void CreateEditContext(TEntity entity)
        {
            var shouldCreate = false;
            if (EditContext == null)
                shouldCreate = true;
            else
            {
                var id = typeof(TEntity).GetPrimaryKey().GetValue(EditContext.Model);
                if (Convert.ToInt32(id) != selectedId.Value)
                    shouldCreate = true;
            }
            if (shouldCreate)
            {
                var model = Activator.CreateInstance<TEntity>();
                foreach(var info in typeof(TEntity).GetProperties())
                {
                    var type = info.PropertyType;
                    if (type.IsValueType || type.IsNullableType() || type == typeof(string))
                        info.SetValue(model, info.GetValue(entity));
                }
                EditContext = new EditContext(model);
            }
        }

        async Task OnAfterRenderOperation()
        {
            if (shouldSetFocuc)
            {
                shouldSetFocuc = false;
                await updateContiner.FocusAsync();
            }
            if (errorMessage.HasValue())
            {
                await jsRuntime.InvokeVoidAsync("$.telerik.showMessage", errorMessage);
                errorMessage = null;
            }
            else if (FormAppState.Element.HasValue)
            {
                await jsRuntime.InvokeVoidAsync("$.telerik.focusAndShowErrorMessage", FormAppState.Element, FormAppState.ErrorMessage);
                FormAppState.Element = null;
            }
        }

        public void CalcelEdit(UpsertMode upsertMode)
        {
            if (upsertMode == UpsertMode.Edit)
            {
                selectedId = null;
                EditContext = null;
            }
            else
            {
                insertedEntity = null;
                InsertContext = null;
            }
            StateHasChanged();
        }

        public async Task ValidateUpsert(UpsertMode upsertMode)
        {
            if (upsertMode == UpsertMode.Edit)
            {
                EditContext.Validate();
                FormAppState.AllControlsIsValid = true;
                FormAppState.Element = null;
                EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
                var result = await (Task<ValidationResult>)asyncValidationTask;
                if (result.IsValid)
                {
                    selectedId = null;
                    await UpdateAsync(EditContext.Model as TEntity);
                }
                else
                {
                    if (FormAppState.AllControlsIsValid)
                        errorMessage = result.Errors[0].ErrorMessage;
                }
            }
            else
            {
                InsertContext.Validate();
                FormAppState.AllControlsIsValid = true;
                FormAppState.Element = null;
                InsertContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
                var result = await (Task<ValidationResult>)asyncValidationTask;
                if (result.IsValid)
                {
                    await InsertAsync(InsertContext.Model as TEntity);
                    InsertContext = null;
                    insertedEntity = null;
                }
                else
                {
                    if (FormAppState.AllControlsIsValid)
                        errorMessage = result.Errors[0].ErrorMessage;
                }
            }
            StateHasChanged();

        }

        public void CreateInsert()
        {
            insertedEntity = new RowData<TEntity>();
            insertedEntity.UpsertMode = UpsertMode.Insert;
            insertedEntity.Data = Activator.CreateInstance<TEntity>();
            InsertContext = new EditContext(insertedEntity.Data);
            StateHasChanged();
        }
    }
}
