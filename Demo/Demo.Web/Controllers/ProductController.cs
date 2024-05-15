using Caspian.Common.Service;
using Demo.Model;
using Demo.Service;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Web
{
    [ApiController]
    [Route("api/Product/[action]")]
    public class ProductController: Controller
    {
        ProductService service;
        public ProductController(IBaseService<Product> service)
        {
            this.service = service as ProductService;
        }

        [HttpGet]
        public async Task<ActionResult> GetImage(int id)
        {
            var product = await service.SingleAsync(id);
            return base.File(product.Image, "image/png");
        }
    }
}
