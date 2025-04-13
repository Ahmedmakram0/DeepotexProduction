using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deepotex.core.Models;
public class Product
{
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string SmallDescription { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public required string Description { get; set; }
    
    [Required]
    public required List<string> Features { get; set; } = new();
    
    [Required]
    public decimal Price { get; set; }
    public int? CategoryId { get; set; } = 1;
    public Category? Category { get; set; }
    [Required]
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
