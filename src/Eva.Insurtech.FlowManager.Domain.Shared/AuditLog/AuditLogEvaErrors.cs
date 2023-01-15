using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditLogEvaErrors
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer) { _localizer = localizer; }

        public static Error GetErrorGeneral()
        {
            return new Error()
            {
                Code = AuditLogEvaConsts.ERROR_AUDIT,
                Message = _localizer.GetString(AuditLogEvaConsts.ERROR_AUDIT)
            };
        }

        public static Error GetErrorGeneral(string code, string message = null, string details = null)
        {
            return new Error()
            {
                Code = code,
                Message = (message ?? _localizer.GetString(code)),
                Details = details
            };
        }
    }
}
