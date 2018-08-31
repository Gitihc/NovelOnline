using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;

namespace Repository.Mapper
{
    public class WebsiteNovelMapper : IEntityTypeConfiguration<WebsiteNovel>
    {
        void IEntityTypeConfiguration<WebsiteNovel>.Configure(EntityTypeBuilder<WebsiteNovel> builder)
        {
            builder.ToTable("WebsiteNovel");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.WebSiteId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.Name).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Author).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.OriginLink).HasColumnType("varchar(200)").HasMaxLength(200);
            builder.Property(u => u.CreateDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.State).HasDefaultValue(0);
        }
    }
}
