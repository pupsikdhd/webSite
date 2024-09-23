using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace tutorialAsp.NetCore.Data.Context
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}