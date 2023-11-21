using Mango.Web.Models.CartDto;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
           
            
            return View(await LoadCartDtoBasedOnLoggedInUSer());
            
        }
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUSer()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
            CartDto cart = new();

            if (response != null && response.IsSuccess)
            {
                cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            return cart;
        }
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "cart updated successfully";
                return RedirectToAction("CartIndex");
            }

            return View();
           
        }
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUSer();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.EmailCart(cart);


            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction("CartIndex");
            }

            return View();

        }
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);


            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "cart updated successfully";
                return RedirectToAction("CartIndex");
            }

            return View();

        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);


            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "cart updated successfuly";
                 return RedirectToAction("CartIndex");

            }

            return View();
        }
    }
}
