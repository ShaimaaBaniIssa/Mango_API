using Mango.Service.ShoppingCartAPI.Models;

using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCart.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }

       
    }
}
