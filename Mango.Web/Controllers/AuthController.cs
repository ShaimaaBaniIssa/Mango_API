using Mango.Web.Models.AuthDto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService , ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;

        }
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem{Text=SD.RoleCustomer ,Value=SD.RoleCustomer},
                new SelectListItem{Text=SD.RoleAdmin ,Value=SD.RoleAdmin},

            };
            ViewBag.RoleList = roleList;
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequest)
        {

            ResponseDto response = await _authService.RegisterAsync(registrationRequest);
            ResponseDto assignRole;
            if (response != null && response.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequest.Role))
                {
                    registrationRequest.Role = SD.RoleCustomer;
                }

                assignRole = await _authService.AssignRoleAsync(registrationRequest);
                if (assignRole != null && assignRole.IsSuccess)
                {
                   
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }


            }
            else
            {
                TempData["error"] = response.Message;

            }


            var roleList = new List<SelectListItem>
            {
                new SelectListItem{Text=SD.RoleCustomer ,Value=SD.RoleCustomer},
                new SelectListItem{Text=SD.RoleAdmin ,Value=SD.RoleAdmin},

            };
            ViewBag.RoleList = roleList;
            return View(registrationRequest);

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {

            ResponseDto response = await _authService.LoginAsync(loginRequestDto);

            if (response != null && response.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                await SignInUser(loginResponseDto);
                _tokenProvider.SetTokent(loginResponseDto.Token); // set token in cookies
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                return View(loginRequestDto );

            }
        }
        public async  Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.CleanToken();
            return RedirectToAction("Index", "Home");

        }
        private async Task SignInUser(LoginResponseDto loginResponseDto) {
            //signing user using .net identity

            
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponseDto.Token); // decrypt the token

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            // extract claims 
            //add claim types to identity
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        
        }
    }
}
