using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;

namespace Repository.Mapper
{
   public class UserNovelMapper : IEntityTypeConfiguration<UserNovel>
    {
        void IEntityTypeConfiguration<UserNovel>.Configure(EntityTypeBuilder<UserNovel> builder)
        {
            builder.ToTable("Chapter");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.UserId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.NovelId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.LastOpenTime).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.State).HasDefaultValue(0);
            builder.Property(u => u.LastChapterId).HasColumnType("varchar(64)").HasMaxLength(64);
        }
    }
}
