using HotFix_UI;
using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

public class MyPostgresDbContext : DbContext
{
    public MyPostgresDbContext(DbContextOptions<MyPostgresDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ShopData> ShopDatas { get; set; }
    
    public DbSet<UserResData> UserResDatas { get; set; }
}