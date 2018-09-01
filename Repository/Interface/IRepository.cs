
using Repository.Core;
using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Interface
{

    /// <summary>
    /// 定义泛型仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T, TPrimaryKey> where T : EntityBase<TPrimaryKey>
    {
        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        T FindById(TPrimaryKey id);

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        T FindSingle(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        /// <returns></returns>
        T Add(T entity, bool autoSave = true);

        List<T> AddRange(List<T> entitys, bool autoSave = true);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        T Update(T entity, bool autoSave = true);

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="entity">The entity.</param>
        void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity);

        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        T AddOrUpdate(T entity, bool autoSave = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        void Delete(T entity, bool autoSave = true);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="autoSave">是否立即执行保存</param>
        void Delete(TPrimaryKey id, bool autoSave = true);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        void Delete(Expression<Func<T, bool>> where, bool autoSave = true);

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面条目</param>
        /// <param name="rowCount">数据总数</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        IQueryable<T> LoadPageList(int startPage, int pageSize, out int rowCount, Expression<Func<T, bool>> where, Expression<Func<T, object>> order);

        void Save();

        int ExecuteSql(string sql);

        object ExecuteScalar(string sql);

        IQueryable<Chapter> ChapterQueryFromSql(string sql);
    }

    /// <summary>
    /// 默认Guid主键类型仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<T> : IRepository<T, string> where T : EntityBase
    {

    }

}
