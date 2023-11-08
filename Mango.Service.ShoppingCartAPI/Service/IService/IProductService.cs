using Mango.Service.ShoppingCartAPI.Models.Dto;

namespace Mango.Service.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
