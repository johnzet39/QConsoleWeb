using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Infrastructure.Attributes
{
    public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
    {
        private String PropertyName { get; set; }
        private Object DesiredValue { get; set; }
        private readonly RequiredAttribute _innerAttribute;

        public RequiredIfAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
            _innerAttribute = new RequiredAttribute();
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (context.ObjectInstance.GetType().GetProperty(PropertyName) != null)
            {
                var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);

                if (dependentValue.ToString() == DesiredValue.ToString())
                {
                    if (!_innerAttribute.IsValid(value))
                    {
                        return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                    }
                }
            }
            
            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-requiredif",
                ErrorMessageResourceName ?? "Введите пароль");
        }
    }
}
