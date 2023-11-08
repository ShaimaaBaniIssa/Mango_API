using Mango.Service.ShoppingCartAPI.Models.Dto;

namespace Mango.Service.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponAsync(string couponCode);
    }
}
