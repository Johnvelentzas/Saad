using Microsoft.EntityFrameworkCore;
using Models.Finances;
using Models.Attributes;
using Models.Production;

namespace Saad_Web_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<FabricPatterns> FabricPatterns { get; set; }
        public DbSet<Fabrics> Fabrics { get; set; }
        public DbSet<Models.Attributes.Models> Models { get; set; }
        public DbSet<PatternAreas> PatternAreas { get; set; }
        public DbSet<Patterns> Patterns { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<SewingTypes> SewingTypes { get; set; }
        public DbSet<TaskAtributes> TaskAtributes { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Processes> Processes { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<TaskDependencies> TaskDependencies { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<UserProcesses> UserProcesses { get; set; }
        public DbSet<Users> Users { get; set; }

    }
}
