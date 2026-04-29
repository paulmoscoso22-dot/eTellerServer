using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace eTeller.Application.Contracts.Commons
{
    public interface IBaseSimpleRepository
    {
        public interface IBaseSimpleRepository<T>
        {
            Task<IReadOnlyList<T>> GetAllAsync();

            Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

            Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                           string includeString = null,
                                           bool disableTracking = true);

            Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                           List<Expression<Func<T, object>>> includes = null,
                                           bool disableTracking = true);


            Task<T> GetByIdAsync(int id);
            Task<T> GetByIdAsync(string id);

            Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

            Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

            Task DeleteAsync(T entity);


            void AddEntity(T entity);

            void AddRangeEntity(IEnumerable<T> entities);

            void DeleteRangeEntity(IEnumerable<T> entities);

            void UpdateEntity(T entity);

            void DeleteEntity(T entity);

        }
    }
}
