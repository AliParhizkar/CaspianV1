using System;
using FluentValidation;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Caspian.Common;

namespace Caspian.UI
{
    public class CaspianValidationValidator: ComponentBase, IControlFocuseValidation
    {
        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [CascadingParameter]
        private EditContext EditContext { get; set; }

        [CascadingParameter(Name = "ParentForm")]
        private ICaspianForm CaspianForm { get; set; }

        [Parameter]
        public Type ValidatorType { get; set; }

        [Parameter]
        public bool OnlyValidateOnSubmit { get; set; } = true;

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
            var context = new ValidationContext<object>(EditContext.Model, new PropertyChain(), new RulesetValidatorSelector("default"));
            var result = await Validator.ValidateAsync(context);
            AddValidationResult(EditContext.Model, result);
        }

        public bool IgnoreDetailsProperty { get; set; }

        async Task ValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            ValidationMessageStore.Clear();
            var context = new ValidationContext<Object>(EditContext.Model);
            using var scope = ServiceScopeFactory.CreateScope();
            Validator = (IValidator)Activator.CreateInstance(ValidatorType, scope);
            (Validator as ICaspianValidator).IgnoreDetailsProperty = IgnoreDetailsProperty;
            context.RootContextData["__ServiceScopeFactory"] = ServiceScopeFactory;
            var result = Validator.Validate(context);
            AddValidationResult(EditContext.Model, result);
        }

        void AddValidationResult(object model, ValidationResult validationResult)
        {
            foreach (ValidationFailure error in validationResult.Errors)
            {
                var propertyName = error.PropertyName;
                if (propertyName != MasterIdName && !propertyName.EndsWith('.' + MasterIdName))
                {
                    var fieldIdentifier = new FieldIdentifier(model, propertyName);
                    ValidationMessageStore.Add(fieldIdentifier, error.ErrorMessage);
                }
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
    }
}
