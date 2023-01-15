using Eva.Insurtech.FlowManagers.PreTrackings;
using FluentValidation;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eva.Insurtech.FlowManagers
{
    public static class CustomFluentValidations
    {
        public static IRuleBuilderOptions<T, Guid> IsGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder, string message)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return Guid.TryParse(value.ToString(), out Guid guid);
            })
            .WithMessage("The '{PropertyName}: '" + message);
        }

        public static IRuleBuilderOptions<T, string> IsCode<T>(this IRuleBuilder<T, string> ruleBuilder, string message, int lenght)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return !string.IsNullOrEmpty(value) && value.Length <= lenght;
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsName<T>(this IRuleBuilder<T, string> ruleBuilder, string message, int lenght)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return !string.IsNullOrEmpty(value) && value.Length <= lenght;
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsRequiredDescription<T>(this IRuleBuilder<T, string> ruleBuilder, string message, int lenght)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return !string.IsNullOrEmpty(value) && value.Length <= lenght;
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsIpV4<T>(this IRuleBuilder<T, string> ruleBuilder, string message)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                if (value.IsNullOrEmpty())
                {
                    return false;
                }

                string[] splitValues = value.Split('.');
                if (splitValues.Length != 4)
                {
                    return false;
                }


                return splitValues.All(r => byte.TryParse(r, out byte tempForParsing));
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsRequiredString<T>(this IRuleBuilder<T, string> ruleBuilder, string message, int lenght)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return !string.IsNullOrEmpty(value) && value.Length <= lenght;
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsRequiredStringMaxSize<T>(this IRuleBuilder<T, string> ruleBuilder, string message)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return !string.IsNullOrEmpty(value);
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsNotRequiredString<T>(this IRuleBuilder<T, string> ruleBuilder, string message, int lenght)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                if (value.IsNullOrEmpty())
                {
                    return true;
                }
                return !string.IsNullOrEmpty(value) && value.Length <= lenght;
            })
            .WithMessage("'{PropertyName}': " + message);
        }

        public static IRuleBuilderOptions<T, string> IsIdentification<T>(this IRuleBuilder<T, string> ruleBuilder, string message)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return ValidateIsIdentification(value);
            })
            .WithMessage("'{PropertyName}: '" + message);
        }

        public static IRuleBuilderOptions<T, string> IsPhone<T>(this IRuleBuilder<T, string> ruleBuilder, string message)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return ValidateIsPhone(value);
            })
            .WithMessage("'{PropertyName}: '" + message);
        }

        public static IRuleBuilderOptions<T, string> IsEmail<T>(this IRuleBuilder<T, string> ruleBuilder, string message)
        {
            return ruleBuilder.Must((rootObject, value, context) =>
            {
                return ValidateIsEmail(value);
            })
            .WithMessage("'{PropertyName}': " + message);
        }


        #region Private Methods

        private static bool ValidateIsIdentification(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length >= PreTrackingConsts.IdentificationMinLength && value.Length <= PreTrackingConsts.IdentificationMaxLength;
        }

        private static bool ValidateIsEmail(string value)
        {
            if (Regex.IsMatch(value, TrackingConsts.EmailPattern))
            {
                return true;
            }
            return false;
        }

        private static bool ValidateIsPhone(string value)
        {
            try
            {
                _ = double.Parse(value);
            }
            catch (Exception)
            {
                return false;
            }

            return !string.IsNullOrEmpty(value) && value.Length >= PreTrackingConsts.CellPhoneMinLength && value.Length <= PreTrackingConsts.CellPhoneMaxLength;
        }

        #endregion


    }
}
