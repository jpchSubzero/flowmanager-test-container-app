using Volo.Abp.Reflection;

namespace Eva.Insurtech.FlowManagers.Permissions
{
    public class FlowManagerPermissions
    {
        public const string GroupName = "FlowManager";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(FlowManagerPermissions));
        }
    }
}