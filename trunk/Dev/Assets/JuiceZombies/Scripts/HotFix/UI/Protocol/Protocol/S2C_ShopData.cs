using MessagePack;
using System.Collections.Generic;
#if SERVER_BUILD
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
#endif


namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_ShopData : IMessagePack
    {
#if SERVER_BUILD
        [System.ComponentModel.DataAnnotations.Key]
#endif
        [IgnoreMember] public long Id { get; set; }

        [IgnoreMember] public long GameUserId { get; set; }

        /// <summary>
        /// 是否购买免广告卡
        /// </summary>
        [MessagePack.Key(0)]
        public bool isBuyADCard { get; set; }

        /// <summary>
        /// 是否购买月卡
        /// </summary>
        [MessagePack.Key(1)]
        public bool isBuyMonthCard { get; set; }

        /// <summary>
        /// 月卡剩余时间
        /// </summary>
        [MessagePack.Key(2)]
        public long buyedMonthCardms { get; set; }
    }
}