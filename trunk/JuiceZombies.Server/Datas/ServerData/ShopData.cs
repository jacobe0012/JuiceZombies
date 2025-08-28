using Microsoft.EntityFrameworkCore;

namespace JuiceZombies.Server.Datas;

[Index(nameof(UserId), IsUnique = true)]
public class ShopData
{
    public uint Id { get; set; }

    public uint UserId { get; set; }

    /// <summary>
    /// 是否购买免广告卡
    /// </summary>

    public bool IsBuyADCard { get; set; }

    /// <summary>
    /// 是否购买月卡
    /// </summary>
    public bool IsBuyMonthCard { get; set; }

    /// <summary>
    /// 月卡剩余时间
    /// </summary>
    public int BuyedMonthCardms { get; set; }
}