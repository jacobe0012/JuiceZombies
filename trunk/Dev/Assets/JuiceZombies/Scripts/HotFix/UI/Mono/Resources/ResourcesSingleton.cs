using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using cfg.config;
using Common;
using Google.Protobuf.Collections;
using Main;
using UnityEngine;
using XFramework;



namespace HotFix_UI
{
    /// <summary>
    /// 前端缓存的所有数据单例
    /// </summary>
    public sealed class ResourcesSingleton : Singleton<ResourcesSingleton>, IDisposable
    {
        /// <summary>
        /// 当前缓存的玩家资产
        /// </summary>
        //public PlayerResources playerResources;

        /// <summary>
        /// 当前缓存的背包内物品
        /// </summary>
        public Dictionary<int, long> items = new Dictionary<int, long>();

        /// <summary>
        /// 当前缓存的角色所有装备
        /// </summary>
        public EquipmentData equipmentData;

        /// <summary>
        /// 当前缓存的小弟
        /// </summary>
        public Dictionary<int, long> littlebro = new Dictionary<int, long>();

        /// <summary>
        /// 当前缓存的配件
        /// </summary>
        public Dictionary<int, long> parts = new Dictionary<int, long>();

        /// <summary>
        /// 当前缓存的邮件
        /// </summary>
        public List<MailInfo> mails = new List<MailInfo>();

        /// <summary>
        /// 当前缓存的角色设置数据
        /// </summary>
        //public SettingsData settingsData;

        /// <summary>
        /// 当前缓存的角色设置数据 新 Unlock为解锁了的list  GuidList为未完成的新手引导
        /// </summary>
        public SettingDate settingData;

        /// <summary>
        /// 当前缓存的玩家属性
        /// </summary>
        public PlayerProperty playerProperty;

        /// <summary>
        /// 当前缓存的章节信息
        /// </summary>
        public LevelInfo levelInfo;

        /// <summary>
        /// 当前缓存的战局信息
        /// </summary>
        public BattleData battleData;

        public Talent talentID;

        /// <summary>
        /// 商店初始化信息
        /// </summary>
        public ShopInit shopInit;

        /// <summary>
        /// 名称ID
        /// </summary>
        public GameRole UserInfo;

        /// <summary>
        /// 日常周常信息 
        /// </summary>
        public RoleTaskInfo dayAndWeekInfo;

        /// <summary>
        /// 成就信息 
        /// </summary>
        public RoleTaskInfo achieveInfo;

        /// <summary>
        /// 成就分组依赖排序
        /// </summary>
        public Dictionary<int, List<int>> groupIDs;

        /// <summary>
        /// 签到信息
        /// </summary>
        public GameCheckIn signData;

        /// <summary>
        /// 分享
        /// </summary>
        public GameShare gameShare;

        /// <summary>
        /// 公告信息
        /// </summary>
        public List<GameNotice> noticeData;

        /// <summary>
        /// 挑战信息
        /// </summary>
        public ChallengeInfo challengeInfo;

        /// <summary>
        /// 怪物图鉴
        /// </summary>
        //public Dictionary<int, int> monsterCollection;

        /// <summary>
        /// 怪物图鉴
        /// </summary>
        public GameMonster resMonster;

        /// <summary>
        /// 商店map 根据tagfun的id拿到相关的商店数据表
        /// // 抽奖 BoxInfo
        /// // 充值 ChargeInfo
        /// // 礼包 GiftInfo
        /// // 每日购买 DailyBuy
        /// // 基金 GameFoundation
        /// // 特惠 GameSpecials
        /// // 月卡 specialCard
        /// </summary>
        public GameShopMap shopMap;

        /// <summary>
        /// 月卡是否购买
        /// </summary>
        public bool monthBuy;

        /// <summary>
        /// bank
        /// </summary>
        public GoldPig goldPig;

        /// <summary>
        /// 通行证信息
        /// </summary>
        public List<GamePass> gamePasses = new List<GamePass>();


        /// <summary>
        /// 通行证经验
        /// </summary>
        public int gamePassExp;

        /// <summary>
        /// 活动数据
        /// </summary>
        public Activity activity;

        /// <summary>
        /// 非活动红点信息
        /// </summary>
        public TaskRedFlag taskRedFlag;

        /// <summary>
        /// 当前所有系统包括穿戴上的装备给玩家的属性 生命攻击
        /// </summary>
        public Dictionary<int, int> mainProperty = new Dictionary<int, int>();

        /// <summary>
        /// 所有模块的初始红点
        /// </summary>
        public Dictionary<int, int> redDots = new Dictionary<int, int>();

        /// <summary>
        /// 首充
        ///0 ：初始化
        ///1 ：解锁成功
        ///2：充值成功 未解锁
        ///3： 解锁成功 充值成功
        ///4： 已领取
        /// </summary>
        public int firstChargeInt = 0;

        public bool haveSetBankWeb = false;

        public bool haveBuyBank = false;

        /// <summary>
        /// 服务器每天更新时间为UTC 00:00 + UpdateTime(second)
        /// </summary>
        public long updateTime = 0;

        /// <summary>
        /// 服务器与本地时间的差值(毫秒) ServerDeltaTime = severtime - localtime;
        /// </summary>
        public long serverDeltaTime = 0;

        /// <summary>
        /// 服务器时间 频繁更新此值 单位/ms
        /// </summary>
        public long serverTime = 0;

        /// <summary>
        /// 日常周常信息 废弃
        /// </summary>
        public TaskData dayWeekTask;

        /// <summary>
        /// 成就信息 废弃
        /// </summary>
        public TaskData achieve;
        //public PassTime passTime;

        /// <summary>
        /// 通行证是否开启
        /// </summary>
        //public bool gamePassStart = false;

        /// <summary>
        /// 通行证开启时间(s)
        /// </summary>
        //public long gamePassStartTime;

        /// <summary>
        /// 通行证关闭时间
        /// </summary>
        //public long gamePassEndTime;
        /// <summary>
        /// 是否从Runtime场景转出
        /// </summary>
        public bool FromRunTimeScene = false;

        /// <summary>
        /// 是否初始化完毕UI场景
        /// </summary>
        public bool isUIInit = false;

        public UIPosInfo UIPosInfo;

        /// <summary>
        /// 链接是否成功
        /// </summary>
        public bool isConnectSuccess;

        public void Init()
        {
            //TODO:

            DateTime now = DateTime.Now;
            long currentMilliseconds = ((DateTimeOffset)now).ToUnixTimeMilliseconds();
            serverTime = currentMilliseconds;

            talentID = new Talent
            {
                talentPropID = 0,
                talentSkillID = 0
            };
            shopInit = new ShopInit
            {
                shopHelpDic = new Dictionary<int, Dictionary<int, List<long>>>()
            };
            levelInfo = new LevelInfo
            {
                chapterID = 1,
                maxUnLockChapterSurviveTime = 0,
                maxUnLockChapterID = 0,
                levelId = 10001,
                seed = "",
                maxPassChapterID = 0,
                levelBox = new LevelBox
                {
                    boxStateDic = new Dictionary<int, bool>(),
                    minNotLockBoxID = -1,
                    minNotGetBoxID = -1,
                },
                maxMainBlockID = 0
            };
            InitSeed();

            UserInfo = new GameRole();
            settingData = new SettingDate
            {
                Quality = 0,
                EnableFx = true,
                EnableBgm = true,
                EnableShock = true,
                EnableWeakEffect = true,
                EnableShowStick = true,
                CurrentL10N = 2
            };
            equipmentData = new EquipmentData
            {
                equipments = new Dictionary<long, MyGameEquip>(),
                isWearingEquipments = new Dictionary<int, MyGameEquip>(),
                isMaterials = new Dictionary<Vector3, int>(),
                isCompoundSort = false
            };
            dayWeekTask = new TaskData
            {
                tasks = new Dictionary<int, Vector2>(),
                boxes = new Dictionary<int, Vector2>()
            };
            achieve = new TaskData
            {
                tasks = new Dictionary<int, Vector2>(),
                boxes = new Dictionary<int, Vector2>()
            };
            groupIDs = new Dictionary<int, List<int>>();
            signData = new GameCheckIn();
            noticeData = new List<GameNotice>();
            challengeInfo = new ChallengeInfo
            {
                maxAreaChallengeID = 0,
                maxMainChallengeID = 0,
                challengeStateMap = new Dictionary<int, int>()
            };
            resMonster = new GameMonster();
            shopMap = new GameShopMap();
            goldPig = new GoldPig();
            taskRedFlag = new TaskRedFlag();
            gameShare = new GameShare();
            gamePasses = new List<GamePass>();
            activity = new Activity
            {
                activityMap = new ActivityMap(),
                activityTaskDic = new Dictionary<int, List<GameTaskInfo>>(),
                ActivityFlag = new ActivityFlag()
            };
        }

        public string GenerateRandomString(string characters, int length)
        {
            char[] buffer = new char[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = characters[UnityEngine.Random.Range(0, characters.Length)];
            }

            return new string(buffer);
        }

        public void InitSeed()
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            int length = 10; // 指定要生成的随机字符串的长度
            UnityEngine.Random.InitState((int)Stopwatch.GetTimestamp());
            //Log.Error($"(int)DateTime.Now.Ticks {(int)Stopwatch.GetTimestamp()}");
            string randomString = GenerateRandomString(characters, length);

            int seed = randomString.GetHashCode();
            UnityEngine.Random.InitState(seed);

            //UnityEngine.Random.
            //uint newseed = (uint)DateTime.Now.Ticks;
            //uint newseed = (uint)Stopwatch.GetTimestamp() + (uint)DateTime.Now.Ticks;

            this.levelInfo.seed = randomString;
            this.levelInfo.seedHash = seed;
        }

        /// <summary>
        /// 更新主界面资源显示UI  能量 钻石 金币 用法：在修改玩资源数量后直接调用即可刷新主界面资源UI显示。
        /// </summary>
        public void UpdateResourceUI()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out UI ui))
            {
                var uiJiyuGame = ui as UIPanel_JiyuGame;
                uiJiyuGame?.SetResourceUI();
            }
        }

        public void Clear()
        {
            shopInit.Dispose();
            noticeData.Clear();
            gamePasses.Clear();
            groupIDs.Clear();
            dayWeekTask.Dispose();
            achieve.Dispose();
            equipmentData.Dispose();
            items.Clear();
            mainProperty.Clear();
            littlebro.Clear();
            parts.Clear();
            resMonster.MonsterMap.Clear();
            resMonster = null;
            goldPig = null;
            taskRedFlag = null;
            shopMap = null;
            settingData = null;
            UserInfo = null;
            gameShare = null;
            redDots.Clear();
        }

        public void Dispose()
        {
            Clear();
            Instance = null;
        }
    }

    public struct PassTime
    {
        /// <summary>
        /// 通行证id
        /// </summary>
        public int id;

        /// <summary>
        /// 通行证开启时间(s)
        /// </summary>
        public long gamePassStartTime;

        /// <summary>
        /// 通行证关闭时间
        /// </summary>
        public long gamePassEndTime;
    }

    /// <summary>
    /// 战局相关信息
    /// </summary>
    public struct BattleData
    {
        public long battleId;
    }

    public enum GraphicQuality
    {
        HD,
        Normal,
        Flow
    }

    public struct SettingsData
    {
        //
        public GraphicQuality quality;
        public bool enableFx;
        public bool enableBgm;
        public bool enableShock;
        public bool enableWeakEffect;
        public bool enableShowStick;
        public Version version;
        public bool isShared;
        public Tblanguage.L10N currentL10N;
    }

    /// <summary>
    /// 如果reward有值 则为展示tips 如果equipDto有值则为装备详情tips
    /// </summary>
    public class MyGameEquip
    {
        public Vector3 reward;
        public EquipDto equip;
        public bool isWearing;
        public bool canCompound;
    }

    public struct Activity : IDisposable
    {
        /// <summary>
        /// 活动数据
        /// </summary>
        public ActivityMap activityMap;

        /// <summary>
        /// 活动任务数据
        /// </summary>
        public Dictionary<int, List<GameTaskInfo>> activityTaskDic;

        /// <summary>
        /// 活动红点 key：activityId  value:有无红点
        /// </summary>
        public ActivityFlag ActivityFlag;

        public void Dispose()
        {
            activityMap = null;
            foreach (var VARIABLE in activityTaskDic)
            {
                VARIABLE.Value.Clear();
            }

            ActivityFlag.ActivityFlagMap.Clear();
            activityTaskDic.Clear();
            activityTaskDic = null;
        }
    }

    public struct EquipmentData : IDisposable
    {
        /// <summary>
        /// 当前缓存的所有排过序的装备 key:Uid 
        /// </summary>
        //public List<long> sortedEquips;

        /// <summary>
        /// 当前缓存的所有装备 key:Uid 
        /// </summary>
        public Dictionary<long, MyGameEquip> equipments;

        /// <summary>
        /// 当前缓存的所有装备 key:posid 
        /// </summary>
        public Dictionary<int, MyGameEquip> isWearingEquipments;

        /// <summary>
        /// 当前缓存的通用合成材料 reward串 数量
        /// </summary>
        public Dictionary<Vector3, int> isMaterials;

        public bool isCompoundSort;

        public void Dispose()
        {
            //sortedEquips.Clear();
            equipments.Clear();
            isWearingEquipments.Clear();
            isMaterials.Clear();
        }
    }


    public struct PlayerResources
    {
        /// <summary>
        /// 当前缓存的能量数
        /// </summary>
        public long energy;

        /// <summary>
        /// 当前缓存的能量上限
        /// </summary>
        public long maxEnergy;

        /// <summary>
        /// 当前缓存的宝石(充值货币)
        /// </summary>
        public long gems;

        /// <summary>
        /// 当前缓存的金币(游戏币)
        /// </summary>
        public long gold;

        /// <summary>
        /// 当前缓存的玩家等级
        /// </summary>
        public int level;

        /// <summary>
        /// 当前缓存的玩家总经验
        /// </summary>
        public long playerTotalExp;
    }

    public struct PlayerProperty
    {
        public ChaProperty chaProperty;
        public PlayerData playerData;
    }

    public struct LevelBox
    {
        public Dictionary<int, bool> boxStateDic;
        public int minNotLockBoxID;
        public int minNotGetBoxID;
    }

    public struct UIPosInfo
    {
        public Vector3 KBtn_DiamondPos;
        public Vector3 KBtn_MoneyPos;
        public Vector3 KBagPos;
    }

    /// <summary>
    ///章节信息
    /// </summary>
    public struct LevelInfo
    {
        /// <summary>
        /// 当前所选择的章节id
        /// </summary>
        public int chapterID;

        /// <summary>
        /// 最小未通关关卡的生存时间
        /// </summary>
        public int maxUnLockChapterSurviveTime;

        public LevelBox levelBox;

        /// <summary>
        /// 最大已解锁街区id
        /// </summary>
        public int maxMainBlockID;

        /// <summary>
        /// 最大通关章节id
        /// </summary>
        public int maxPassChapterID;

        /// <summary>
        /// 最大已解锁章节id
        /// </summary>
        public int maxUnLockChapterID;

        /// <summary>
        /// 当前选择的缓存关卡id
        /// </summary>
        public int levelId;

        /// <summary>
        /// 当前缓存的种子数
        /// </summary>
        public string seed;

        /// <summary>
        /// 当前缓存的种子数 hash
        /// </summary>
        public int seedHash;

        /// <summary>
        /// 当前关卡剩余复活次数
        /// </summary>
        public int rebirthNum;

        /// <summary>
        /// 当前关卡剩余广告复活次数
        /// </summary>
        public int adRebirthNum;
    }

    public struct Talent
    {
        /// <summary>
        /// 当前解锁的最大天赋id
        /// </summary>
        public int talentPropID;

        /// <summary>
        /// 当前解锁的最大天赋技能id
        /// </summary>
        public int talentSkillID;
    }


    public struct ChallengeInfo
    {
        /// <summary>
        /// 最大已解锁主线挑战关卡
        /// </summary>
        public int maxMainChallengeID;

        /// <summary>
        /// 最大已解锁区域挑战关卡
        /// </summary>
        public int maxAreaChallengeID;

        /// <summary>
        /// 挑战id---state
        /// </summary>
        public Dictionary<int, int> challengeStateMap;
    }


    /// <summary>
    /// 商店初始化数据
    /// </summary>
    public struct ShopInit : IDisposable
    {
        /// <summary>
        /// gameShop的dictionary
        /// </summary>
        public Dictionary<int, Dictionary<int, List<long>>> shopHelpDic;


        public void Dispose()
        {
            foreach (var shop in shopHelpDic)
            {
                foreach (var shop0 in shop.Value)
                {
                    shop0.Value.Clear();
                }

                shop.Value.Clear();
            }

            shopHelpDic.Clear();
        }
    }

    public struct TaskData : IDisposable
    {
        public Dictionary<int, Vector2> tasks;
        public Dictionary<int, Vector2> boxes;

        public void Dispose()
        {
            tasks.Clear();
            boxes.Clear();
        }
    }
}