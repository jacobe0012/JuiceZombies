using HotFix_UI;
using MessagePack;

namespace HotFix_UI
{
    [MessagePackObject]
    public class GameUser : IMessagePack
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Key(0)]
        public long Id { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Key(1)]
        public string? UserName { get; set; }
    }

    // [MessagePackObject]
    // public struct OtherData
    // {
    //     [Key(0)] public string Code { get; set; }
    //
    //     /// <summary>
    //     /// 开发者账号id
    //     /// </summary>
    //     [Key(1)]
    //     public string UnionidId { get; set; }
    // }
    //
    // [MessagePackObject]
    // public struct LocationData
    // {
    //     [Key(0)] public string Addr { get; set; }
    //
    //     [Key(1)] public string Country { get; set; }
    //
    //     [Key(2)] public string Province { get; set; }
    //
    //     [Key(3)] public string City { get; set; }
    // }
}