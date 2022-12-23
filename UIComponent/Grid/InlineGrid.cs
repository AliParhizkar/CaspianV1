using System;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : ComponentBase, IEnableLoadData, IGridRowSelect where TEntity : class
    {
        Type serviceType;
        string errorMessage;
        bool shouldSetFocuc;
        TEntity selectedEntity;
        EditContext EditContext;
        EditContext InsertContext;
        CaspianContainer insertContiner;
        CaspianContainer updateContiner;
        RowData<TEntity> insertedEntity;
        RowData<TEntity> updatedEntity;
        bool insertContinerHouldhasFocus;
        PropertyInfo ignoreValidateProperty;
        CaspianValidationValidator validator;

        public void OnInitializedOperation()
        {
            var type = typeof(ISimpleService<TEntity>);
            using var scope = ServiceScopeFactory.CreateScope();
            serviceType = scope.ServiceProvider.GetService(type).GetType();
            if (!AutoHide && Inline)
                CreateInsert();
        }

        [Inject]
        public BatchService BatchService { get; set; }

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public bool AutoHide { get; set; } 

        [Parameter]
        public Expression<Func<TEntity, Object>> IgnoreForeignKeyInfo { get; set; } 

        public void SetSelectedId(TEntity entity)
        {
            var model = entity.CreateNewSimpleEntity();
            if (updatedEntity == null)
                updatedEntity = new RowData<TEntity>();
            updatedEntity.Data = model;
            updatedEntity.UpsertMode = UpsertMode.Edit;
            EditContext = new EditContext(model);
            selectedEntity = entity;
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
            HideInsertIcon = !AutoHide && Inline;
        }

        async Task OnAfterRenderOperation()
        {
            if (insertContinerHouldhasFocus)
            {
                insertContinerHouldhasFocus = false;
                await insertContiner.FocusAsync();
            }
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

        public async Task CalcelEdit(UpsertMode upsertMode)
        {
            if (upsertMode == UpsertMode.Edit)
            {
                selectedEntity = null;
                EditContext = null;
            }
            else
                await ReadyToInsert();
            StateHasChanged();
        }

        async Task ReadyToInsert()
        {
            if (Inline)
            {
                if (AutoHide)
                {
                    insertedEntity = null;
                    InsertContext = null;
                }
                else
                {
                    await insertContiner.ResetAsync();
                    if (insertedEntity == null)
                    {
                        insertedEntity = new RowData<TEntity>();
                        insertedEntity.UpsertMode = UpsertMode.Insert;
                    }
                    insertedEntity.Data = Activator.CreateInstance<TEntity>();
                    if (BatchService.MasterId > 0)
                        BatchService.IgnorePropertyInfo.SetValue(insertedEntity.Data, BatchService.MasterId);
                    InsertContext = new EditContext(insertedEntity.Data);
                    insertContiner.Focus();
                }
            }
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
                    selectedEntity = null;
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
                    var newEntity = Activator.CreateInstance<TEntity>();
                    foreach (var info in typeof(TEntity).GetProperties())
                    {
                        var type = info.PropertyType;
                        if (type.IsValueType || type.IsNullableType() || type == typeof(string))
                            info.SetValue(newEntity, info.GetValue(insertedEntity.Data));
                    }
                    await InsertAsync(newEntity);
                    await ReadyToInsert();
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
            if (BatchService.MasterId > 0)
                BatchService.IgnorePropertyInfo.SetValue(insertedEntity.Data, BatchService.MasterId);
            InsertContext = new EditContext(insertedEntity.Data);
            insertContinerHouldhasFocus = AutoHide;
            StateHasChanged();

        }
    }
}
