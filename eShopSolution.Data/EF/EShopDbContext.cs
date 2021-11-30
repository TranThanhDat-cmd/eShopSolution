using Microsoft.EntityFrameworkCore;
using eShopSolution.Data.Entities;


namespace eShopSolution.Data.EF
{
    public class EShopDbContext : DbContext
    {
        public EShopDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category>  Categories  { get; set; }


    }
}
