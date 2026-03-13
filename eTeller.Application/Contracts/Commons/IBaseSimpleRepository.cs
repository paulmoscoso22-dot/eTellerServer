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

            /// <summary>
            /// Aggiunge una nuova entità al contesto per l'inserimento nel database.
            /// La persistenza avviene al prossimo <see cref="IUnitOfWork.Complete"/>.
            /// </summary>
            /// <param name="entity">Entità da inserire</param>
            /// <param name="cancellationToken">Token di cancellazione</param>
            Task AddAsync(T entity, CancellationToken cancellationToken = default);

            /// <summary>
            /// Aggiorna un'entità esistente nel contesto.
            /// La persistenza avviene al prossimo <see cref="IUnitOfWork.Complete"/>.
            /// </summary>
            /// <param name="entity">Entità da aggiornare</param>
            /// <param name="cancellationToken">Token di cancellazione</param>
            Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        }
    }
}
