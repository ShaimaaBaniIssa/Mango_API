using Mango.Services.EmailAPI.Models.Dto.CartDto;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
