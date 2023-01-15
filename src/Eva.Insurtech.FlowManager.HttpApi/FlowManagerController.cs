using Eva.Insurtech.FlowManagers.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Eva.Insurtech.FlowManagers
{
    public abstract class FlowManagerController : AbpController
    {
        protected FlowManagerController()
        {
            LocalizationResource = typeof(FlowManagerResource);
        }
    }
}
