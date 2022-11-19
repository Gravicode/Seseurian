namespace Seseurian.Data
{
    public interface IBasicPersistenceProvider<T>
     : IDisposable
    {
        T GetById(string id);

        IList<T> GetByIds(ICollection<string> ids);

        IList<T> GetAll();

        T Store(T entity);

        void StoreAll(IEnumerable<T> entities);

        void Delete(T entity);

        void DeleteById(string id);

        void DeleteByIds(ICollection<string> ids);

        void DeleteAll();
    }
}
