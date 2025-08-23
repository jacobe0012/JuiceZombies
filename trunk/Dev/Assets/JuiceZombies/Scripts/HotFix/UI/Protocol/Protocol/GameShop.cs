using MessagePack;
using System.Collections.Generic;

namespace HotFix_UI
{
    [MessagePackObject]
    public class GameShop : IMessagePack
    {
        
        [Key(0)]
        public bool isBuyADCard { get; set; }
        [Key(1)]
        public bool isBuyMonthCard { get; set; }

        [Key(2)]
        public long buyedMonthCardms { get; set; }

    }
}