using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class HomeVM
    {
        IEnumerable<Product> Products { get; set; }
        IEnumerable<Category> Categories { get; set; }
    }
}
