using System.Linq.Expressions;

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
        }
    }
}
