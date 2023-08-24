using Microsoft.EntityFrameworkCore;
namespace Application_Cloning.DbContext_Data
{
    public class DemoDbContext:DbContext
    {
        public DemoDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<BranchDetails> BranchDetails { get; set; }

    }
}
