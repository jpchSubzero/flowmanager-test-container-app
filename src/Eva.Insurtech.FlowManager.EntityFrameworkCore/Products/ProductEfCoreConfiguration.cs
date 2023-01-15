using Eva.Insurtech.FlowManagers.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Products
{
    public class ProductEfCoreConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable($"{FlowManagerDbProperties.DbTablePrefix}{ProductConsts.TableName}", FlowManagerDbProperties.DbSchema);

            builder.ConfigureExtraProperties();

            builder.Property(q => q.ProductId).IsRequired();
            builder.Property(q => q.Code).IsRequired().HasMaxLength(ProductConsts.CodeMaxLength);
            builder.Property(q => q.ExternalCode).IsRequired().HasMaxLength(ProductConsts.CodeMaxLength);
            builder.Property(q => q.Name).IsRequired().HasMaxLength(ProductConsts.NameMaxLength);

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Id).IsUnique();
            builder.HasIndex(e => new { e.Code, e.ProductId, e.ExternalCode }).IsUnique();
        }
    }
}
