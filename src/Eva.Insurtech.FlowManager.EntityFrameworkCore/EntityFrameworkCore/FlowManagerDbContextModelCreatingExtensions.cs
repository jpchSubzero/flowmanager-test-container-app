using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore
{
    public static class FlowManagerDbContextModelCreatingExtensions
    {
        public static void ConfigureFlowManager(
            this ModelBuilder builder,
            Action<FlowManagerModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FlowManagerModelBuilderConfigurationOptions(
                FlowManagerDbProperties.DbTablePrefix,
                FlowManagerDbProperties.DbSchema
            );

            optionsAction?.Invoke(options);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.ConfigurePermissionManagement();
            builder.ConfigureSettingManagement();
            builder.ConfigureAuditLogging();
            builder.ConfigureFeatureManagement();
            builder.ConfigureTenantManagement();
        }
    }
}