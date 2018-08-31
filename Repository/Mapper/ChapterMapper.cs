//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Repository.Domain;


//namespace Repository.Mapper
//{
// public   class ChapterMapper : IEntityTypeConfiguration<Chapter>
//    {
//        void IEntityTypeConfiguration<Chapter>.Configure(EntityTypeBuilder<Chapter> builder)
//        {
//            builder.ToTable("Chapter");
//            builder.HasKey(u => u.Id);
//            builder.Property(u => u.Id).HasColumnType("varchar(64)").HasMaxLength(64);
//            builder.Property(u => u.NovelId).HasColumnType("varchar(64)").HasMaxLength(64);
//            builder.Property(u => u.NovelName).HasColumnType("varchar(100)").HasMaxLength(100);
//            builder.Property(u => u.Name).HasColumnType("varchar(100)").HasMaxLength(100);
//            builder.Property(u => u.OriginLink).HasColumnType("varchar(200)").HasMaxLength(200);
//            builder.Property(u => u.Sort).HasDefaultValue(1);
//            builder.Property(u => u.State).HasDefaultValue(0);
//            builder.Property(u => u.ChapterStartPosition).HasDefaultValue(0);
//            builder.Property(u => u.ChapterEndPosition).HasDefaultValue(0);
//        }
//    }
//}
