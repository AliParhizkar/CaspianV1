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
        bool checkValidation;
        IList<IControl> controls;
        bool addControls;
        string ICaspianForm.MasterIdName { get; set; }
        bool ICaspianForm.IgnoreOnValidSubmit { get; set; }
        TEntity oldModel;
        IControl firstControl;

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
        public EventCallback<TEntity> OnInvalidSubmit { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnSubmit { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnReset { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnValidSubmit { get; set; }

        internal EventCallback<TEntity> OnInternalSubmit { get; set; }

        internal EventCallback<TEntity> OnInternalValidSubmit { get; set; }

        public EventCallback<TEntity> OnInternalInvalidSubmit { get; set; }

        internal EventCallback OnInternalReset { get; set; }

        [CascadingParameter(Name = "CSS-Right-To-Left")]
        internal bool RightToLeft { get; set; }

        string ICaspianContainer.GetLableContainerCSSClassName(int colSpan, int? totalSpan)
        {
            var className = RightToLeft ? "pe-2" : "ps-2";
            className += " col-md-";
            if (totalSpan.HasValue)
                return className + (totalSpan.Value - colSpan);
            if (ColumnsCount == 1)
                return className + (12 - colSpan);
            if (colSpan < 6)
                className += (6 - colSpan);
            else
                className += (12 - colSpan);
            return className;
        }

        string ICaspianContainer.GetControlContainerCSSClassName(int colSpan, int? totalSpan)
        {
            var str = $"col-md-{colSpan} ";
            return str + (RightToLeft ? "ps-2" : "pe-2");
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
            if (Service != null)
            {
                Service.Form = this;
                Service.FormInitialize();
            }
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
                await Service.FetchAsync();
                EditContext = new EditContext(Service.UpsertData);
            }
            await base.OnInitializedAsync();
        }

        public async Task SubmitAsync()
        {
            await OnFormSubmitHandler(EditContext);
        }

        async Task OnFormSubmitHandler(EditContext context)
        {
            addControls = true;
            controls.Clear();
            await Task.Delay(10);
            addControls = false;
            if(OnSubmit.HasDelegate)
                await OnSubmit.InvokeAsync(EditContext.Model as TEntity);
            if (OnInternalSubmit.HasDelegate)
                await OnInternalSubmit.InvokeAsync(EditContext.Model as TEntity);
            FormAppState.AllControlsIsValid = true;
            FormAppState.ErrorMessage = null;
            ErrorMessage = null;
            if (Service?.DetailType != null)
                EditContext.Properties["DetailType"] = Service.DetailType;
            EditContext.Validate();
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
                    message = $"Caspian Exception: {message} Types impiliment IBaseService<{typeof(TEntity).Name}> so you should specify service with CaspianValidationValidator component on CaspianForm component";
                    throw new CaspianException(message);
                }
                else if (services.Count()  == 0)
                {

                }
            }
            EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
            var result = await (Task<ValidationResult>)asyncValidationTask;
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
        }

        async Task ResetFormAsync()
        {
            firstControl = null;
            foreach (var control in controls)
            {
                await control.ResetAsync();
            }
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
            if (ErrorMessage.HasValue())
            {
                var message = ErrorMessage;
                ErrorMessage = null;
                await jsRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
            }   
            //var ctr = controls.FirstOrDefault(t => t.HasError());
            //if (ctr != null)
            //    await ctr.FocusAsync();
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task ResetAsync()
        {
            await ResetFormAsync();
        }

        async void OnFormSubmit()
        {
            ErrorMessage = null;
            await OnSubmit.InvokeAsync(EditContext.Model as TEntity);
            EditContext.Validate();
        }

        public void Dispose()
        {
            Service?.ClearForm();
        }

    }
}
