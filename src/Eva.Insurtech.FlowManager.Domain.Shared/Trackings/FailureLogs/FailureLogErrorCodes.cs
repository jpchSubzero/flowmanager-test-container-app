using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows.FailureLogs
{
    public static class FailureLogErrorCodes
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        private static string entity;

        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer)
        {
            _localizer = localizer;
            entity = _localizer.GetString(LabelConsts.FAILURE_LOG);
        }
    }
}
