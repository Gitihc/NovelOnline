using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repository.Domain;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Repository
{
    public class HLDBContext : DbContext
    {
        public HLDBContext(DbContextOptions<HLDBContext> options) : base(options)
        {
        }

        public DbQuery<Chapter> ChapterQuery { get; set; }
        public DbQuery<WebsiteNovel> WebsiteNovelQuery { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //builder.Entity<User>().ToTable("User");


            //var a = Assembly.GetExecutingAssembly().GetTypes().Where(type => !string.IsNullOrEmpty(type.Namespace));

            //var a1 = GetType().GetTypeInfo().Assembly.GetTypes().Where(type => !string.IsNullOrEmpty(type.Namespace) && type.GetInterfaces().Any(y=>y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            //var b = a.Where(type => type.BaseType != null && type.BaseType.IsGenericType);
            //var c = a.Where(type => type.GetInterfaces().Any(y => y.BaseType != null && y.BaseType.IsGenericType && y.BaseType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            //var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(type => !String.IsNullOrEmpty(type.Namespace))
            //    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes().Where(type => !String.IsNullOrEmpty(type.Namespace))
               .Where(type => type.GetInterfaces().Any( y =>  y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
            modelBuilder.SetSimpleUnderscoreTableNameConvention();  //禁用表复数

            base.OnModelCreating(modelBuilder);
            #region "添加表关系"
            //var mappingInterface = typeof(IEntityTypeConfiguration<>);
            ////// Types that do entity mapping
            //var mappingTypes = GetType().GetTypeInfo().Assembly.GetTypes()
            //.Where(x => x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));

            ////// Get the generic Entity method of the ModelBuilder type
            //var entityMethod = typeof(ModelBuilder).GetMethods()
            //                .Single(x => x.Name == "Entity" &&
            //                x.IsGenericMethod &&
            //                x.ReturnType.Name == "EntityTypeBuilder`1");

            //foreach (var mappingType in mappingTypes)
            //{

            //    // Get the type of entity to be mapped
            //    var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

            //    // Get the method builder.Entity<TEntity>
            //    var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);

            //    // Invoke builder.Entity<TEntity> to get a builder for the entity to be mapped
            //    var entityBuilder = genericEntityMethod.Invoke(builder, null);

            //    // Create the mapping type and do the mapping
            //    var mapper = Activator.CreateInstance(mappingType);
            //    mapper.GetType().GetMethod("Configure").Invoke(mapper, new[] { entityBuilder });
            //}
            //foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}
            #endregion
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void SetSimpleUnderscoreTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                Regex underscoreRegex = new Regex(@"((?<=.)[A-Z][a-zA-Z]*)|((?<=[a-zA-Z])\d+)");
                entity.Relational().TableName = underscoreRegex.Replace(entity.DisplayName(), @"$1$2").ToLower();
            }
        }
    }
}
