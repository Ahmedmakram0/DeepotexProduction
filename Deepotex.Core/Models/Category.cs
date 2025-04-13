using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deepotex.core.Models;
public class Category
{
    public int Id { get; set; }
    [Required,MaxLength(150)]
    public string Name { get; set; }

    [Required,MaxLength(500)]
    public string Description { get; set; }
    public List<Product> Products { get; set; }
}
