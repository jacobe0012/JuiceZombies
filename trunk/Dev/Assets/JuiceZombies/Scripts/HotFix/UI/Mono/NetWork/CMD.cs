namespace HotFix_UI
{
    public  static class CMD
    {
        //0,1   登录
        public const int LOGIN = (MainCmd.loginCmd << 16) + LoginCmd.loginVerify;

        //0, 2 切换账号
        public const int SWITCHACCOUNT = (MainCmd.loginCmd << 16) + LoginCmd.switchAccount;

        //1,1 
        public const int INITPLAYER = (MainCmd.playerCmd << 16) + PlayerCmd.initCacheRole;

        //10,1  查询背包      
        public const int OPENBAG = (MainCmd.bagCmd << 16) + BagCmd.findItem;

        //10,5 自选宝箱
        public const int SELFCHOOSEBOX = (MainCmd.bagCmd << 16) + BagCmd.selfChooseBox;

        //1,6
        public const int CHANGENAME = (MainCmd.playerCmd << 16) + PlayerCmd.rename;

        //1,5
        public const int CHANGESTATUS = (MainCmd.playerCmd << 16) + PlayerCmd.checkStatus;

        //1,8 查询当前关卡
        public const int QUERYLEVEL = (MainCmd.playerCmd << 16) + PlayerCmd.queryLevel;

        //2, 1 战斗结算
        public const int BATTLEGAIN = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.battleGain;

        //2,3 请求战局id
        //public const int REQUESTBATTLEID = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.requestBattleId;

        //2,5 验证对局是否可以开始
        public const int QUERYCANSTART = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.queryCanStart;

        //2,4 查询章节信息 精简
        public const int CHAPTERINFO = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.chapterInfo;

        //2,7 查询角色属性             
        public const int QUERYPROPERTY = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.playerProperty;

        //2,10查询解锁天赋
        public const int QUERYTALENT = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.queryTalent;

        //2,11 解锁天赋
        public const int LOCKTALENT = (MainCmd.combatPowerCmd << 16) + TalentCmd.lockTatant;

        //2,12 
        public const int CHALLENGECLAIM = (MainCmd.combatPowerCmd << 16) + ChallengCmd.challengeClaim;

        //2,13
        public const int QUERYCHALLENGE = (MainCmd.combatPowerCmd << 16) + ChallengCmd.challengeQuery;

        //2,14 消耗复活币
        public const int CONSUMEREBIRTHCOIN = (MainCmd.combatPowerCmd << 16) + ChallengCmd.consumeRebirthCoin;

        //2,15 设置战斗信息
        public const int SETBATTLEINFO = (MainCmd.combatPowerCmd << 16) + ChallengCmd.setBattleInfo;

        //2,16 获得扫荡奖励
        public const int GETSWEEPREWARD = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.getSweepReward;

        //2,17 获得主属性
        public const int GETMAINPROPERTY = (MainCmd.combatPowerCmd << 16) + CombatPowerCmd.getMainProperty;

        //5,1  查询邮件
        public const int QUERYMAIL = (MainCmd.mailSystemCmd << 16) + MailCmd.queryMail;

        //6,3
        public const int QUERYAUTOPATROL = (MainCmd.dropSystemCmd << 16) + PatrolCmd.queryAutoPatrol;

        //9,5查询所有装备
        public const int QUERYEQUIP = (MainCmd.equipCmd << 16) + EquipCmd.findEquip;

        //9,11查询所有已穿戴装备
        public const int QUERYWEAR = (MainCmd.equipCmd << 16) + EquipCmd.allwearEquip;

        //9,3升级 gameequip  gameequip
        public const int EQUIPUPGRADE = (MainCmd.equipCmd << 16) + EquipCmd.upgrade;

        //9,7一键升级 gameequip  gameequip
        public const int EQUIPALLUPGRADE = (MainCmd.equipCmd << 16) + EquipCmd.allUpgrade;

        //9,10穿戴  UID gameequip(废弃)
        //public const int EQUIPWEAR = (MainCmd.equipCmd << 16) + EquipCmd.wearEquip;

        //9,12卸下 UID gameequip(废弃)
        //public const int EQUIPUNWEAR = (MainCmd.equipCmd << 16) + EquipCmd.unwearEquip;

        //9,13穿卸换 
        public const int EQUIPWEARORUNWEAR = (MainCmd.equipCmd << 16) + EquipCmd.wearunwearEquip;

        //9,2降级 gameequip  gameequip
        public const int EQUIPDOWNGRADE = (MainCmd.equipCmd << 16) + EquipCmd.downgrade;

        //9,4降品 gameequip  ALLgameequip
        public const int EQUIPDOWNQUALITY = (MainCmd.equipCmd << 16) + EquipCmd.downQuality;

        //9,6合成  gameequipLIST  ALLgameequip
        public const int EQUIPCOMPOSE = (MainCmd.equipCmd << 16) + EquipCmd.compose;

        //9,1  一键合成    ALLgameequip
        public const int EQUIPALLCOMPOUND = (MainCmd.equipCmd << 16) + EquipCmd.allcompose;

        //11,10 查询商店信息
        public const int QUERYSHOP = (MainCmd.shopCmd << 16) + ShopCmd.queryShop;

        //11,4 钞票SHOP1402
        public const int SHOP1402 = (MainCmd.shopCmd << 16) + ShopCmd.shop1402;

        //11,12 SHOP1202
        public const int SHOP1202 = (MainCmd.shopCmd << 16) + ShopCmd.shop1202;

        //11,7 SHOP1201
        public const int SHOP1201 = (MainCmd.shopCmd << 16) + ShopCmd.shop1201;

        //13,1 修改settings
        public const int CHANGESETTINGS = (MainCmd.settings << 16) + SettingsCmd.changeSettings;

        //13,2 查询设置
        public const int QUERYSETTINGS = (MainCmd.settings << 16) + SettingsCmd.querySettings;

        //13,3  设置分享
        public const int SETSHARE = (MainCmd.settings << 16) + SettingsCmd.setShare;

        //13,4  查询分享
        public const int QUERYSHARE = (MainCmd.settings << 16) + SettingsCmd.queryShare;

        //13,5  发送兑换码
        public const int SENDGIFTCODE = (MainCmd.settings << 16) + SettingsCmd.sendGiftCode;

        //13,6  查询版本号
        public const int QUERYVERSION = (MainCmd.settings << 16) + SettingsCmd.queryVersion;

        //13,8  查询版本号
        public const int SUBMITQUEST = (MainCmd.settings << 16) + SettingsCmd.submitQuest;

        //15,1
        public const int QUERYBANK = (MainCmd.bank << 16) + BankCmd.query;

        //17,1  查询首充
        public const int QUERYCHARGE = (MainCmd.charge << 16) + Charge.query;

        //17,2  领取首充
        public const int GETCHARGE = (MainCmd.charge << 16) + Charge.get;

        //18,1 查询怪物图鉴
        public const int QUERYMONSTERCOLLECTION = (MainCmd.monsterCollection << 16) + MonsterCollection.query;


        /// <summary>
        /// 18,2 领取怪物图鉴奖励
        /// 传入参数:type;getType(;bookId)
        /// type:1:敌对势力 2：武器 getType:1:领取单个2：领取所有
        /// </summary>
        public const int RECEIVECOLLECTIONREWARD = (MainCmd.monsterCollection << 16) + MonsterCollection.receive;

        //19,1 服务器当前时间戳
        public const int SERVERTIME = (MainCmd.timeCmd << 16) + TimeCmd.time;

        //19,2 服务器每天更新时间
        public const int UPDATETIME = (MainCmd.timeCmd << 16) + TimeCmd.updateTime;

        //14,1 解锁通行证下一等级
        public const int UNLOCKNEXTPASSLEVEL = (MainCmd.passCmd << 16) + PassCmd.unlock;

        //14,2 查询通行证信息
        public const int QUERYPASS = (MainCmd.passCmd << 16) + PassCmd.queryPass;

        //14,3 领取通行证奖励
        public const int GETPASSREWARD = (MainCmd.passCmd << 16) + PassCmd.getReward;

        //14,5 解锁通行证令牌 废弃 使用订单接口
        public const int GETPASSTOKEN = (MainCmd.passCmd << 16) + PassCmd.buyToken;

        //14,6 查询通行证解锁时间 废弃
        public const int PASSTIME = (MainCmd.passCmd << 16) + PassCmd.time;

        /// <summary>
        /// 14,8 查询通行证经验
        /// </summary>
        public const int PASSEXP = (MainCmd.passCmd << 16) + PassCmd.exp;

        //8,1 查询活动相关信息
        public const int QUERYACTIVITY = (MainCmd.activeSystemCmd << 16) + ActivityCmd.query;

        //8,2 通过活动id查询单个活动信息
        public const int QUERTSINGLEACTIVITY = (MainCmd.activeSystemCmd << 16) + ActivityCmd.querySingleActivity;

        //8,3  购买骰子
        public const int BUYDICE = (MainCmd.activeSystemCmd << 16) + ActivityCmd.buyDice;

        //8,4 大富翁兑换活动物品
        public const int MONOEXCHANGE = (MainCmd.activeSystemCmd << 16) + ActivityCmd.exchange;

        //8,5 大富翁行动
        public const int MONOPOLYACTION = (MainCmd.activeSystemCmd << 16) + ActivityCmd.monopolyAction;

        //8,6 兑换体力商店
        public const int ENERGYSHOP = (MainCmd.activeSystemCmd << 16) + ActivityCmd.energyShop;

        //8,7 查询活动任务
        public const int QUERYACTIVITYTASK = (MainCmd.activeSystemCmd << 16) + ActivityCmd.queryActivityTask;

        //8,8 查询单个活动红点
        public const int QUERYACTIVITYREDDOT = (MainCmd.activeSystemCmd << 16) + ActivityCmd.queryActivityRedDot;

        //3,1 查询日常周长任务
        public const int QUERYDAYANDWEEKTASK = (MainCmd.taskSystemCmd << 16) + TaskCmd.queryDayAndWeek;

        //3,2 领取任务奖励
        public const int GETTASKSCORE = (MainCmd.taskSystemCmd << 16) + TaskCmd.getTask;

        //3,3 领取任务宝箱奖励
        public const int GETTASKBOX = (MainCmd.taskSystemCmd << 16) + TaskCmd.getBox;

        //3,4 查询成就
        public const int QUERYACHIEVE = (MainCmd.taskSystemCmd << 16) + TaskCmd.queryAchieve;

        //3,6 查询成就    
        public const int QUERYACHIEVEMENT = (MainCmd.taskSystemCmd << 16) + TaskCmd.queryAchievement;

        //3,7 一键领取日常周常  
        public const int GETALLDAILY = (MainCmd.taskSystemCmd << 16) + TaskCmd.getAllDaily;

        //3-8 查询日常周常红点 
        public const int QUERYDAILYREDDOT = (MainCmd.taskSystemCmd << 16) + TaskCmd.queryDailyRedDot;

        //4,2  预支付,返回支付码   
        public const int PREPAY = (MainCmd.paySystemCmd << 16) + PayCmd.prePay;

        /// <summary>
        /// 20,1 查询各模块红点
        /// </summary>
        public const int QUERYREDDOT = (MainCmd.reddot << 16) + RedDot.query;

        //99,1 接收邮件广播
        public const int BOARDCASTMAIL = (MainCmd.boardCastCmd << 16) + BoardCastCmd.mail;

        //99-7 定时任务拉取标记(东八时区6.00)
        public const int BOARDCASTUPDATETASK = (MainCmd.boardCastCmd << 16) + BoardCastCmd.updateTask;

        //99,8 属性变更时广播
        public const int BOARDCASTUPDATEPROPERTY = (MainCmd.boardCastCmd << 16) + BoardCastCmd.updateProperty;

        //99,9 tag_func模块任务变更时广播
        public const int BOARDCASTUPDATEFUCTASK = (MainCmd.boardCastCmd << 16) + BoardCastCmd.updateFuncTask;

        //99,10 支付的返回值return StringValue
        public const int BOARDCASTPAYRESPONSE = (MainCmd.boardCastCmd << 16) + BoardCastCmd.payResponse;

        private static class MainCmd
        {
            //登录模块

            public const int loginCmd = 0;


            //玩家资产模块

            public const int playerCmd = 1;


            //战力培养模块

            public const int combatPowerCmd = 2;


            //任务模块

            public const int taskSystemCmd = 3;


            //支付模块

            public const int paySystemCmd = 4;


            //邮件模块

            public const int mailSystemCmd = 5;


            //掉落模块

            public const int dropSystemCmd = 6;


            //外部系统模块

            public const int externalSystemCmd = 7;


            //活动系统模块

            public const int activeSystemCmd = 8;


            // 装备系统模块

            public const int equipCmd = 9;


            //充值系统接口

            public const int paymentSystem = 701;


            //背包模块

            public const int bagCmd = 10;

            //商店模块
            public const int shopCmd = 11;

            //设置
            public const int settings = 13;

            //设置
            public const int bank = 15;

            //首充
            public const int charge = 17;

            //怪物图鉴
            public const int monsterCollection = 18;


            //时间模块
            public const int timeCmd = 19;

            //广播模块
            public const int boardCastCmd = 99;

            //通行证模块
            public const int passCmd = 14;

            //通行证模块
            public const int reddot = 20;
        }


        //子路由模块

        #region SubRoute

        private static class Charge
        {
            public const int query = 1;
            public const int get = 2;
        }

        private static class RedDot
        {
            public const int query = 1;
        }

        private static class MonsterCollection
        {
            public const int query = 1;
            public const int receive = 2;
        }

        private static class SettingsCmd
        {
            public const int querySettings = 2;
            public const int changeSettings = 1;
            public const int setShare = 3;
            public const int queryShare = 4;
            public const int sendGiftCode = 5;
            public const int queryVersion = 6;
            public const int submitQuest = 8;
        }

        private static class BoardCastCmd
        {
            public const int mail = 1;
            public const int updateTask = 7;
            public const int updateProperty = 8;
            public const int updateFuncTask = 9;

            public const int payResponse = 10;
        }

        private static class CombatPowerCmd
        {
            public const int battleGain = 1;
            public const int chapterInfo = 4;
            public const int playerProperty = 7;
            public const int queryTalent = 10;

            public const int requestBattleId = 3;
            public const int queryCanStart = 5;

            public const int getSweepReward = 16;
            public const int getMainProperty = 17;
        }

        private static class BagCmd
        {
            public const int findItem = 1;

            public const int saveItem = 2;

            public const int deleteItem = 3;

            public const int selfChooseBox = 5;
        }

        private static class EquipCmd
        {
            public const int allcompose = 1;

            public const int downgrade = 2;

            public const int upgrade = 3;

            public const int downQuality = 4;

            public const int findEquip = 5;

            public const int compose = 6;

            public const int allUpgrade = 7;

            public const int insertEquip = 9;

            public const int wearEquip = 10;

            public const int allwearEquip = 11;

            public const int unwearEquip = 12;
            public const int wearunwearEquip = 13;
        }

        //支付接口
        private static class PayCmd
        {
            //预支付
            public const int prePay = 2;
            //public const int switchAccount = 2;
        }

        //登录验证
        private static class LoginCmd
        {
            public const int loginVerify = 1;
            public const int switchAccount = 2;
        }

        private static class PlayerCmd
        {
            public const int initCacheRole = 1;
            public const int editCacheRole = 2;
            public const int findCacheRole = 3;
            public const int checkStatus = 5;
            public const int queryLevel = 8;
            public const int rename = 6;
        }


        private static class TalentCmd
        {
            public const int lockTatant = 11;
        }

        private static class ChallengCmd
        {
            public const int challengeClaim = 12;
            public const int challengeQuery = 13;
            public const int consumeRebirthCoin = 14;
            public const int setBattleInfo = 15;
        }

        private static class MailCmd
        {
            public const int queryMail = 1;
        }

        private static class PatrolCmd
        {
            public const int queryAutoPatrol = 3;
        }

        private static class ShopCmd
        {
            public const int queryShop = 10;
            public const int shop1402 = 4;
            public const int shop1202 = 12;
            public const int shop1201 = 7;
        }

        private static class TimeCmd
        {
            public const int time = 1;
            public const int updateTime = 2;
        }

        private static class PassCmd
        {
            public const int unlock = 1;
            public const int queryPass = 2;
            public const int getReward = 3;
            public const int buyToken = 5;
            public const int time = 6;
            public const int exp = 8;
        }

        private static class ActivityCmd
        {
            public const int query = 1;
            public const int querySingleActivity = 2;
            public const int buyDice = 3;
            public const int exchange = 4;
            public const int monopolyAction = 5;
            public const int energyShop = 6;
            public const int queryActivityTask = 7;
            public const int queryActivityRedDot = 8;
        }

        private static class TaskCmd
        {
            public const int queryDayAndWeek = 1;
            public const int getTask = 2;
            public const int getBox = 3;
            public const int queryAchieve = 4;
            public const int queryAchievement = 6;
            public const int getAllDaily = 7;
            public const int queryDailyRedDot = 8;
        }

        private static class BankCmd
        {
            public const int query = 1;
        }

        #endregion
    }
}