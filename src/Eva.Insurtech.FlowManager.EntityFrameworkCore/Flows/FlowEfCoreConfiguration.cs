using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.Flows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Flows
{
    public class FlowEfCoreConfiguration : IEntityTypeConfiguration<Flow>
    {
        public void Configure(EntityTypeBuilder<Flow> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{FlowConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();

            builder.Property(q => q.Code).IsRequired().HasMaxLength(FlowConsts.CodeMaxLength);
            builder.Property(q => q.ChannelCode).IsRequired().HasMaxLength(FlowConsts.CodeMaxLength);
            builder.Property(q => q.Name).IsRequired().HasMaxLength(FlowConsts.NameMaxLength);
            builder.Property(q => q.Description).IsRequired().HasMaxLength(FlowConsts.DescriptionMaxLength);
            builder.Property(q => q.IsActive).IsRequired();
            builder.Property(q => q.ProductId).IsRequired();

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id).IsUnique();
            builder.HasIndex(q => q.Code).IsUnique();

            builder.OwnsMany(b => b.FlowSteps, a =>
            {
                a.ToTable(FlowStepConsts.TableName, FlowManagerDbProperties.DbSchema);
                a.Property(q => q.FlowId).IsRequired().ValueGeneratedNever();
                a.Property(q => q.StepId).IsRequired().ValueGeneratedNever();

                a.HasKey(e => new { e.FlowId, e.StepId });
                a.HasIndex(e => new { e.FlowId, e.StepId }).IsUnique();
            });
        }
    }
}

