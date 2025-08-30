using MessagePack;

namespace HotFix_UI
{
    [MessagePackObject]
    public class S2C_UserResData
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        [Key(0)]
        public string? UserName { get; set; }

        /// <summary>
        /// 用户所有资源
        /// </summary>
        [Key(1)]
        public System.Collections.Generic.List<S2C_ItemData> Items { get; set; }
    }
}