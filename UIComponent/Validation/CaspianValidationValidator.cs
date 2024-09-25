using Caspian.Common;
using FluentValidation;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using FluentValidation.Results;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caspian.UI
{
    public class CaspianValidationValidator: ComponentBase, IControlFocuseValidation
    {
        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public FormAppState FormAppState { get; set; }

        [Inject]
        public BatchServiceData BatchServiceData { get; set; }

        [Inject]
        public CaspianDataService CaspianDataService { get; set; }

        [CascadingParameter]
        private EditContext EditContext { get; set; }

        [CascadingParameter(Name = "ParentForm")]
        private ICaspianForm CaspianForm { get; set; }

        [Parameter]
        public Type ValidatorType { get; set; }

        [Parameter]
        public bool OnlyValidateOnSubmit { get; set; } = true;

        [Parameter]
        public object Source { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        public string MasterIdName { get; set; }

        IValidator Validator;
        ValidationMessageStore ValidationMessageStore;

        public bool IsFirstInvalidControl { get; set; }

        private void HookUpEditContextEvents()
        {
            EditContext.OnValidationRequested += async (sender, args) => await ValidationRequested(sender, args);
            //EditContext.OnFieldChanged += async (sender, args) => await FieldChanged(sender, args);
        }

        async Task FieldChanged(object sender, FieldChangedEventArgs args)
        {
            ValidationMessageStore.Clear();
            var list = new List<string>
            {
                "default"
            };
            var chain = new PropertyChain();
            chain.Add(args.FieldIdentifier.FieldName);
            var context = new ValidationContext<object>(EditContext.Model, chain, new RulesetValidatorSelector(list));
            using var scope = ServiceScopeFactory.CreateScope();
            if (CaspianDataService != null)
            {
                var dataService = scope.GetService<CaspianDataService>();
                dataService.UserId = CaspianDataService.UserId;
                dataService.Language = CaspianDataService.Language;
            }
            Validator = (IValidator)Activator.CreateInstance(ValidatorType, scope.ServiceProvider);
            var result = await Validator.ValidateAsync(context);
            AddValidationResult(EditContext.Model, result);
        }

        async Task ValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            ValidationMessageStore.Clear();
            var context = new ValidationContext<Object>(EditContext.Model);
            using var scope = ServiceScopeFactory.CreateScope();
            if (CaspianDataService != null)
            {
                var dataService = scope.GetService<CaspianDataService>();
                dataService.UserId = CaspianDataService.UserId;
                dataService.Language = CaspianDataService.Language;
            }
            Validator = (IValidator)Activator.CreateInstance(ValidatorType, scope.ServiceProvider);
            (Validator as ICaspianValidator).BatchServiceData = BatchServiceData;
            if (Source != null)
                (Validator as IBaseService).SetSource(Source);
            var asyncValidationTask = Validator.ValidateAsync(context);
            EditContext.Properties["AsyncValidationTask"] = asyncValidationTask;
            var result = await asyncValidationTask;

            AddValidationResult(EditContext.Model, result);
        }

        void AddValidationResult(object model, ValidationResult validationResult)
        {
            foreach (ValidationFailure error in validationResult.Errors)
            {
                var fieldIdentifier = new FieldIdentifier(model, error.PropertyName);
                ValidationMessageStore.Add(fieldIdentifier, error.ErrorMessage);
            }
            EditContext.NotifyValidationStateChanged();
        }

        private void EditContextChanged()
        {
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            HookUpEditContextEvents();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            //Keep a reference to the original values so we can check if they have changed
            var previousEditContext = EditContext;
            //MasterIdName = CaspianForm.MasterIdName;
            var previousValidatorType = ValidatorType;
            await base.SetParametersAsync(parameters);
            if (EditContext == null)
                throw new NullReferenceException($"{nameof(CaspianValidationValidator)} must be placed within an CaspianForm");
            if (ValidatorType == null)
                throw new NullReferenceException($"{nameof(ValidatorType)} must be specified.");
            if (!typeof(IValidator).IsAssignableFrom(ValidatorType))
                throw new ArgumentException($"{ValidatorType.Name} must implement {typeof(IValidator).FullName}");
            //if (ValidatorType != previousValidatorType)
            //    ValidatorTypeChanged();
            // If the EditForm.Model changes then we get a new EditContext
            // and need to hook it up
            if (EditContext != previousEditContext)
            {
                EditContextChanged();
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (CaspianForm != null)
            {
                CaspianForm.ValidationValidator = this;
                MasterIdName = CaspianForm.MasterIdName;
            }
            base.OnAfterRender(firstRender);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (FormAppState.ValidationChecking)
            {
                var control = CaspianForm.GetFirstInvalidControl();
                if (control != null)
                {
                    FormAppState.ValidationChecking = false;
                    await control.FocusAsync();
                }
                else if (FormAppState.ErrorMessage !=  null)    
                    await JSRuntime.InvokeVoidAsync("caspian.common.showMessage", FormAppState.ErrorMessage);
            }
            if (CaspianForm == null && FormAppState.AllControlsIsValid)
            {
                if (FormAppState.Control?.InputElement == null)
                {
                    if (FormAppState.ErrorMessage.HasValue())
                        await JSRuntime.InvokeVoidAsync("caspian.common.showMessage", FormAppState.ErrorMessage);
                }
                else
                    await FormAppState.Control.FocusAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
