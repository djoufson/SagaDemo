using Microsoft.AspNetCore.Mvc;
using SagaDemo.OrderService.Persistence.Products;

namespace SagaDemo.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsRepository _productRepository;

    public ProductsController(IProductsRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        return Ok(await _productRepository.GetAllAsync());
    }
}
