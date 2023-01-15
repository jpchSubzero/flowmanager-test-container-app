using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.PreTrackings
{
    public class PreTrackingEfCoreConfiguration : IEntityTypeConfiguration<PreTracking>
    {
        public void Configure(EntityTypeBuilder<PreTracking> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{PreTrackingConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();

            builder.Property(q => q.TrackingId).IsRequired(false);
            builder.Property(q => q.TransactionReference).IsRequired().HasMaxLength(PreTrackingConsts.TransactionReferenceMaxLength);
            builder.Property(q => q.Identification).IsRequired().HasMaxLength(PreTrackingConsts.IdentificationMaxLength);
            builder.Property(q => q.FullName).IsRequired().HasMaxLength(PreTrackingConsts.FullNameMaxLength);
            builder.Property(q => q.CellPhone).IsRequired().HasMaxLength(PreTrackingConsts.CellPhoneMaxLength);
            builder.Property(q => q.Email).IsRequired().HasMaxLength(PreTrackingConsts.EmailMaxLength);

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.TransactionReference).IsUnique();

            builder.OwnsMany(b => b.PreTrackingSteps, a =>
            {
                a.ToTable(PreTrackingStepConsts.TableName, FlowManagerDbProperties.DbSchema);

                a.Property(e => e.PreTrackingId).IsRequired().ValueGeneratedNever();

                a.Property(e => e.Container).IsRequired().HasMaxLength(PreTrackingStepConsts.ContainerMaxLength);
                a.Property(e => e.Component).IsRequired().HasMaxLength(PreTrackingStepConsts.ComponentMaxLength);
                a.Property(e => e.Method).IsRequired().HasMaxLength(PreTrackingStepConsts.MethodMaxLength);
                a.Property(e => e.Body).IsRequired();
                a.Property(e => e.Iterations).IsRequired();
                a.Property(e => e.RegisterDate).IsRequired();
                a.Property(e => e.Observations).IsRequired(false).HasMaxLength(PreTrackingStepConsts.ObservationsMaxLength);

                a.HasKey(e => new { e.PreTrackingId, e.Container, e.Component, e.Method });
                a.HasIndex(e => new { e.PreTrackingId, e.Container, e.Component, e.Method }).IsUnique();
            });
        }
    }
}

