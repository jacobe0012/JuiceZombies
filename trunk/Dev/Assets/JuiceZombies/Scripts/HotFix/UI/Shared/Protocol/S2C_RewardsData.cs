using System.Collections.Generic;
using MessagePack;


namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_RewardsData
    {
        /// <summary>
        /// 抽到的所有item id,count
        /// </summary>
        [Key(0)]
        public List<UnityEngine.Vector2> Result { get; set; }
    }
}