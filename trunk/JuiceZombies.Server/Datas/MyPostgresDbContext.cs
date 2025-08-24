using HotFix_UI;
using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

public class MyPostgresDbContext : DbContext
{
    public MyPostgresDbContext(DbContextOptions<MyPostgresDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<GameShop> GameShops { get; set; }
    
    public DbSet<GameUser> GameUsers { get; set; }
}