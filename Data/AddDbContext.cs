
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using MinimalAPI.Models;

    namespace MinimalAPI.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }

            public DbSet<Coupon> Coupons => Set<Coupon>();
        }
    }

