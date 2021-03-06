﻿
using Microsoft.EntityFrameworkCore;
using Repository.Core;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace Repository
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public class BaseRepository<T, TPrimaryKey> : IRepository<T, TPrimaryKey> where T : EntityBase<TPrimaryKey>
    {
        //定义数据访问上下文对象
        protected readonly HLDBContext _dbContext;

        /// <summary>
        /// 通过构造函数注入得到数据上下文对象实例
        /// </summary>
        /// <param name="dbContext"></param>
        public BaseRepository(HLDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            return _dbContext.Set<T>();
        }

        /// <summary>
        /// 根据lambda表达式条件获取实体集合
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public T FindById(TPrimaryKey id)
        {
            return _dbContext.Set<T>().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        /// 根据lambda表达式条件获取单个实体
        /// </summary>
        /// <param name="predicate">lambda表达式条件</param>
        /// <returns></returns>
        public T FindSingle(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        /// <returns></returns>
        public T Add(T entity, bool autoSave = true)
        {
            _dbContext.Set<T>().Add(entity);
            if (autoSave)
                Save();
            return entity;
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        /// <returns></returns>
        public List<T> AddRange(List<T> entitys, bool autoSave = true)
        {
            _dbContext.Set<T>().AddRange(entitys);
            if (autoSave)
                Save();
            return entitys;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public T Update(T entity, bool autoSave = true)
        {
            var obj = FindById(entity.Id);
            EntityToEntity(entity, obj);
            if (autoSave)
                Save();
            return entity;
        }

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="entity">The entity.</param>
        public void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            _dbContext.Set<T>().Where(where).Update(entity);
        }

        private void EntityToEntity(T pTargetObjSrc, T pTargetObjDest)
        {
            foreach (var mItem in typeof(T).GetProperties())
            {
                mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
            }
        }
        /// <summary>
        /// 新增或更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public T AddOrUpdate(T entity, bool autoSave = true)
        {
            if (FindById(entity.Id) != null)
                return Update(entity, autoSave);
            return Add(entity, autoSave);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public void Delete(T entity, bool autoSave = true)
        {
            _dbContext.Set<T>().Remove(entity);
            if (autoSave)
                Save();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="autoSave">是否立即执行保存</param>
        public void Delete(TPrimaryKey id, bool autoSave = true)
        {
            _dbContext.Set<T>().Remove(FindById(id));
            if (autoSave)
                Save();
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">lambda表达式</param>
        /// <param name="autoSave">是否自动保存</param>
        public void Delete(Expression<Func<T, bool>> where, bool autoSave = true)
        {
            _dbContext.Set<T>().Where(where).ToList().ForEach(it => _dbContext.Set<T>().Remove(it));
            if (autoSave)
                Save();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="startPage">页码</param>
        /// <param name="pageSize">单页数据数</param>
        /// <param name="rowCount">行数</param>
        /// <param name="where">条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public IQueryable<T> LoadPageList(int startPage, int pageSize, out int rowCount, Expression<Func<T, bool>> where = null, Expression<Func<T, object>> order = null)
        {
            var result = from p in _dbContext.Set<T>()
                         select p;
            if (where != null)
                result = result.Where(where);
            if (order != null)
                result = result.OrderBy(order);
            else
                result = result.OrderBy(m => m.Id);
            rowCount = result.Count();
            return result.Skip((startPage - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 事务性保存
        /// </summary>
        public void Save()
        {
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// 根据主键构建判断表达式
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        protected static Expression<Func<T, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(T));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<T, bool>>(lambdaBody, lambdaParam);
        }
        /// <summary>
        /// 返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql);
        }

        /// <summary>
        /// 返回第一个值
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
            var conn = _dbContext.Database.GetDbConnection();
            if (!conn.State.ToString().Equals("Open"))
            {
                conn.Open();
            }
            var command = conn.CreateCommand();
            string query = sql;
            command.CommandText = query;
            return command.ExecuteScalar();
        }

        public IQueryable<Chapter> ChapterQueryFromSql(string sql)
        {
            return _dbContext.ChapterQuery.FromSql(sql);
        }

        public IQueryable<WebsiteNovel> WebsiteNovelQueryFromSql(string sql)
        {
            return _dbContext.WebsiteNovelQuery.FromSql(sql);
        }
    }

    /// <summary>
    /// 主键为Guid类型的仓储基类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class BaseRepository<T> : BaseRepository<T, string>, IRepository<T> where T : EntityBase
    {
        public BaseRepository(HLDBContext dbContext) : base(dbContext)
        {

        }
    }
}
