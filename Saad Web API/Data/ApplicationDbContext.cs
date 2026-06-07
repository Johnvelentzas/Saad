using Microsoft.EntityFrameworkCore;
using Models.Attributes;
using Models.Management;
using Models.Production;

namespace Saad_Web_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Attributes.Models> Models { get; set; }
        public DbSet<Patterns> Patterns { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Processes> Processes { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<TaskDependencies> TaskDependencies { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<UserProcesses> UserProcesses { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Fabrics> Fabrics { get; set; }
        public DbSet<StitchTypes> StitchTypes { get; set; }
        public DbSet<YarnColors> YarnColors { get; set; }

    }
}
