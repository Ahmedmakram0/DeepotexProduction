// Deepotex.EF/Repos/ProductRepo.cs
using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Deepotex.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deepotex.EF.Repos
{
    public class ProductRepo : BaseRepository<Product>, IProductRepo
    {
        private readonly ApplicationDbContext _context;

        public ProductRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Override GetAll to include Category
        public override List<Product> GetAll()
        {
            return _context.Set<Product>()
                .Include(p => p.Category)
                .ToList();
        }

        // Override GetById to include Category  
        public override Product GetById(int id)
        {
            var result = _context.Set<Product>()
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);
            return result;
        }

        public void Add(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _context.Set<Product>().Add(product);
            Save();
        }

        public List<Product> GetLatest()
        {
            var result = _context.Set<Product>()
                .Include(p => p.Category)
                .OrderByDescending(x => x.CreatedAt)
                .Take(3)
                .ToList();

            return result;
        }

        public void Update(int id, Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var entity = _context.Set<Product>()
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (entity == null)
            {
                throw new Exception("No data found");
            }

            // Update all properties including ImageUrls
            entity.Name = product.Name;
            entity.SmallDescription = product.SmallDescription;
            entity.Description = product.Description;
            entity.Features = product.Features;
            entity.Price = product.Price;
            entity.CategoryId = product.CategoryId;
            entity.ImageUrls = product.ImageUrls; // Update JSON-serialized list
            entity.UpdatedAt = DateTime.Now;

            _context.Entry(entity).State = EntityState.Modified;
            Save();
        }

        public List<Category> GetCategories()
        {
            var result = _context.Set<Category>().ToList();
            if (result == null || !result.Any())
            {
                throw new Exception("No data found");
            }
            return result;
        }
    }
}