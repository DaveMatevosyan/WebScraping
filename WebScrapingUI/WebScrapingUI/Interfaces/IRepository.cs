using System.Collections.Generic;

namespace WebScrapingUI.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();
        T Get(int id);
        //IEnumerable<T> Find(Func<T, Boolean> predicate);
        T Create(T item);
        //void Update(T item);
        void Delete(int id);
    }
}
