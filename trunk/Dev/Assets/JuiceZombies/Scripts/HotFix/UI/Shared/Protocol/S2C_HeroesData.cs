using System.Collections.Generic;
using MessagePack;


namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_HeroesData
    {
        /// <summary>
        /// 是否购买免广告卡
        /// </summary>
        [Key(0)]
        public List<S2C_HeroData> Heroes { get; set; }
    }

    [MessagePackObject]
    public class S2C_HeroData
    {
        [Key(0)] public int ConfigId { get; set; }
        [Key(1)] public int Level { get; set; }
        [Key(2)] public int Quality { get; set; }
    }
}