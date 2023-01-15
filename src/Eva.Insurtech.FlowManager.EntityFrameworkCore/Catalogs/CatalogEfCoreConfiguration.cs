using Eva.Insurtech.FlowManagers.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Catalogs
{
    public class CatalogEfCoreConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{CatalogConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();

            builder.Property(q => q.CatalogId).IsRequired();
            builder.Property(q => q.Code).IsRequired().HasMaxLength(CatalogConsts.CodeMaxLength);
            builder.Property(q => q.Name).IsRequired().HasMaxLength(CatalogConsts.NameMaxLength);

            builder.HasKey(e => e.Id);
            builder.HasKey(e => e.Code);

            builder.HasIndex(e => e.Id).IsUnique();
            builder.HasIndex(e => new { e.Code, e.CatalogId }).IsUnique();
        }
    }
}
