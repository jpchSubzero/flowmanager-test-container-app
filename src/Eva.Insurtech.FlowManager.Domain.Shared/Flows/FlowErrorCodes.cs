using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public static class FlowErrorCodes
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        private static string entity;

        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer)
        {
            _localizer = localizer;
            entity = _localizer.GetString(LabelConsts.FLOW);
        }

        public static Error GetErrorGeneral()
        {
            return new Error() { Code = ErrorConsts.ERROR_GENERAL, Message = _localizer.GetString(ErrorConsts.ERROR_GENERAL) };
        }

        public static Error GetErrorContextAlreadyExists()
        {
            return new Error() { Code = ErrorConsts.ERROR_ALREADY_EXIST_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_ALREADY_EXIST_CODE, _localizer.GetString(LabelConsts.CONTEXT)) };
        }

        public static Error GetErrorAlreadyExists()
        {
            return new Error() { Code = ErrorConsts.ERROR_ALREADY_EXIST_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_ALREADY_EXIST_CODE, entity) };
        }

        public static Error GetErrorNotFoundById()
        {
            return new Error() { Code = ErrorConsts.ERROR_NOT_FOUND_BY_ID, Message = _localizer.GetString(ErrorConsts.ERROR_NOT_FOUND_BY_ID, entity) };
        }

        public static Error GetErrorNotFoundByCode()
        {
            return new Error() { Code = ErrorConsts.ERROR_NOT_FOUND_BY_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_NOT_FOUND_BY_CODE, entity) };
        }
    }
}
