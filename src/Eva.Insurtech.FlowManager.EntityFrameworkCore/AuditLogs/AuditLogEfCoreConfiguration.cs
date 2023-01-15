using Eva.Insurtech.FlowManagers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditLogEfCoreConfiguration : IEntityTypeConfiguration<AuditLogEva>
    {
        public void Configure(EntityTypeBuilder<AuditLogEva> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{AuditLogEvaConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();
            builder.Property(q => q.TrackingId).IsRequired().HasMaxLength(AuditLogEvaConsts.CodeMaxLength);
            builder.Property(q => q.Action).IsRequired().HasMaxLength(AuditLogEvaConsts.DescriptionMaxLength);
            builder.Property(q => q.DateTimeExecution).IsRequired();
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id).IsUnique();
            builder.HasIndex(q => q.TrackingId);            
        }
    }
}

