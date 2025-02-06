using ManagerTestK2.Domain.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTestK2.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetById(long id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<PagedList<TEntity>> GetAllPagination(int pageNumber, int pageSize);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<TEntity> Remove(TEntity entity);
        void Save();
        void Dispose();

    }
}
