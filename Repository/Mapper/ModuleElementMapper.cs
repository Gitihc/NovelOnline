using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;
namespace Repository.Mapper
{
    public class ModuleElementMapper : IEntityTypeConfiguration<ModuleElement>
    {
        void IEntityTypeConfiguration<ModuleElement>.Configure(EntityTypeBuilder<ModuleElement> builder)
        {
            builder.ToTable("Module");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.ModuleId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.DomId).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Name).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Type).HasDefaultValue(0);
            builder.Property(u => u.Icon).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Class).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Sort).HasDefaultValue(1);
            builder.Property(u => u.Remark).HasColumnType("text");
        }
    }
}
