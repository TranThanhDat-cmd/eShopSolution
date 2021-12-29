using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModel.Catalog.Product.Manage
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }

        public string FilePath { get; set; }

        public bool IsDefault { get; set; }

        public long FileSize { get; set; }
    }
}
