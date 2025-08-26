using MessagePack;


namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_ShopData : IMessagePack
    {
        /// <summary>
        /// 是否购买免广告卡
        /// </summary>
        [Key(0)]
        public bool isBuyADCard { get; set; }

        /// <summary>
        /// 是否购买月卡
        /// </summary>
        [Key(1)]
        public bool isBuyMonthCard { get; set; }

        /// <summary>
        /// 月卡剩余时间
        /// </summary>
        [Key(2)]
        public int buyedMonthCardms { get; set; }
    }
}