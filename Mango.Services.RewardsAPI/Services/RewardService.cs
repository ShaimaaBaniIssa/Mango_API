
using Mango.Services.RewardsAPI.Data;
using Mango.Services.RewardsAPI.Message;
using Mango.Services.RewardsAPI.Models;
using Mango.Services.RewardsAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardsAPI.Services
{
    public class RewardService : IRewardService
    {
        // we cannot use application dbcontext using dependency injection because that is scoped implementation 
        // we need to register an implementation for reward service that is Singleton

        private DbContextOptions<ApplicationDbContext> _dbOptions;
        public RewardService(DbContextOptions<ApplicationDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task UpdateRewards(RewardMessage rewardMessage)
        {

            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardsActivity = rewardMessage.RewardsActivity,
                    RewardsDate = DateTime.Now,
                    UserId = rewardMessage.UserId,
                };
                
                await using var _db = new ApplicationDbContext(_dbOptions);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();

            }
            catch (Exception)
            {

            }
        

        }
    }
}
