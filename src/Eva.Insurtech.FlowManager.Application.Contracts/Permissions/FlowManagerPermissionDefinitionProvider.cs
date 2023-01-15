using Eva.Insurtech.FlowManagers.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Eva.Insurtech.FlowManagers.Permissions
{
    public class FlowManagerPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(FlowManagerPermissions.GroupName, L("Permission:FlowManager"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FlowManagerResource>(name);
        }
    }
}