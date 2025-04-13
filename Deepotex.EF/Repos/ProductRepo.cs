using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Deepotex.EF.Repos;
using Deepotex.EF;
using Deepotex.core.ViewModels;

public class ProductRepo : BaseRepository<Product>, IProductRepo
{
    private new readonly ApplicationDbContext _context;

    public ProductRepo(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Add(Product product)
    {
        _context.Set<Product>().Add(product);
        Save();
    }

    public List<Product> GetLatest()
    {
        var result = _context.Set<Product>().OrderByDescending(x => x.CreatedAt).Take(3).ToList();
        if (result == null)
        {
            throw new Exception("No data found");
        }
        return result;
    }

    public void Update(int id ,Product product)
    {
        var entity = _context.Set<Product>().Find(id);
        if (entity == null)
        {
            throw new Exception("No data found");
        }
        var updatedProduct = new Product
        {
            Id = id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = entity.ImageUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = DateTime.Now,
            SmallDescription = product.SmallDescription,
            Features = product.Features,
        };
        _context.Entry(entity).CurrentValues.SetValues(updatedProduct);
        Save();
    }

    public List<Category> GetCategories()
    {
        var result = _context.Set<Category>().ToList();
        if (result == null)
        {
            throw new Exception("No data found");
        }
        return result;
    }
}
