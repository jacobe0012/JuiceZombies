using HotFix_UI;
using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

public class MyPostgresDbContext : DbContext
{
    public MyPostgresDbContext(DbContextOptions<MyPostgresDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 ItemData 抽象类及其子类的继承关系
        // 使用 HasDiscriminator 来告诉 EF Core 使用 TPH 模式
        modelBuilder.Entity<ItemData>()
            .HasDiscriminator<string>("Type") // "ItemType" 是鉴别器列的名称
            .HasValue<HeroItemData>("Hero") // CurrencyData 对应鉴别器值为 "Currency"
            .HasValue<BagItemData>("Bag"); // EquipmentData 对应鉴别器值为 "Equipment"
    }

    public DbSet<ShopData> ShopDatas { get; set; }

    public DbSet<UserResData> UserResDatas { get; set; }

    public DbSet<ItemData> ItemDatas { get; set; }

    public DbSet<GachaPityCounterData> GachaPityCounterDatas { get; set; }
}