using HotFix_UI;
using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

public class MyPostgresDbContext : DbContext
{
    public MyPostgresDbContext(DbContextOptions<MyPostgresDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<S2C_ShopData> GameShops { get; set; }
    
    public DbSet<S2C_UserResData> GameUsers { get; set; }
}