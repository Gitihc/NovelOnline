using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Repository.Domain;

namespace Repository.Mapper
{
    public class RelevanceMapper : IEntityTypeConfiguration<Relevance>
    {
        void IEntityTypeConfiguration<Relevance>.Configure(EntityTypeBuilder<Relevance> builder)
        {
            builder.ToTable("Relevance");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.Description).HasColumnType("text");
            builder.Property(u => u.Key).HasColumnType("varchar(100)").HasMaxLength(100);
            builder.Property(u => u.Status).HasDefaultValue(0);
            builder.Property(u => u.OperateTime).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.OperatorId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.FirstId).HasColumnType("varchar(64)").HasMaxLength(64);
            builder.Property(u => u.SecondId).HasColumnType("varchar(64)").HasMaxLength(64);
        }
    }
}
