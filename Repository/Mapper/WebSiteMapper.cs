using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;

namespace Repository.Mapper
{
    public class WebSiteMapper : IEntityTypeConfiguration<WebSite>
    {
        void IEntityTypeConfiguration<WebSite>.Configure(EntityTypeBuilder<WebSite> builder)
        {
            builder.ToTable("WebSite");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.Name).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Url).HasColumnType("varchar(200)").HasMaxLength(200);
            builder.Property(u => u.CreateDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.State).HasDefaultValue(0);
            builder.Property(u => u.CreatorId).HasColumnType("varchar(64)").HasMaxLength(64);
        }
    }
}
