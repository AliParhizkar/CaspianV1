using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class CaspianForm<TEntity>: ICaspianContainer, ICaspianForm<TEntity> where TEntity : class
    {
        string ErrorMessage;
        IList<IControl> controls;
        bool addControls, formSubmitted, submitting;
        TEntity oldModel;
        IControl firstControl;

        string ICaspianForm.MasterIdName { get; set; }
        bool ICaspianForm.IgnoreOnValidSubmit { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public RenderFragment<TEntity> Content { get; set; }

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }

        [Parameter]
        public TEntity Model { get; set; }

        public EditContext EditContext { get; private set; }

        [Parameter]
        public IUIService<TEntity> Service { get; set; }

        [Parameter]
        public IBatchService<TEntity> DetailsService { get;set;}

        [Parameter]
        public EventCallback<TEntity> OnInvalidSubmit { get; set; }

        [Parameter]
        public EventCallback<CancelableEvent<TEntity>> OnSubmit { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnReset { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnValidSubmit { get; set; }

        internal EventCallback<TEntity> OnInternalSubmit { get; set; }

        internal EventCallback<TEntity> OnInternalValidSubmit { get; set; }

        public EventCallback<TEntity> OnInternalInvalidSubmit { get; set; }

        public bool Disposed { get; private set; }

        internal EventCallback OnInternalReset { get; set; }

        Type GetServiceType()
        {
            if (PageData != null)
                provider.GetService<CaspianDataService>().UserId = PageData.UserId;
            var services = provider.GetServices<IBaseService<TEntity>>();
            return services.Count() == 1 ? services.SingleOrDefault()?.GetType() : null;
        }

        [CascadingParameter]
        internal PageData PageData { get; set; }

        string ICaspianContainer.GetLabelContainerCSSClassName(int colSpan)
        {
            var className = PageData?.RightToLeft == true ? "pe-2" : "ps-2";
            className += " col-md-";
            if (ColumnsCount == 1)
                return className + (12 - colSpan);
            if (colSpan < 6)
                className += (6 - colSpan);
            else
                className += (12 - colSpan);
            return className;
        }

        public void SetFirstControl(IControl control)
        {
            if (firstControl == null || firstControl.InputElement == null) 
                firstControl = control;
        }

        [Parameter]
        public int ColumnsCount { get; set; } = 1;

        public async Task FocusAsync()
        {
            if (firstControl != null) 
                await firstControl?.FocusAsync();
        }

        public void AddControl(IControl control)
        {
            if (addControls)
            {
                if (controls.Contains(control))
                    controls.Clear();
                controls.Add(control);
            }
        }

        public CaspianValidationValidator<TEntity> ValidationValidator { get; set; }

        public IControl GetFirstInvalidControl()
        {
            return controls.FirstOrDefault(t => t.HasError());
        }

        protected override void OnInitialized()
        {
            if (NestedForm != null)
                throw new CaspianException("Nested form is disallowed. You can have multiple forms in the page, but nested form is disallowed");
            if (Service != null)
                (Service as IInternalUIService<TEntity>).FormInitializer(this);
            controls = new List<IControl>();
            if (FormAppState == null)
                FormAppState = new FormAppState();
            FormAppState.AllControlsIsValid = true;
            FormAppState.ErrorMessage = null;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (Service != null)
            {
                await (Service as IInternalUIService<TEntity>).FetchAsync();
                EditContext = new EditContext(Service.UpsertData);
            }
            await base.OnInitializedAsync();
        }

        public async Task<bool?> SubmitAsync()
        {
            return await OnFormSubmitHandler(EditContext);
        }

        [CascadingParameter(Name = "ParentForm")]
        internal ICaspianForm NestedForm { get; set; }

        internal EventCallback OnBeforeValidate {  get; set; }

        public async Task<ValidationResult> ValidateAsync()
        {
            EditContext.Validate();
            EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
            var result = await (Task<ValidationResult>)asyncValidationTask;
            return result;
        }

        async Task<bool?> OnFormSubmitHandler(EditContext context)
        {
            if (submitting)
                return null;
            submitting = true;
            addControls = true;
            controls.Clear();
            await Task.Delay(10);
            addControls = false;
            var cancel = false;
            if (OnSubmit.HasDelegate)
            {
                var cancelableEvent = new CancelableEvent<TEntity>(EditContext.Model as TEntity);
                await OnSubmit.InvokeAsync(cancelableEvent);
                cancel = cancelableEvent.Cancel;
            }
            if (cancel)
                return null;
            if (OnInternalSubmit.HasDelegate)
                await OnInternalSubmit.InvokeAsync(EditContext.Model as TEntity);
            FormAppState.AllControlsIsValid = true;
            FormAppState.ErrorMessage = null;
            ErrorMessage = null;
            if ((Service as IInternalUIService<TEntity>)?.OtherType != null)
            {
                EditContext.Properties["DetailType"] = (Service as IInternalUIService<TEntity>).OtherType;
            }

            var qqq = EditContext.Validate();
            if (OnBeforeValidate.HasDelegate)
                await OnBeforeValidate.InvokeAsync();
            if (ValidationValidator == null)
            {
                var services = provider.GetServices<IBaseService<TEntity>>();
                if (services.Count() > 1)
                {
                    var message = "";
                    foreach(var service in services)
                    {
                        if (message.HasValue())
                            message += " and ";
                        message += service.GetType().Name;
                    }
                    message = $"Caspian Exception: {message} Types implement IBaseService<{typeof(TEntity).Name}> so you should specify service with CaspianValidationValidator component on CaspianForm component";
                    throw new CaspianException(message);
                }
            }
            EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
            var result = await (Task<ValidationResult>)asyncValidationTask;
            formSubmitted = true;
            
            if (result.IsValid)
            {
                if (OnValidSubmit.HasDelegate)
                    await OnValidSubmit.InvokeAsync(context.Model as TEntity);
                if (OnInternalValidSubmit.HasDelegate)
                    await OnInternalValidSubmit.InvokeAsync(context.Model as TEntity);
            }
            else
            {
                if (FormAppState.AllControlsIsValid)
                    ErrorMessage = EditContext.GetValidationMessages().First();

                FormAppState.ValidationChecking = true;
                if (OnInvalidSubmit.HasDelegate)
                    await OnInvalidSubmit.InvokeAsync(EditContext.Model as TEntity);
                if (OnInternalInvalidSubmit.HasDelegate)
                    await OnInternalInvalidSubmit.InvokeAsync(EditContext.Model as TEntity);
            }
            submitting = false;
            return result.IsValid;
        }

        async Task ResetFormAsync()
        {
            firstControl = null;
            foreach (var control in controls)
            {
                await control.ResetAsync();
            }
            if (OnInternalReset.HasDelegate)
                await OnInternalReset.InvokeAsync(Service?.UpsertData);
            if (OnReset.HasDelegate)
                await OnReset.InvokeAsync();
        }

        protected override void OnParametersSet()
        {
            if (EditContext == null || !Model.Equals(oldModel))
            {
                EditContext = new EditContext(Model);
                oldModel = Model;
            }
            base.OnParametersSet();
        }

        internal void SetModel(TEntity entity)
        {
            EditContext = new EditContext(entity);
            oldModel = entity;
            Model = entity;
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (formSubmitted)
            {
                formSubmitted = false;
                var ctr = controls.FirstOrDefault(t => t.HasError());
                if (ctr == null)
                {
                    if (FormAppState.ErrorMessage.HasValue())
                        ErrorMessage = FormAppState.ErrorMessage;
                }
                else
                    await ctr.FocusAsync();
            }
            if (ErrorMessage.HasValue())
            {
                var message = ErrorMessage;
                ErrorMessage = null;
                await jsRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task ResetAsync()
        {
            await ResetFormAsync();
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
