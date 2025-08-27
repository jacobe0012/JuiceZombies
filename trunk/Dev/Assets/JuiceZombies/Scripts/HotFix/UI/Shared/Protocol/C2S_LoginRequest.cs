using HotFix_UI;
using MessagePack;

namespace HotFix_UI
{
    [MessagePackObject]
    public class C2S_LoginRequest
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        [Key(0)]
        public string Name { get; set; }

        // 机器码
        [Key(1)] public string Udid { get; set; }

        // 客户端类型 1 本机登录 name 不为空 2.快速登录 私钥不能为空 3 uid 不为空 游客登陆
        [Key(2)] public int Type { get; set; }

        // 手机类型 "|"
        [Key(3)] public string PhoneType { get; set; }
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