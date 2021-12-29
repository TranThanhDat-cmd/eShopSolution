using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities;
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Catalog.Product.Manage;
using eShopSolution.ViewModel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;
        public ManageProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<int> AddImages(int productId, List<IFormFile> files)
        {
            foreach (IFormFile item in files)
            {
                var productImage = new ProductImage()
                {
                    ProductId = productId,
                    Caption = "Thumbnail image",
                    DateCreated = DateTime.Now,
                    FileSize = item.Length,
                    ImagePath = await this.SaveFile(item),
                    IsDefault = false,
                    SortOrder = await _context.ProductImages.Where(x => x.ProductId == productId).CountAsync() + 1,
                };

                await _context.ProductImages.AddAsync(productImage);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest req)
        {
            Product product = new Product()
            {
                Price = req.Price,
                OriginalPrice = req.OriginalPrice,
                Stock = req.Stock,
                ViewCount = 0,
                DateCreated = DateTime.UtcNow,

                ProductTranslations = new List<ProductTranslation>() {
                    new ProductTranslation() {
                        Name = req.Name,
                        Description = req.Description,
                        Details =req.Details,
                        SeoAlias = req.SeoAlias,
                        SeoDescription = req.SeoDescription,
                        SeoTitle = req.SeoTitle,
                        LanguageId = req.LanguageId
                    }

                }
            };

            //Save image

            if (req.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = req.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(req.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }

            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Cannot find a product: {productId}");

            var images = _context.ProductImages.Where(i => i.ProductId == productId);
            foreach (var image in images)
            {
                await _storageService.DeleteFileAsync(image.ImagePath);
            }

            _context.Products.Remove(product);

            return await _context.SaveChangesAsync();


        }

        public async Task<PageResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest req)
        {

            //  1.select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            // 2.filter
            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(req.Keyword));

            }
            if (req.CategoryIds.Count > 0)
            {
                query = query.Where(x => req.CategoryIds.Contains(x.pic.CategoryId));
            }

            // 3.Orcerby

            // 4.Paging and convert ProductVM

            int totalRecord = await query.CountAsync();
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

        public async Task<List<ProductImageViewModel>> GetListImage(int productId)
        {
            return await _context.ProductImages.Where(x => x.ProductId == productId)
                 .Select(x => new ProductImageViewModel()
                 {
                     FilePath = x.ImagePath,
                     FileSize = x.FileSize,
                     Id = x.Id,
                     IsDefault = x.IsDefault

                 }).ToListAsync();

        }

        public async Task<int> RemoveImages(int imageId)
        {
            var image = _context.ProductImages.SingleOrDefault(x => x.Id == imageId);
            if (image == null) { throw new EShopException($"Can't fint a image with id : {imageId}"); }
            _context.Remove(image);
            return await _context.SaveChangesAsync();

        }

        public async Task<int> Update(ProductUpdateRequest req)
        {
            var product = await _context.Products.FindAsync(req.Id);
            var productTranslation = await _context.ProductTranslations
                .SingleOrDefaultAsync(x => req.Id == x.Id && req.LanguageId == x.LanguageId);
            if (product == null || productTranslation == null)
            {
                throw new EShopException($"Can't find a product with id : {req.Id}");
            }

            productTranslation.SeoAlias = req.SeoAlias;
            productTranslation.SeoTitle = req.SeoTitle;
            productTranslation.SeoDescription = req.SeoDescription;
            productTranslation.Details = req.Details;
            productTranslation.Name = req.Name;
            productTranslation.Description = req.Description;

            //save image

            if (req.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == req.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = req.ThumbnailImage.Length;
                    thumbnailImage.ImagePath = await this.SaveFile(req.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int imageId, string caption, bool isDefault)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.Id == imageId);
            if (image == null) { throw new EShopException($"Can't fint a image with id : {imageId}"); }

            image.Caption = caption;
            image.IsDefault = isDefault;
            return await _context.SaveChangesAsync();

        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == productId);
            if (product == null)
            {
                throw new EShopException($"Can't find a product with id : {productId}");

            }

            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.SingleOrDefaultAsync(x => x.Id == productId);
            if (product == null)
            {
                throw new EShopException($"Can't find a product with id : {productId}");

            }

            product.Stock += addedQuantity;
            return await _context.SaveChangesAsync() > 0;




        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
