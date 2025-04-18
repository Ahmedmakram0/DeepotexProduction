using Deepotex.core.Models;
using Deepotex.core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deepotex.core.Repositories;
public interface IProductRepo : IBaseRepository<Product>
{
     List<Product>  GetLatest();
    void Add(Product product);
    void Update(int id, Product product);
    List<Category> GetCategories();
    
}
