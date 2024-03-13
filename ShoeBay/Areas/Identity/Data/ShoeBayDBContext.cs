using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoeBay.Areas.Identity.Data;

using ShoeBay.Data;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoeBay.Models;

namespace ShoeBay.Data;

public class ShoeBayDBContext : IdentityDbContext<ShoeBayUser>
{
    public ShoeBayDBContext(DbContextOptions<ShoeBayDBContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Shoe> Shoes { get; set; }
    public virtual DbSet<ShoeCart> ShoeOrders { get; set; }
    public virtual DbSet<OrderHistory> OrderHist { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
