using Mango.Web.Models.CartDto;
using Mango.Web.Models.OrderDto;
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
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService , IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;   

        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
           
            
            return View(await LoadCartDtoBasedOnLoggedInUSer());
            
        }
        public async Task<IActionResult> Checkout()
        {


            return View(await LoadCartDtoBasedOnLoggedInUSer());

        }
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUSer();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.FirstName = cartDto.CartHeader.FirstName;
            cart.CartHeader.LastName = cartDto.CartHeader.LastName;
            cart.CartHeader.Email = cartDto.CartHeader.Email;

            ResponseDto? response =await _orderService.CreateOrder(cart);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            if(response!=null && response.IsSuccess)
            {
                //payment

            }
            
            return View();

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
