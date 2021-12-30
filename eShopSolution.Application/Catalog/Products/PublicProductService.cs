
using eShopSolution.Data.EF;
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Catalog.Product.Public;
using eShopSolution.ViewModel.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _context;

        public PublicProductService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductViewModel>> GetAll()
        {
            var query = from p in _context.Products
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        select new { p, pt, pic };
            var data = await query.Select(x => new ProductViewModel()
            {
                Id = x.p.Id,
                Name = x.pt.Name,
                DateCreated = x.p.DateCreated,
                Description = x.pt.Description,
                Details = x.pt.Details,
                LanguageId = x.pt.LanguageId,
                OriginalPrice = x.p.OriginalPrice,
                Price = x.p.Price,
                SeoAlias = x.pt.SeoAlias,
                SeoDescription = x.pt.SeoDescription,
                SeoTitle = x.pt.SeoTitle,
                Stock = x.p.Stock,
                ViewCount = x.p.ViewCount,
            }).ToListAsync<ProductViewModel>();

            return data;
        }

        public async Task<PageResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest req)
        {
            var query = from p in _context.Products
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        select new { p, pt, pic };



            if (req.CategoryId.HasValue && req.CategoryId > 0)
            {
                query = query.Where(x => x.pic.CategoryId == req.CategoryId);
            }


            var totalRecord = await query.CountAsync();


            var data = await query.Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize).Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                }).ToListAsync<ProductViewModel>();


            var pageResult = new PageResult<ProductViewModel>()
            {
                Items = data,
                TotalRecord = totalRecord,
            };

            return pageResult;

        }
    }
}
