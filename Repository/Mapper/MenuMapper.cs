using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;
namespace Repository.Mapper
{
    public class MenuMapper : IEntityTypeConfiguration<Menu>
    {
        void IEntityTypeConfiguration<Menu>.Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menu");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.ParentId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.Name).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Type).HasDefaultValue(0);
            builder.Property(u => u.Link).HasColumnType("varchar(200)").HasMaxLength(200);
            builder.Property(u => u.IsEnable).HasDefaultValue(1);
            builder.Property(u => u.Sort).HasDefaultValue(1);
            builder.Property(u => u.CreateDate).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.CreatorId).HasColumnType("varchar(64)").HasMaxLength(64);
        }
    }
}
