using System.Collections.Generic;
using MessagePack;


namespace HotFix_UI
{
    [MessagePackObject]
    public class C2S_GachaRequest
    {
        /// <summary>
        /// 盲盒id
        /// </summary>
        [Key(0)]
        public int BoxId { get; set; }

        /// <summary>
        /// 1：单抽 2：十连
        /// </summary>
        [Key(1)]
        public int Type { get; set; }
    }
}