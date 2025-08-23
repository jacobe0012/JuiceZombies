using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

public class MyPostDbContext : DbContext
{
    public MyPostDbContext(DbContextOptions<MyPostDbContext> options)
        : base(options)
    {
    }

    // 在这里添加你的 DbSet 属性，比如：
    // public DbSet<Player> Players { get; set; }
    // public DbSet<Item> Items { get; set; }
}