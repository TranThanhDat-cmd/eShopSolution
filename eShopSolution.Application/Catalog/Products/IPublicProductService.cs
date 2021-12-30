
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Catalog.Product.Public;
using eShopSolution.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest getProductByCategoryRequest);
        Task<List<ProductViewModel>> GetAll();

    }
}
