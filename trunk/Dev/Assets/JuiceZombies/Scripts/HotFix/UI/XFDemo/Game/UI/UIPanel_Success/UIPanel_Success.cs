//---------------------------------------------------------------------
// UnicornStudio
// Author: 黄金国
// Time: 2023-9-27
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HotFix_UI;
using Main;
using UnityEngine;
using LevelInfo = Common.LevelInfo;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Success)]
    internal sealed class UISuccessEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Success;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Success>();
        }
    }

    public partial class UIPanel_Success : UI, IAwake
    {
        //读取多语言表
        private Tblanguage language;

        //关卡ID
        private int publicLevelID = 0;


        public async void Initialize()
        {
            await UnicornUIHelper.InitBlur(this);
            UnicornUIHelper.StartStopTime(false);

            AudioManager.Instance.PlayFModAudio(2203);
            language = ConfigManager.Instance.Tables.Tblanguage;

            WebMessageHandlerOld.Instance.AddHandler(CMDOld.BATTLEGAIN, OnBattleGainResponse);
            //NetWorkManager.Instance.SendMessage(CMDOld.BATTLEGAIN);

            QueryBattleGain();
            //初始化文本
            InitStateTxt();


            this.GetFromReference(KCongratulationsText).GetTextMeshPro()
                .SetTMPText(language.Get("level_success_text_content").current);

            var KBtn_Confirm = this.GetFromReference(UIPanel_Fail.KBtn_Confirm);
            var KBtn_DamageInfo = this.GetFromReference(UIPanel_Fail.KBtn_DamageInfo);
            var KText_Confirm = this.GetFromReference(UIPanel_Fail.KText_Confirm);

            KText_Confirm.GetTextMeshPro().SetTMPText(language.Get("common_state_confirm").current);

            KBtn_Confirm.GetRectTransform().SetScale(Vector2.one);
            KBtn_DamageInfo.GetRectTransform().SetScale(Vector2.one);
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Confirm, () =>
            {
                Close();
                UnicornUIHelper.ExitRunTimeScene();
                // var sceneController = Common.Instance.Get<SceneController>();
                // var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
                // SceneResManager.WaitForCompleted(sceneObj).ToCoroutine();
            });
            UnicornTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_DamageInfo,
                () => { UIHelper.CreateAsync(UIType.UIPanel_BattleDamageInfo); });
        }

        void OnIsFirstResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var gameChapter = new GameChapter();
            gameChapter.MergeFrom(e.data);
            if (e.data.IsEmpty)
            {
                Log.Debug("OnIsFirstResponse e.data.IsEmpty", Color.red);
                return;
            }

            string isfirst = gameChapter.IsFirst;
            Debug.Log("isfirst: " + isfirst);
            //if (isfirst == "true")
            //{
            //    this.GetFromReference(KNewRecordTxt).SetActive(true);
            //}
            //else
            //{
            //    this.GetFromReference(KNewRecordTxt).SetActive(false);
            //}
        }


        void QueryBattleGain()
        {
            UnityHelper.OutPlayerGoldAndLiveTime(out var gameTimeData, out var playerData, out var chaStats);
            var battleGain = UnicornUIHelper.GetBattleGain(gameTimeData.logicTime.elapsedTime, playerData, chaStats, true);
            Log.Debug($"battleGain {battleGain}");

            NetWorkManager.Instance.SendMessage(CMDOld.BATTLEGAIN, battleGain);

            if (UnicornUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uis = ui as UIPanel_RunTimeHUD;
                RepeatedField<int> skills = new RepeatedField<int>();

                foreach (var kv in uis.skillsDic)
                {
                    skills.Add(kv.Key - 1 + kv.Value);
                }

                skills.AddRange(uis.displaySelectedTechs);
                var battleLog = new BattleLog
                {
                    BattleId = ResourcesSingletonOld.Instance.battleData.battleId,
                    LevelId = ResourcesSingletonOld.Instance.levelInfo.levelId,
                    PassLevel = 1,
                    Attr = chaStats.chaProperty.atk,
                    Hp = chaStats.chaProperty.maxHp,
                    KillSum = battleGain.KillMobs + battleGain.KillBoss + battleGain.KillElite,
                    Banknotes = playerData.playerData.exp,
                    ResurrectionSum = uis.reBirthCount
                };
                battleLog.SkillList.AddRange(skills);

                NetWorkManager.Instance.SendMessage(CMDOld.SETBATTLEINFO, battleLog);
            }

            //设置杀敌数
            this.GetFromKeyOrPath(KKillNumberText).GetTextMeshPro()
                .SetTMPText(playerData.playerData.killEnemy.ToString());
            //设置章节id
            var tblevel = ConfigManager.Instance.Tables.Tblevel;
            if (tblevel[ResourcesSingletonOld.Instance.levelInfo.levelId].type == 1) //如果是主线章节的话
            {
                this.GetFromReference(KLevelText).GetTextMeshPro().SetTMPText(
                    language.Get("common_chapter_name").current + " "
                                                                + tblevel[ResourcesSingletonOld.Instance.levelInfo.levelId]
                                                                    .num.ToString());
            }
            else
            {
                // this.GetFromReference(KLevelText).GetTextMeshPro()
                //     .SetTMPText(language.Get(tblevel[battleGain.LevelId].name).current);
            }
        }


        void OnBattleGainResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            var levelInfo = new LevelInfo();
            levelInfo.MergeFrom(e.data);
            Log.Debug($"levelInfo{levelInfo}", Color.green);

            string isfirst = levelInfo.IsFirst;

            //if (isfirst == "true")
            //{
            //    this.GetFromReference(KNewRecordTxt).SetActive(true);
            //}
            //else
            //{
            //    this.GetFromReference(KNewRecordTxt).SetActive(false);
            //}

            var rewards = UnicornUIHelper.TurnStrReward2List(levelInfo.Args);
            InitReWardItem(rewards);
        }

        public async void InitReWardItem(List<Vector3> args)
        {
            var KDroppedItemView = GetFromReference(UIPanel_Success.KDroppedItemView);
            var content = KDroppedItemView.GetScrollRect().Content;
            var list = content.GetList();
            list.Clear();
            foreach (var reward in args)
            {
                var ui = await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward, false);
                UnicornUIHelper.SetRewardOnClick(reward, ui);
            }

            list.Sort(UnicornUIHelper.RewardUIComparer);

            // list.Sort((obj11, obj21) =>
            // {
            //     Tblanguage language = ConfigManager.Instance.Tables.Tblanguage;
            //     Tbuser_variable user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            //     TbitemOld item = ConfigManager.Instance.Tables.TbitemOld;
            //     Tbequip_data equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            //     Tbequip_quality equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            //     Tbquality quality = ConfigManager.Instance.Tables.Tbquality;
            //
            //     var obj111 = obj11 as UICommon_RewardItem;
            //     var obj211 = obj21 as UICommon_RewardItem;
            //     var obj1 = obj111.trueReward;
            //     var obj2 = obj211.trueReward;
            //     var obj1rewardx = (int)obj1.x;
            //     var obj1rewardy = (int)obj1.y;
            //     var obj1rewardz = (int)obj1.z;
            //     var obj2rewardx = (int)obj2.x;
            //     var obj2rewardy = (int)obj2.y;
            //     var obj2rewardz = (int)obj2.z;
            //
            //     if (UnicornUIHelper.IsResourceReward(obj1) && UnicornUIHelper.IsResourceReward(obj2))
            //     {
            //         if (obj1rewardx < obj2rewardx)
            //             return -1;
            //         else if (obj1rewardx > obj2rewardx)
            //             return 1;
            //     }
            //
            //     if (UnicornUIHelper.IsResourceReward(obj1) && !UnicornUIHelper.IsResourceReward(obj2))
            //         return -1;
            //     else if (!UnicornUIHelper.IsResourceReward(obj1) && UnicornUIHelper.IsResourceReward(obj2))
            //         return 1;
            //     // if (obj1rewardx != 5 && obj2rewardx == 5)
            //     //     return -1;
            //     // else if (obj1rewardx == 5 && obj2rewardx != 5)
            //     //     return 1;
            //
            //     if (obj1rewardx == 11 && obj2rewardx != 11)
            //         return 1;
            //     else if (obj1rewardx != 11 && obj2rewardx == 11)
            //         return -1;
            //
            //     if (obj1rewardx == 5 && obj2rewardx == 5)
            //     {
            //         if (item.Get(obj1rewardy).sort < item.Get(obj2rewardy).sort)
            //             return -1;
            //         else if (item.Get(obj1rewardy).sort > item.Get(obj2rewardy).sort)
            //             return 1;
            //         if (obj1rewardy < obj2rewardy)
            //             return -1;
            //         else if (obj1rewardy > obj2rewardy)
            //             return 1;
            //     }
            //
            //
            //     if (obj1rewardx == 11 && obj2rewardx == 11)
            //     {
            //         if (!UnicornUIHelper.IsCompositeEquipReward(obj1) &&
            //             UnicornUIHelper.IsCompositeEquipReward(obj2))
            //             return -1;
            //         else if (UnicornUIHelper.IsCompositeEquipReward(obj1) &&
            //                  !UnicornUIHelper.IsCompositeEquipReward(obj2))
            //             return 1;
            //
            //         if (equip_data.Get(obj1rewardy).quality >
            //             equip_data.Get(obj2rewardy).quality)
            //             return -1;
            //         else if (equip_data.Get(obj1rewardy).quality <
            //                  equip_data.Get(obj2rewardy).quality)
            //             return 1;
            //
            //         if (equip_data.Get(obj1rewardy).sYn == 1 &&
            //             equip_data.Get(obj2rewardy).sYn != 1)
            //             return -1;
            //         else if (equip_data.Get(obj1rewardy).sYn != 1 &&
            //                  equip_data.Get(obj2rewardy).sYn == 1)
            //             return 1;
            //
            //
            //         if (equip_data.Get(obj1rewardy).posId <
            //             equip_data.Get(obj2rewardy).posId)
            //             return -1;
            //         else if (equip_data.Get(obj1rewardy).posId >
            //                  equip_data.Get(obj2rewardy).posId)
            //             return 1;
            //
            //         if (obj1rewardy > obj2rewardy)
            //             return -1;
            //         else if (obj1rewardy < obj2rewardy)
            //             return 1;
            //     }
            //
            //
            //     return 0;
            // });
        }

        /// <summary>
        /// 初始化文本
        /// </summary>
        void InitStateTxt()
        {
            //初始化成功标题
            this.GetFromReference(KSuccessTitle).GetXImage()
                .SetSprite(UnicornUIHelper.GetL10NPicName("level_success"), true);
            //初始化新纪录文本
            //this.GetFromReference(KNewRecordTxt).GetTextMeshPro().SetTMPText(language.Get("level_new_record").current);
            this.GetFromReference(KNewRecordTxt).SetActive(false);
            //读取金币，存活时间以及杀敌数
            UnityHelper.OutPlayerGoldAndLiveTime(out var gameTimeData, out var playerData, out var chaStats);
            //设置杀敌数
            this.GetFromKeyOrPath(KKillNumberText).GetTextMeshPro()
                .SetTMPText(playerData.playerData.killEnemy.ToString());
            this.GetFromReference(KText_Money).GetTextMeshPro()
                .SetTMPText(playerData.playerData.gold.ToString());
            //设置恭喜通关
            this.GetFromReference(KCongratulationsText).GetTextMeshPro()
                .SetTMPText(language.Get("level_success_text_content").current);
            //读取level表
            var tblevel = ConfigManager.Instance.Tables.Tblevel;
            //读取关卡ID
            publicLevelID = ResourcesSingletonOld.Instance.levelInfo.levelId;
            //设置章节id
            var level = tblevel.GetOrDefault(publicLevelID);
            if (level.type == 1)
            {
                //如果是主线章节的话，显示“章节”+章节ID
                this.GetFromReference(KLevelText).GetTextMeshPro().SetTMPText(
                    language.Get("common_chapter_name").current + " "
                                                                + level.num.ToString());
            }
            else
            {
                //如果是其他关卡的话，只显示关卡名称
                // this.GetFromReference(KLevelText).GetTextMeshPro()
                //     .SetTMPText(language.Get(tblevel[publicLevelID].name).current);
            }

            //设置掉落
            // var loopRect = this.GetFromReference(KDroppedItemView).GetScrollRect();
            // //设置固定掉落
            // //读取固定掉落表
            // List<Vector3> RewardList = level.reward;
            // int rewardInt = RewardList.Count;
            // for (int i = 0; i < rewardInt; i++)
            // {
            //     SettlementStruct settlementStruct = new SettlementStruct();
            //     settlementStruct.Clearance = true;
            //     settlementStruct.id = (int)RewardList[i].x;
            //     settlementStruct.count = (int)RewardList[i].z;
            //     var ui = UIHelper.Create(UIType.UISettlementItem, settlementStruct,
            //         loopRect.Content.GameObject.transform);
            //     ui.SetParent(this, true);
            //     uiList.Add(ui);
            // }
            UnicornUIHelper.ForceRefreshLayout(GetFromReference(KMoney));
            UnicornUIHelper.ForceRefreshLayout(GetFromReference(KKillRecordImage));
        }

        protected override void OnClose()
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMDOld.BATTLEGAIN, OnBattleGainResponse);
            UnicornUIHelper.StartStopTime(true);

            base.OnClose();
        }
    }
}