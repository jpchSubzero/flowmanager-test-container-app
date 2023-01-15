using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;
using System;

namespace Eva.Insurtech.FlowManagers.Products
{
    public static class ProductErrorCodes
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        private static string entity;

        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer)
        {
            _localizer = localizer;
            entity = _localizer.GetString(LabelConsts.PRODUCT);
        }

        public static Error GetErrorNotFoundById(string catalogType = "")
        {
            var catalog = string.Empty;
            if (!catalogType.IsNullOrEmpty())
            {
                catalog = $" ({_localizer.GetString(catalogType)})";
            }
            return new Error() { Code = ErrorConsts.ERROR_NOT_FOUND_BY_ID, Message = _localizer.GetString(ErrorConsts.ERROR_NOT_FOUND_BY_ID, $"{entity}{catalog}") };
        }

        public static Error GetErrorNotFoundChannelByCode(string code = "")
        {
            return new Error() { Code = ErrorConsts.ERROR_CHANNEL_NOT_FOUND_BY_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_CHANNEL_NOT_FOUND_BY_CODE, code) };
        }

        public static Error GetErrorNullCode(string code = "")
        {
            return new Error() { Code = ErrorConsts.ERROR_NULL_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_NULL_CODE, code) };
        }
    }
}

