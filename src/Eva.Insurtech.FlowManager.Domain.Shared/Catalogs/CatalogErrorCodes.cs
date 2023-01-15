using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;
using System;

namespace Eva.Insurtech.FlowManagers.Catalogs
{
    public static class CatalogErrorCodes
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        private static string entity;

        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer)
        {
            _localizer = localizer;
            entity = _localizer.GetString(LabelConsts.CATALOG);
        }

        public static Error GetErrorGeneral()
        {
            return new Error() { Code = ErrorConsts.ERROR_GENERAL, Message = _localizer.GetString(ErrorConsts.ERROR_GENERAL) };
        }

        public static Error GetErrorNotFoundById(string catalogType = "")
        {
            var catalog = string.Empty;
            if (!catalogType.IsNullOrEmpty())
            {
                catalog = $" ({_localizer.GetString(catalogType)})";
            }
            return new Error() { Code = ErrorConsts.ERROR_NOT_FOUND_BY_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_NOT_FOUND_BY_CODE, $"{entity}{catalog}") };
        }
    }
}

