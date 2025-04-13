using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deepotex.EF.Repos;
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Delete(int id)
    {
        var entity = _context.Set<T>().Find(id);
        if (entity == null)
        {
            throw new Exception("No data found");
        }
        _context.Set<T>().Remove(entity);
        Save();
    }

    public List<T> GetAll()
    {
        var result = _context.Set<T>().ToList();
        if(result == null)
        {
            throw new Exception("No data found");
        }
        return result;
    }
    public T GetById(int id)
    {
        var result = _context.Set<T>().FirstOrDefault();
        if (result == null)
        {
            throw new Exception("No data found");
        }
        return result;

    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
