namespace JuiceZombies.Server.Datas;

public class ShopData
{
    public long Id { get; set; }

    public long GameUserId { get; set; }

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