using eShopSolution.Application.Catalog.Products;
using eShopSolution.Data.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublicProductService _publicProductService;

        public ProductController(IPublicProductService publicProductService)
        {
            _publicProductService = publicProductService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _publicProductService.GetAll();
            return Ok(data);
        }
    }
}
