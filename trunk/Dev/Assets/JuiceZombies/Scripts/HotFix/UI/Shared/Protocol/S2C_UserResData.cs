using HotFix_UI;
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
        /// 金币
        /// </summary>
        [Key(1)]
        public long Golds { get; set; }

        /// <summary>
        /// 钻石
        /// </summary>
        [Key(2)]
        public long Diamonds { get; set; }
        // // 修改呢称时间
        // int64 update_time_nickname = 4;
        // // 头像id
        // int32 role_avatar = 5;
        // // 头像框id
        // int32 role_avatar_frame = 6;
        // // 角色资产
        // GameRoleAssets role_assets = 7;
        // // 巡逻加权参数；
        // int32 patrol_gain_name = 8;
        // // 日常点数
        // int32 daily_point = 9;
        // // 周长点数
        // int32 weekly_point = 10;
        // // 通行证经验
        // int32 passport_exp = 11;
        // // 体力加权参数；
        // int32 energy_param = 12;
        // // 快速巡逻次数
        // int32 patrol_quick_count = 13;
        // // 免广告标识  0 不免  1 免
        // int32 ad_free_flag = 14;
        // // 月卡剩余天数
        // int32 month_card_remain_days = 15;
        // // 角色拥有头像
        // RoleAvatarMap avatar_map = 16;
        // // 最大章节id
        // int32 max_chapter_id = 17;
        // // 最大关卡id
        // int32 max_level_id = 18;
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