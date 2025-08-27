using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

[Index(nameof(GameUserId), IsUnique = true)]
public class ShopData
{
    public uint Id { get; set; }

    public uint GameUserId { get; set; }

    /// <summary>
    /// 是否购买免广告卡
    /// </summary>

    public bool isBuyADCard { get; set; }

    /// <summary>
    /// 是否购买月卡
    /// </summary>
    public bool isBuyMonthCard { get; set; }

    /// <summary>
    /// 月卡剩余时间
    /// </summary>
    public int buyedMonthCardms { get; set; }
}