namespace Deepotex.core.ViewModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string SmallDescription { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public List<string> Features { get; set; } 

    [Required]
    public decimal Price { get; set; }

    public List<string> ImageUrls { get; set; } = new();

    // For backward compatibility with single image workflows
    public string ImageUrl => ImageUrls?.FirstOrDefault() ?? string.Empty;

    [Required]
    public List<IFormFile> Images { get; set; } = new();

    // For backward compatibility with single image workflows
    public IFormFile Image { get => Images?.FirstOrDefault(); set { if (value != null) { Images.Add(value); } } }
}
