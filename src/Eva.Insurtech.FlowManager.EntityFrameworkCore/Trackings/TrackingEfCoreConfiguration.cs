using Eva.Insurtech.FlowManagers.FailureLogs;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Trackings
{
    public class TrackingEfCoreConfiguration : IEntityTypeConfiguration<Tracking>
    {
        public void Configure(EntityTypeBuilder<Tracking> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{TrackingConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();

            builder.Property(q => q.FlowId).IsRequired();
            builder.Property(q => q.StepId).IsRequired();
            builder.Property(q => q.StateId).IsRequired();
            builder.Property(q => q.GeneralStateId).IsRequired();
            builder.Property(q => q.Start).IsRequired();
            builder.Property(q => q.ChangeState).IsRequired();
            builder.Property(q => q.ChannelCode).IsRequired();
            builder.Property(q => q.WayCode).IsRequired();
            builder.Property(q => q.IpClient).IsRequired();

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id).IsUnique();

            builder.OwnsMany(b => b.FailureLogs, a =>
            {
                a.ToTable(FailureLogConsts.TableName, FlowManagerDbProperties.DbSchema);

                a.Property(e => e.TrackingId).IsRequired().ValueGeneratedNever();
                a.Property(e => e.Method).IsRequired().HasMaxLength(FailureLogConsts.NameMaxLength);
                a.Property(e => e.Error).IsRequired().HasMaxLength(FailureLogConsts.DescriptionMaxLength);
                a.Property(e => e.Detail).IsRequired().HasMaxLength(FailureLogConsts.ExceptionMaxLength);
                a.Property(e => e.RegisterTime).IsRequired();
                a.Property(e => e.StateId).IsRequired();

                a.HasKey(e => new { e.TrackingId, e.RegisterTime });
                a.HasIndex(e => new { e.TrackingId, e.RegisterTime }).IsUnique();
            });

            builder.OwnsMany(b => b.ProcessLogs, a =>
            {
                a.ToTable(ProcessLogConsts.TableName, FlowManagerDbProperties.DbSchema);

                a.Property(e => e.TrackingId).IsRequired().ValueGeneratedNever();
                a.Property(e => e.RegisterTime).IsRequired();
                a.Property(e => e.StepId).IsRequired();
                a.Property(e => e.Request).IsRequired().HasMaxLength(ProcessLogConsts.RequestMaxLength);
                a.Property(e => e.Response).IsRequired().HasMaxLength(ProcessLogConsts.ResponseMaxLength);
                a.Property(e => e.Action).IsRequired(false).HasMaxLength(ProcessLogConsts.ActionMaxLength);
                a.Property(e => e.Version).IsRequired();

                a.HasKey(e => new { e.TrackingId, e.StepId, e.Version });
                a.HasIndex(e => new { e.TrackingId, e.StepId, e.Version }).IsUnique();
            });

            builder.OwnsMany(b => b.SubStepLogs, a =>
            {
                a.ToTable(SubStepLogConsts.TableName, FlowManagerDbProperties.DbSchema);

                a.Property(e => e.TrackingId).IsRequired().ValueGeneratedNever();
                a.Property(e => e.RegisterTime).IsRequired();
                a.Property(e => e.StepId).IsRequired();
                a.Property(e => e.SubStepCode).IsRequired().HasMaxLength(SubStepLogConsts.SubStepCodeMaxLength);
                a.Property(e => e.Attempts).IsRequired();

                a.HasKey(e => new { e.TrackingId, e.StepId, e.SubStepCode, e.Attempts });
                a.HasIndex(e => new { e.TrackingId, e.StepId, e.SubStepCode, e.Attempts }).IsUnique();
            });

        }
    }
}


