using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;

namespace Repository.Mapper
{
  public  class NovelMapper : IEntityTypeConfiguration<Novel>
    {
        void IEntityTypeConfiguration<Novel>.Configure(EntityTypeBuilder<Novel> builder)
        {
            builder.ToTable("Novel");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.Name).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.SourceUrl).HasColumnType("varchar(200)").HasMaxLength(200);
            builder.Property(u => u.State).HasDefaultValue(0);
            builder.Property(u => u.Type).HasDefaultValue(0);
            builder.Property(u => u.CreateDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        }
    }
}
