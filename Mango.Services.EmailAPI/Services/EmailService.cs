using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto.CartDto;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        // we cannot use application dbcontext using dependency injection because that is scoped implementation 
        // we need to register an implementation for email service that is Singleton

        private DbContextOptions<ApplicationDbContext> _dbOptions;

        public EmailService(DbContextOptions<ApplicationDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        
        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach(var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count) ;
                message.Append("</li>");

            }
            message.Append("</ul>");
            await LogAndEmail(message.ToString(),cartDto.CartHeader.Email);


        }
        public async Task EmailRegisterUser(string email)
        {
            await LogAndEmail("User Registration Successful. <br/> Email:  "+ email, "domain@gmail.com");

        }

        public async Task LogOrderPlaced(RewardMessage rewardMessage)
        {
            string message = "New Order Placed. </br> Order ID: " + rewardMessage.OrderId;
            await LogAndEmail(message, "domain@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message , string email)
        {
            try
            {
                EmailLogger emailLogger = new EmailLogger()
                {
                    Email = email,
                    Message = message,
                    EmailSent = DateTime.Now
                };
                await using var _db = new ApplicationDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}
