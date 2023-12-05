using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto.CartDto;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task EmailRegisterUser(string email);
        Task LogOrderPlaced(RewardMessage rewardMessage);

    }
}
