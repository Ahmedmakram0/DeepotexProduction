using Deepotex.core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Deepotex.EF;
public  class ApplicationDbContext:DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>()
            .HasData(
                new Category
                {
                    Id = 1,
                    Name = "Sealants",
                    Description = "Sealants are specialized substances designed to create a secure and durable bond between surfaces, preventing leaks, moisture intrusion, and air passage. They are commonly used in industrial, automotive, and construction applications to seal joints, flanges, and gaps. These products offer excellent resistance to extreme temperatures, chemicals, and mechanical stress, ensuring long-lasting performance in demanding environments.\r\n\r\n",
                }
            );
        modelBuilder.Entity<Product>()
                .Property(p => p.Features)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        modelBuilder.Entity<Product>()
                .Property(p => p.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>());
    }
}
