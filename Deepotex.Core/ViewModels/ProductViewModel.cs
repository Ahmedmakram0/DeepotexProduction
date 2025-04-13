namespace Deepotex.core.ViewModels;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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

    [Required]
    public IFormFile Image { get; set; } 
}
