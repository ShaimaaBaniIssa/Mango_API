using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private ResponseDto _response;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;


        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _response = new();
            _messageBus = messageBus;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {

            var ErrorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = ErrorMessage;
                return BadRequest(_response);

            }
            await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicsAndQueueNames:EmailRegisterUserQueue"));

            return Ok(_response);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "User Name or Password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {

            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error uncounted";
                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
        
    
}
