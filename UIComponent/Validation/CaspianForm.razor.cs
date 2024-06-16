using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class CaspianForm<TEntity>: ICaspianForm where TEntity : class
    {
        string ErrorMessage;
        bool checkValidation;
        IList<IControl> controls;
        bool addControls;
        string ICaspianForm.MasterIdName { get; set; }
        bool ICaspianForm.IgnoreOnValidSubmit { get; set; }
        TEntity oldModel;
        IControl firstControl;

        [CascadingParameter]
        public CrudComponent<TEntity> CrudComponent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TEntity Model { get; set; }

        public EditContext EditContext { get; private set; }

        [Parameter]
        public ISimpleService<TEntity> Service { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnInvalidSubmit { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnSubmit { get; set; }

        [Parameter]
        public EventCallback OnReset { get; set; }

        [Parameter]
        public EventCallback<EditContext> OnValidSubmit { get; set; }

        internal EventCallback<EditContext> OnInternalSubmit { get; set; }

        internal EventCallback<EditContext> OnInternalValidSubmit { get; set; }

        public EventCallback<EditContext> OnInternalInvalidSubmit { get; set; }

        internal EventCallback OnInternalReset { get; set; }

        public void SetFirstControl(IControl control)
        {
            if (firstControl == null || firstControl.InputElement == null) 
                firstControl = control;
        }

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

        public CaspianValidationValidator ValidationValidator { get; set; }

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
            if (CrudComponent != null)
                CrudComponent.UpsertForm = this;
            FormAppState.AllControlsIsValid = true;
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

        async Task OnFormSubmitHandler(EditContext context)
        {
            addControls = true;
            controls.Clear();
            await Task.Delay(10);
            addControls = false;
            if(OnSubmit.HasDelegate)
                await OnSubmit.InvokeAsync(EditContext);
            if (OnInternalSubmit.HasDelegate)
                await OnInternalSubmit.InvokeAsync(EditContext);
            FormAppState.AllControlsIsValid = true;
            ErrorMessage = null;
            EditContext.Validate();
            EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
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
            }
            var result = await (Task<ValidationResult>)asyncValidationTask;
            if (result.IsValid)
            {
                if (OnValidSubmit.HasDelegate)
                    await OnValidSubmit.InvokeAsync(context);
                if (OnInternalValidSubmit.HasDelegate)
                    await OnInternalValidSubmit.InvokeAsync(context);
            }
            else
            {
                if (FormAppState.AllControlsIsValid)
                    ErrorMessage = EditContext.GetValidationMessages().First();
                FormAppState.ValidationChecking = true;
                if (OnInvalidSubmit.HasDelegate)
                    await OnInvalidSubmit.InvokeAsync(EditContext);
                if (OnInternalInvalidSubmit.HasDelegate)
                    await OnInternalInvalidSubmit.InvokeAsync(EditContext);
            }
        }

        async Task ResetFormAsync()
        {
            //firstControl = null;
            //foreach (var control in controls)
            //{
            //    await control.ResetAsync();
            //}
            //if (OnReset.HasDelegate)
            //    await OnReset.InvokeAsync();
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

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (ErrorMessage.HasValue())
            {
                var message = ErrorMessage;
                ErrorMessage = null;
                await jsRuntime.InvokeVoidAsync("$.caspian.showMessage", message);
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
            await OnSubmit.InvokeAsync(EditContext);
            EditContext.Validate();
        }

        public void Dispose()
        {
            Service?.ClearForm();
        }

    }
}
