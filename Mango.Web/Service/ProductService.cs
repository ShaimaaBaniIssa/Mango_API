using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
        
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/product",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                
                Url = SD.ProductAPIBase + "/api/product/"+id,
            });
        }

        public Task<ResponseDto?> GetAllProductAsync()
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                
                Url = SD.ProductAPIBase + "/api/product",
            });
        }

        public Task<ResponseDto?> GetProductAsync(string productName)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                
                Url = SD.ProductAPIBase + "/api/product/GetByName/" + productName,
            });
        }

        public Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                
                Url = SD.ProductAPIBase + "/api/product/"+id,
            });
        }

        public Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/product",
                ContentType = SD.ContentType.MultipartFormData

            });
        }
    }
}
