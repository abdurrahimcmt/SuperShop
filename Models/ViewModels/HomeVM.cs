using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> products { get; set; }

        public IEnumerable <Category> Categories { get; set; }
    }
}
