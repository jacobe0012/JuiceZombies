using MessagePack;


namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_ShopData 
    {
        /// <summary>
        /// 是否购买免广告卡
        /// </summary>
        [Key(0)]
        public bool IsBuyADCard { get; set; }

        /// <summary>
        /// 是否购买月卡
        /// </summary>
        [Key(1)]
        public bool IsBuyMonthCard { get; set; }

        /// <summary>
        /// 月卡剩余时间 ms
        /// </summary>
        [Key(2)]
        public int BuyedMonthCardms { get; set; }
    }
}