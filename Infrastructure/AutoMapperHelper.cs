using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
namespace Infrastructure
{
    /// <summary>
    /// AutoMapper扩展帮助类
    /// </summary>
    public static class AutoMapperHelper
    {
        /// <summary>
        ///  类型映射
        /// </summary>
        public static T MapTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            var config = new MapperConfiguration(cfg => cfg.CreateMap(obj.GetType(), typeof(T)));
            var mapper = config.CreateMapper();
            return mapper.Map<T>(obj);
            //Mapper.CreateMap(obj.GetType(), typeof(T));
            //return Mapper.Map<T>(obj);
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            IMapper mapper = null;
            foreach (var first in source)
            {
                var type = first.GetType();
                var config = new MapperConfiguration(cfg => cfg.CreateMap(type, typeof(TDestination)));
                mapper = config.CreateMapper();
                //Mapper.CreateMap(type, typeof(TDestination));
                break;
            }
            return mapper.Map<List<TDestination>>(source);
            //return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            //IEnumerable<T> 类型需要创建元素的映射
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
            var mapper = config.CreateMapper();
            return mapper.Map<List<TDestination>>(source);
            //Mapper.CreateMap<TSource, TDestination>();
            //return Mapper.Map<List<TDestination>>(source);
        }
        /// <summary>
        /// 类型映射
        /// </summary>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
            var mapper = config.CreateMapper();
            return mapper.Map(source, destination);

            //Mapper.CreateMap<TSource, TDestination>();
            //return Mapper.Map(source, destination);
        }
        /// <summary>
        /// DataReader映射
        /// </summary>
        public static IEnumerable<T> DataReaderMapTo<T>(this IDataReader reader)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<IDataReader, IEnumerable<T>>());
            var mapper = config.CreateMapper();
            return mapper.Map<IDataReader, IEnumerable<T>>(reader);
            //Mapper.Reset();
            //Mapper.CreateMap<IDataReader, IEnumerable<T>>();
            //return Mapper.Map<IDataReader, IEnumerable<T>>(reader);
        }
    }
}
