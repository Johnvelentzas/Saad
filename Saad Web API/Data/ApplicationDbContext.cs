using Microsoft.EntityFrameworkCore;
using Saad_Web_API.Models.Entities.Finances;
using Saad_Web_API.Models.Entities.Production;

namespace Saad_Web_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Costumers> Costumers { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
    }
}
