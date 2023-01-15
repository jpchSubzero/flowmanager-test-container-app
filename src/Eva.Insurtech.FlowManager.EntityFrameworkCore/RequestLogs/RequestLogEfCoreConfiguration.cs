using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.RequestLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.RequestLogs
{
    public class RequestLogEfCoreConfiguration : IEntityTypeConfiguration<RequestLog>
    {
        public void Configure(EntityTypeBuilder<RequestLog> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{RequestLogConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();

            builder.Property(q => q.Service).IsRequired().HasMaxLength(RequestLogConsts.ServiceMaxLength);
            builder.Property(q => q.Iterations).IsRequired();
            builder.Property(q => q.RegisterDate).IsRequired();
            builder.Property(q => q.Observations).IsRequired(false).HasMaxLength(RequestLogConsts.ObservationsMaxLength);

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id).IsUnique();

            builder.OwnsMany(b => b.Requests, a =>
            {
                a.ToTable(RequestConsts.TableName, FlowManagerDbProperties.DbSchema);

                a.Property(e => e.RequestLogId).IsRequired().ValueGeneratedNever();

                a.Property(e => e.TransactionReference).IsRequired().HasMaxLength(RequestConsts.TransactionReferenceMaxLength);
                a.Property(e => e.Service).IsRequired().HasMaxLength(RequestConsts.ServiceMaxLength);
                a.Property(e => e.Body).IsRequired();
                a.Property(e => e.RegisterDate).IsRequired();
                a.Property(e => e.Observations).IsRequired(false).HasMaxLength(RequestConsts.ObservationsMaxLength);

                a.HasKey(e => new { e.TransactionReference, e.RequestLogId, e.Service });
                a.HasIndex(e => new { e.TransactionReference, e.RequestLogId, e.Service }).IsUnique();
            });
        }
    }
}
