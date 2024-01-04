using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {

                ResponseDto? response = await _productService.CreateProductAsync(productDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Created Successfully";

                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDto);
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {

            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                ProductDto? product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(product);

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(ProductDto productDto)
        {

            ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product Deleted Successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(productDto);

        }
        public async Task<IActionResult> EditProduct(int id)
        {

            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                ProductDto? product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(product);

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductAsync(productDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDto);

        }
    }
}
