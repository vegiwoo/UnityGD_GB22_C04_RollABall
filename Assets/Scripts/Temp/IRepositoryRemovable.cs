// ReSharper disable once CheckNamespace
namespace RollABall.Test
{
    public interface IRepositoryRemovable<T>
    {
        void Delete(object id);
        void Delete(T item);
        void DeleteAll();
    }
}