using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConvertOnesToQuota.Common
{
    public abstract class BaseRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDatabaseContext Context;
        protected readonly DbSet<TEntity> DbSet;

        protected BaseRepository(ApplicationDatabaseContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
        }

        public virtual TEntity GetItemOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual IQueryable<TEntity> GetItemAsQueryable(Expression<Func<TEntity, bool>> wherePredicate)
        {
            return DbSet.Where(wherePredicate).Take(1).AsQueryable();
        }

        public virtual IQueryable<TEntity> GetCollectionAsQueryable(Expression<Func<TEntity, bool>> wherePredicate)
        {
            return DbSet.Where(wherePredicate);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> wherePredicate, int limit = 0)
        {
            var query = DbSet.Where(wherePredicate);
            if (limit != 0)
            {
                query = query.Take(limit);
            }

            return query;
        }

        public virtual IEnumerable<TEntity> Get<TKey>(Expression<Func<TEntity, bool>> wherePredicate,
            Expression<Func<TEntity, TKey>> orderBy, int limit = 0)
        {
            var query = DbSet.Where(wherePredicate);
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            if (limit != 0)
            {
                query = query.Take(limit);
            }

            return query;
        }

        public virtual TEntity Find(int id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<TEntity> FindAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual IQueryable<TEntity> GetAllAsQueryable()
        {
            return DbSet.AsQueryable();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual bool Contains(TEntity entity)
        {
            return DbSet.Contains(entity);
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.Remove(entity);
            }
        }

        public virtual void AddOrUpdate(TEntity entity)
        {
            DbSet.AddOrUpdate(entity);
        }

        public virtual void AddOrUpdate(IEnumerable<TEntity> entities)
        {
            DbSet.AddOrUpdate(entities.ToArray());
        }

        public virtual void AddOrUpdate(Expression<Func<TEntity, object>> identifierExpression, TEntity entity)
        {
            DbSet.AddOrUpdate(identifierExpression, entity);
        }
    }
}
