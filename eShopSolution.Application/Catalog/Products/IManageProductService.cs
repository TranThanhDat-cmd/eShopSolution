
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Catalog.Product.Manage;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest productCreateRequest);
        Task<int> Update(ProductUpdateRequest productUpdateRequest);
        Task<int> Delete(int id);


        Task<bool> UpdatePrice(int productId, decimal newPrice);
        Task<bool> UpdateStock(int productId, int addedQuantity);
        Task AddViewCount(int productId);

        Task<PageResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest getProductPagingRequest);

        Task<int> AddImages(int productId, List<IFormFile> files);

        Task<int> RemoveImages(int imageId);

        Task<int> UpdateImage(int imageId, string caption, bool isDefault);

        Task<List<ProductImageViewModel>> GetListImage(int productId);


    }
}
