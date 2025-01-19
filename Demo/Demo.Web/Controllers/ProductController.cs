using Caspian.Common.Service;
using Demo.Model;
using Demo.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IList<Product>> GetProducts()
        {
            var list = await service.GetAll().ToListAsync();
            foreach (var item in list) 
                item.Image = null;
            return list;
        }

        [HttpGet]
        public async Task<ActionResult> GetImage(int id)
        {
            var product = await service.SingleAsync(id);
            return base.File(product.Image, "image/png");
        }
    }
}
