using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deepotex.core.ViewModels;
public class HomeViewModel
{
    public int ProductID { get; set; }
    [Required]
    public string ProductName { get; set; }
    [Required]
    public decimal ProductPrice { get; set; }
    [Required]
    public string ProductDescription { get; set; }

}
