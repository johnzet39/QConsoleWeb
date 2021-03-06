﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QConsoleWeb.Infrastructure.Attributes
{
    public class IpAddressAttribute : RegularExpressionAttribute, IClientModelValidator
    {
        public IpAddressAttribute()
            : base(@"^([\d]{1,3}\.){3}[\d]{1,3}$")
        { }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            if (!base.IsValid(value))
                return false;

            string ipValue = value as string;
            if (IsIpAddressValid(ipValue))
                return true;

            return false;
        }

        private bool IsIpAddressValid(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            string[] values = ipAddress.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            byte ipByteValue;
            foreach (string token in values)
            {
                if (!byte.TryParse(token, out ipByteValue))
                    return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("The '{0}' field has an invalid format.", name);
        }


        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-ipformat",
                "Ip-адрес имеет неверный формат");
        }
    }
}
