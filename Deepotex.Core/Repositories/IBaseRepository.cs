

namespace Deepotex.core.Repositories;
public interface IBaseRepository<T> where T : class
{
    List<T> GetAll();
    T GetById(int id);
    void Delete(int id);
    void Save();

}
