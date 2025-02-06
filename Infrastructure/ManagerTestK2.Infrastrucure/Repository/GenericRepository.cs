using ManagerTestK2.Domain.Interfaces;
using ManagerTestK2.Domain.Pagination;
using ManagerTestK2.Infrastrucure.Context;
using ManagerTestK2.Infrastrucure.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTestK2.Infrastrucure.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public readonly ContextDb _context;
        private readonly DbSet<TEntity> _entities;

        public GenericRepository(ContextDb context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }


        public async Task<TEntity> Create(TEntity entity)
        {
            _entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<PagedList<TEntity>> GetAllPagination(int pageNumber, int pageSize)
        {
            var query = _entities.AsQueryable();
            return await PaginationHelper.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<TEntity> GetById(long id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<TEntity> Remove(TEntity entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
