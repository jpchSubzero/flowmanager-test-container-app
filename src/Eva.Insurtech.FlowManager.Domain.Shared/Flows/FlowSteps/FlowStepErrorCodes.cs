using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows.FlowSteps
{
    public static class FlowStepErrorCodes
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        private static string entity;

        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer)
        {
            _localizer = localizer;
            entity = _localizer.GetString(LabelConsts.FLOW_STEP);
        }
    }
}
