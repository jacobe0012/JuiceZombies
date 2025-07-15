//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cfg.config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HotFix_UI;
using Main;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_BattleShop)]
    internal sealed class UIPanel_BattleShopEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_BattleShop;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_BattleShop>();
        }
    }

    public partial class UIPanel_BattleShop : UI, IAwake<int>
    {
        private Tbbattle_constant constant;
        //private Tbbattle_exp battle_exp;

        private Tblanguage language;
        private Tbbattleshop_stage battleshop_stage;
        private Tbskill_binding player_skill_binding;
        private Tbskill_binding_rank player_skill_binding_rank;

        private Tblevel levelConfig;
        private Tbuser_variable user_variable;
        private Tbbattleshop_drop battleshop_drop;
        private Tbskill_quality player_skill_quality;
        private Tbskill player_skill;
        private Tbskill_effectNew skill_effect;
        private Tbskill_effectElement skill_effectElement;
        private Tbguide tbguide;

        private int battleshop_refresh_price;
        private int battleshop_refresh_num;
        private EntityManager entityManager;
        private EntityQuery wbeQuery;
        private EntityQuery playerQuery;
        private EntityQuery enemyQuery;
        private EntityQuery gameRandomQuery;
        private DynamicBuffer<Skill> skillBuffer;
        private PlayerData playerData;
        private Entity player;
        //private PlayerData playerData;
        private const int MaxStars = 5;
        private int shopId;
        /// <summary>
        /// 本次打开时总免费刷新次数
        /// </summary>
        private int sumFree;
        private UI panelRunTimeHUD = null;
        private Dictionary<int, bool> curIsBuyDic = new Dictionary<int, bool>();
        private GameTimeData _gameTimeData;
        private string redColor = "FF0000";
        private bool isGuide;
        
        private CancellationTokenSource cts = new CancellationTokenSource();

        public async void Initialize(int args)
        {
            await JiYuUIHelper.InitBlur(this);
            this.SetActive(false);
            Log.Debug($"商店id {args}", Color.green);
            shopId = args;

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData),
                typeof(GameOthersData));
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            gameRandomQuery = entityManager.CreateEntityQuery(typeof(GameRandomData));
            enemyQuery = entityManager.CreateEntityQuery(typeof(EnemyData));


            player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            skillBuffer = entityManager.GetBuffer<Skill>(player);
            playerData = entityManager.GetComponentData<PlayerData>(player);
            _gameTimeData = entityManager.GetComponentData<GameTimeData>(wbe);
            var gameOthersData = entityManager.GetComponentData<GameOthersData>(wbe);
            gameOthersData.battleShopStage = shopId;
            entityManager.SetComponentData(wbe, gameOthersData);

            OnSkillTriggerForOpen(player, ref skillBuffer);

            await UniTask.Delay(200);
            JiYuUIHelper.StartStopTime(false);

            //
            //UnityHelper.StopTime();

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var parent))
            {
                panelRunTimeHUD = parent;
            }

            //parent = this.GetParent<UIPanel_RunTimeHUD>();
            InitJson();

            InitPanel();

            // int panelId = 612;
            // foreach (var guide in tbguide.DataList)
            // {
            //     if (guide.targetId == panelId &&
            //         ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
            //     {
            //         UIHelper.CreateAsync(this, UIType.UISubPanel_Guid, guide.id,
            //             this.GameObject.transform);
            //     }
            // }

            this.SetActive(true);

            Guide().Forget();
        }

        public void GuideOnClick()
        {
            Log.Error($"isGuide{isGuide}");
            if (isGuide)
            {
                if (JiYuUIHelper.TryGetUI(UIType.UISubPanel_Guid, out var ui))
                {
                    ui.Dispose();
                }
            }
        }

        async UniTaskVoid Guide()
        {
            //await UniTask.Delay(1000,true);
            var KContainer_Skill = this.GetFromReference(UIPanel_BattleShop.KContainer_Skill);
            var guide = tbguide.DataList.Where(a => a.guideType == 314).FirstOrDefault();
            if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
            {
                isGuide = true;
                var guideUI = await UIHelper.CreateAsync(UIType.UISubPanel_Guid, guide.id);

                var KImg_Bg = guideUI.GetFromReference(UISubPanel_Guid.KImg_Bg);
                JiYuUIHelper.SetForceGuideRectUI(KContainer_Skill, KImg_Bg);
            }
        }

        void InitPanel()
        {
            var battleshopStage = battleshop_stage.Get(shopId);
            int refreshPool = battleshopStage.battleshopDropId;
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            var KBtn_ShopDes = GetFromReference(UIPanel_BattleShop.KBtn_ShopDes);
            var KTxt_ShopName = GetFromReference(UIPanel_BattleShop.KTxt_ShopName);
            var KTxt_StagName = GetFromReference(UIPanel_BattleShop.KTxt_StagName);
            var KTxt_SumMoney = GetFromReference(UIPanel_BattleShop.KTxt_SumMoney);
            var KTxt_PropBlue = GetFromReference(UIPanel_BattleShop.KTxt_PropBlue);
            var KTxt_PropPurple = GetFromReference(UIPanel_BattleShop.KTxt_PropPurple);
            var KTxt_PropYellow = GetFromReference(UIPanel_BattleShop.KTxt_PropYellow);
            var KContainer_Binding = GetFromReference(UIPanel_BattleShop.KContainer_Binding);
            var KContainer_Skill = GetFromReference(UIPanel_BattleShop.KContainer_Skill);
            var KBtn_Continue = GetFromReference(UIPanel_BattleShop.KBtn_Continue);
            var KText_Continue = GetFromReference(UIPanel_BattleShop.KText_Continue);
            var KBtn_Lock = GetFromReference(UIPanel_BattleShop.KBtn_Lock);
            var KText_Refresh = GetFromReference(UIPanel_BattleShop.KText_Refresh);
            var KTxt_RefreshMoney = GetFromReference(UIPanel_BattleShop.KTxt_RefreshMoney);
            var KContainer_Selcted = GetFromReference(UIPanel_BattleShop.KContainer_Selcted);
            var KImg_Icon_Binding = GetFromReference(UIPanel_BattleShop.KImg_Icon_Binding);
            var KTxt_Name_Binding = GetFromReference(UIPanel_BattleShop.KTxt_Name_Binding);
            var KContiner_Btn_Refresh = GetFromReference(UIPanel_BattleShop.KContiner_Btn_Refresh);
            var KBg_Mask = GetFromReference(UIPanel_BattleShop.KBg_Mask);
            var KContainer_Top = GetFromReference(UIPanel_BattleShop.KContainer_Top);
            var KBottom = GetFromReference(UIPanel_BattleShop.KBottom);
            var KContainer = GetFromReference(UIPanel_BattleShop.KContainer);
            var KContainer_Mid = GetFromReference(UIPanel_BattleShop.KContainer_Mid);
            var KText_NoSkills = GetFromReference(UIPanel_BattleShop.KText_NoSkills);

            var RenderHeight = KContainer_Top.GetRectTransform().Height() +
                               KContainer_Binding.GetRectTransform().Height() +
                               KContainer_Mid.GetRectTransform().Height() +
                               KBottom.GetRectTransform().Height();
            var upOffset = (Screen.height - RenderHeight) / 2f;
            //KContainer.GetRectTransform().SetAnchoredPositionY(-upOffset);

            //KTxt_PropBlue.
            //KTxt_PropPurple
            //KTxt_PropYellow
            KContainer_Selcted.SetActive(false);
            SetChanceUI(refreshPool);
            battleshop_refresh_price = constant.Get("battleshop_refresh_price").constantValue;
            KTxt_ShopName.GetTextMeshPro().SetTMPText(language.Get("battleshop_name").current);

            KText_Continue.GetTextMeshPro().SetTMPText(language.Get("common_state_continuebattle").current);
            KText_Refresh.GetTextMeshPro().SetTMPText(language.Get("common_state_refresh").current);
            KText_NoSkills.GetTextMeshPro().SetTMPText(language.Get("common_state_battle_shop_sell_out").current);
            KText_NoSkills.SetActive(false);

            int refreshPrice =
                (int)Mathf.Ceil(battleshop_refresh_price * (GetPlayerData().playerData.refreshShopRatios / 10000f));
            refreshPrice = math.max(refreshPrice, 0);

            int curMoney = GetPlayerMoney();
            string refreshStr = UnityHelper.RichTextColor(refreshPrice.ToString(), "FFFFFF");
            if (curMoney < refreshPrice)
            {
                refreshStr = UnityHelper.RichTextColor(refreshPrice.ToString(), redColor);
            }

            KTxt_RefreshMoney.GetTextMeshPro().SetTMPText(refreshStr);
            parentUI.bossState = battleshopStage.stage;
            parentUI.state = battleshopStage.stageDetail;
            var stageStr =
                $"{language.Get("common_stage").current}{parentUI.bossState}-{parentUI.state}";

            KTxt_StagName.GetTextMeshPro().SetTMPText(stageStr);
            RefreshMoneyUI();


            CreateBindingItem().Forget();
            CreateSkillsItem(refreshPool).Forget();
            //var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            //var playerData = entityManager.GetComponentData<PlayerData>(player);

            DisplayFreeRefresh();
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_ShopDes, async () =>
            {
                if (JiYuUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
                {
                    JiYuUIHelper.DestoryAllTips();
                    ;
                    return;
                }
                var desc = language.Get("battleshop_tips").current;
                var KText_Des = GetFromReference(UIPanel_Pass.KText_Des);
                GetFromReference(KCommon_ItemTips).SetActive(!GetFromReference(KCommon_ItemTips).GameObject.activeSelf);
                KText_Des.GetTextMeshPro().SetTMPText(desc);
                var height = KText_Des.GetTextMeshPro().Get().preferredHeight;
                var width = KText_Des.GetTextMeshPro().Get().preferredWidth;
                KText_Des.GetRectTransform().SetHeight(height);
                GetFromReference(Kcontent).GetRectTransform().SetHeight(height + 76 * 2);

            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Continue, () =>
            {
                JiYuUIHelper.DestoryAllTips();
                ;
                JiYuUIHelper.StartStopTime(true);
                Close();
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Lock, () =>
            {
                JiYuUIHelper.DestoryAllTips();
                ;
                if (!IsAllBought())
                {
                    var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
                    parentUI.isLocked = !parentUI.isLocked;
                    LockSkillsItem(parentUI.isLocked);
                    var str = parentUI.isLocked ? "icon_equip_skill_lock_2" : "icon_equip_skill_lock_1";
                    KBtn_Lock.GetImage().SetSprite(str, false);
                }
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KContiner_Btn_Refresh,
                () =>
                {
                    KContainer_Selcted.SetActive(false);
                    JiYuUIHelper.DestoryAllTips();
                    ;
                    var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
                    //if (parentUI.isLocked && !IsAnyBought()) return;
                    var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
                    var playerData = entityManager.GetComponentData<PlayerData>(player);
                    var sumFree = playerData.playerData.skillTempRefreshCount + playerData.playerData.skillRefreshCount;
                    if (sumFree > 0)
                    {
                        parentUI.refreshSkillCount++;
                        if (playerData.playerData.skillTempRefreshCount > 0)
                        {
                            playerData.playerData.skillTempRefreshCount--;
                        }
                        else
                        {
                            playerData.playerData.skillRefreshCount--;
                        }

                        entityManager.SetComponentData(player, playerData);
                        var tempCount = DoGameEvent57ForRefresh(parentUI);
                        sumFree = tempCount + playerData.playerData.skillTempRefreshCount;
                        DisplayFreeRefresh();
                        CreateSkillsItem(refreshPool).Forget();
                    }
                    else
                    {
                        int curMoney = GetPlayerMoney();

                        refreshPrice =
                            (int)Mathf.Ceil(battleshop_refresh_price *
                                            (GetPlayerData().playerData.refreshShopRatios / 10000f));
                        refreshPrice = math.max(refreshPrice, 0);
                        if (curMoney < refreshPrice)
                        {
                            JiYuUIHelper.ClearCommonResource();
                            UIHelper.CreateAsync(UIType.UICommon_Resource,
                                language.Get("text_monry_lack").current).Forget();
                            return;
                        }
                        else if (curMoney - refreshPrice < refreshPrice && curMoney - refreshPrice >= 0)
                        {
                            refreshStr = UnityHelper.RichTextColor(refreshPrice.ToString(), redColor);


                            KTxt_RefreshMoney.GetTextMeshPro().SetTMPText(refreshStr);
                        }

                        SetPlayerMoney(curMoney - refreshPrice);
                        RefreshMoneyUI();
                        parentUI.refreshSkillCount++;

                        var tempCount = DoGameEvent57ForRefresh(parentUI);
                        Debug.LogError($"skillRefreshCount:{tempCount}");
                        sumFree = tempCount;
                        DisplayFreeRefresh();
                        CreateSkillsItem(refreshPool, true).Forget();
                    }
                });

            KBg_Mask.GetButton().OnClick.Add(() =>
            {
                //this.GetFromReference(UIPanel_BattleInfo.KContainer_Skill)?.SetActive(true);
                this.GetFromReference(UIPanel_BattleInfo.KBottom)?.SetActive(true);
                JiYuUIHelper.DestoryAllTips();
                ;

                if (KContainer_Selcted.GameObject.activeSelf)
                {
                    KContainer_Selcted.SetActive(false);
                    DisableAllStageIsSelected();
                }
            });
        }

        // void StartStopTime(bool enable)
        // {
        //     return;
        //     var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
        //     var gameTimeData = entityManager.GetComponentData<GameTimeData>(wbe);
        //     gameTimeData.logicTime.gameTimeScale = enable ? _gameTimeData.logicTime.gameTimeScale : 0;
        //     gameTimeData.refreshTime.gameTimeScale = enable ? _gameTimeData.refreshTime.gameTimeScale : 0;
        //     entityManager.SetComponentData(wbe, gameTimeData);
        //
        //     var physicsSystemGroup =
        //         World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PhysicsSystemGroup>();
        //     physicsSystemGroup.Enabled = enable;
        // }

        void SetChanceUI(int poolId)
        {
            var playerData = GetPlayerData();
            var KTxt_PropBlue = GetFromReference(UIPanel_BattleShop.KTxt_PropBlue);
            var KTxt_PropPurple = GetFromReference(UIPanel_BattleShop.KTxt_PropPurple);
            var KTxt_PropYellow = GetFromReference(UIPanel_BattleShop.KTxt_PropYellow);

            var list = new List<BattleShopDrop>();
            int qualityPower1 = 0;
            int qualityPower2 = 0;
            int qualityPower3 = 0;
            foreach (var item in battleshop_drop.DataList)
            {
                if (item.id == poolId)
                {
                    int power = 0;
                    switch (item.quality)
                    {
                        case 1:
                            power = Mathf.CeilToInt(item.power *
                                                    (1 + playerData.playerData.skillWeightIncrease1 / 10000f));
                            qualityPower1 = power;
                            break;
                        case 2:
                            power = Mathf.CeilToInt(item.power *
                                                    (1 + playerData.playerData.skillWeightIncrease2 / 10000f));
                            qualityPower2 = power;
                            break;
                        case 3:
                            power = Mathf.CeilToInt(item.power *
                                                    (1 + playerData.playerData.skillWeightIncrease3 / 10000f));
                            qualityPower3 = power;
                            break;
                    }

                    list.Add(new BattleShopDrop
                    {
                        battleshop_drop = item,
                        power = power
                    });
                }
            }

            int totalPower = 0;
            foreach (var item in list)
            {
                totalPower += item.power;
            }

            KTxt_PropBlue.GetTextMeshPro().SetTMPText($"{Math.Round(qualityPower1 / (float)totalPower, 2) * 100}%");
            KTxt_PropPurple.GetTextMeshPro().SetTMPText($"{Math.Round(qualityPower2 / (float)totalPower, 2) * 100}%");
            KTxt_PropYellow.GetTextMeshPro().SetTMPText($"{Math.Round(qualityPower3 / (float)totalPower, 2) * 100}%");
        }


        private int DoGameEvent57ForRefresh(UIPanel_RunTimeHUD parentUI)
        {
            var refreshSkillCount = parentUI.refreshSkillCount;
            //var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            var gameEvents = entityManager.GetBuffer<GameEvent>(player, true);

            foreach (var gameEvent in gameEvents)
            {
                if (gameEvent.CurrentTypeId == (GameEvent.TypeId)57)
                {
                    if (gameEvent.Boolean_11)
                    {
                        Debug.LogError($"gameEvent.Boolean_11:{parentUI.refreshSkillCount}");
                        if (refreshSkillCount % gameEvent.int3_7.x == 0 && refreshSkillCount > 0)
                        {
                            var tempCount = playerData.playerData.skillRefreshCount;
                            tempCount += gameEvent.int3_7.y;
                            playerData.playerData.skillRefreshCount = tempCount;
                            entityManager.SetComponentData(player, playerData);
                            Debug.LogError(
                                $"count :{parentUI.refreshSkillCount},playerdata.skillRefreshCount:{playerData.playerData.skillRefreshCount},temp:{tempCount}");
                            return tempCount;
                        }
                    }
                }
            }

            return playerData.playerData.skillRefreshCount;

            //var sumFree = playerData.playerOtherData.tempRefreshCount + playerData.playerData.skillRefreshCount;
            //DisplayFreeRefresh(sumFree);
        }

        private async UniTaskVoid OnClickBindings(int bindId)
        {
            var playerSkillBinding = player_skill_binding.Get(bindId);
            var KImg_Icon_Binding = GetFromReference(UIPanel_BattleShop.KImg_Icon_Binding);
            var KTxt_Name_Binding = GetFromReference(UIPanel_BattleShop.KTxt_Name_Binding);
            var KTxt_Description_Binding = GetFromReference(UIPanel_BattleShop.KTxt_Description_Binding);

            KImg_Icon_Binding.GetImage().SetSpriteAsync(playerSkillBinding.pic, false).Forget();
            KTxt_Name_Binding.GetTextMeshPro().SetTMPText(language.Get(playerSkillBinding.name).current);

            KTxt_Description_Binding.GetTextMeshPro().SetTMPText(language.Get(playerSkillBinding.desc).current);
            var KContainer_SelectedItem = GetFromReference(UIPanel_BattleShop.KContainer_SelectedItem);
            var scrollRect = KContainer_SelectedItem.GetScrollRect();

            const float SelectedItemInterval = 10f;
            //scrollRect.Content.GetRectTransform().SetWidth(Screen.width);
            var gridLayoutGroup = scrollRect.Content.GetComponent<GridLayoutGroup>();
            // var cellsezeX = (Screen.width - (3 * SelectedItemInterval)) / 2f;
            //gridLayoutGroup.cellSize = new Vector2(cellsezeX, gridLayoutGroup.cellSize.y);

            var list = scrollRect.Content.GetList();
            list.Clear();
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            foreach (var skillkv in parentUI.skillsDic)
            {
                var skillId = skillkv.Key;
                var skillLevel = skillkv.Value;

                var curSkill = player_skill.Get(skillId + skillLevel - 1);

                //var playerSkillGroup = player_skill.Get(skillId);
                var bindingId = curSkill.skillBindingId[0];
                if (bindingId != bindId)
                {
                    continue;
                }

                //var playerSkill = player_skill.Get(playerSkillGroup.default0);
                var playerSkillQuality = player_skill_quality.Get(curSkill.skillQualityId);
                //var playerSkillBinding = player_skill_binding.Get(bindingId);

                var ui =
                    await list.CreateWithUITypeAsync(UIType.UIPanel_SelectItem, skillId, false) as
                        UIPanel_SelectItem;
                var KImg_bg_quality = ui.GetFromReference(UIPanel_SelectItem.KImg_bg_quality);
                var KImg_icon_skill = ui.GetFromReference(UIPanel_SelectItem.KImg_icon_skill);
                var KTxt_name_skill = ui.GetFromReference(UIPanel_SelectItem.KTxt_name_skill);
                var KTxt_Descripiton = ui.GetFromReference(UIPanel_SelectItem.KTxt_Descripiton);
                var KContainer_Stars = ui.GetFromReference(UIPanel_SelectItem.KContainer_Stars);
                //TODO:
                var qulityStr = playerSkillQuality.pic + "_tech";

                //KImg_bg_quality.GetImage().SetSpriteAsync(playerSkillQuality.pic, false).Forget();
                KImg_bg_quality.GetImage().SetSpriteAsync(qulityStr, false).Forget();

                KImg_icon_skill.GetImage().SetSpriteAsync(curSkill.icon, false).Forget();
                KTxt_name_skill.GetTextMeshPro().SetTMPText(language.Get(curSkill.name).current);
                string desc = string.Format(language.Get(curSkill.desc).current,
                    curSkill.descPara.ToArray());

                KTxt_Descripiton.GetTextMeshPro().SetTMPText(desc);

                var itemList = KContainer_Stars.GetList();
                SetStars(itemList, playerSkillQuality.levelUpperlimit, skillLevel, playerSkillQuality.id);
            }

            list.Sort((a, b) =>
            {
                var uia = a as UIPanel_SelectItem;
                var uib = b as UIPanel_SelectItem;

                var playerSkilla = player_skill.Get(uia.skillId);
                var playerSkillb = player_skill.Get(uib.skillId);
                return playerSkillb.skillQualityId.CompareTo(playerSkilla.skillQualityId);
            });
            //JiYuUIHelper.ForceRefreshLayout(scrollRect.Content);
        }

        void RefreshMoneyUI()
        {
            var KTxt_SumMoney = GetFromReference(UIPanel_BattleShop.KTxt_SumMoney);
            KTxt_SumMoney.GetTextMeshPro().SetTMPText(GetPlayerMoney().ToString());
        }

        int GetPlayerMoney()
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            return (int)playerData.playerData.exp;
        }

        PlayerData GetPlayerData()
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            return playerData;
        }

        /// <summary>
        /// 显示免费刷新
        /// </summary>
        /// <param name="sumFree"></param>
        public void DisplayFreeRefresh()
        {
            var sumFree = GetPlayerData().playerData.skillTempRefreshCount +
                          GetPlayerData().playerData.skillRefreshCount;
            if (sumFree > 0)
            {
                GetFromReference(UIPanel_BattleShop.KRoot_RefreshMoney)?.SetActive(false);
                var txtRefreshFree = GetFromReference(UIPanel_BattleShop.KTxt_RefreshFree);
                txtRefreshFree.SetActive(true);
                var strFree = sumFree.ToString();
                txtRefreshFree.GetTextMeshPro()
                    .SetTMPText(language.Get("common_free_refresh_text").current);
                GetFromReference(UIPanel_BattleShop.KTipNum).GetTextMeshPro()
                    .SetTMPText(strFree.ToString());
            }
            else
            {
                GetFromReference(UIPanel_BattleShop.KRoot_RefreshMoney)?.SetActive(true);
                GetFromReference(UIPanel_BattleShop.KTxt_RefreshFree)?.SetActive(false);
            }
        }

        void SetPlayerMoney(int value)
        {
            //var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            var freebuyCount = GetPlayerData().playerData.skillFreeBuyCount +
                               GetPlayerData().playerOtherData.skillTempFreeBuyCount;
            if (freebuyCount > 0)
            {
                Log.Debug("buy free");
                if (GetPlayerData().playerOtherData.skillTempFreeBuyCount > 0)
                {
                    playerData.playerOtherData.skillTempFreeBuyCount--;
                }
                else
                {
                    playerData.playerData.skillFreeBuyCount--;
                }
            }
            else
            {
                playerData.playerData.exp = value;
            }

            entityManager.SetComponentData(player, playerData);
        }

        bool IsAnyBought()
        {
            bool isAnyBought = false;
            var KContainer_Skill = GetFromReference(UIPanel_BattleShop.KContainer_Skill);
            var list = KContainer_Skill.GetList();

            foreach (var ui in list.Children)
            {
                var uis = ui as UIPanel_SkillItem;

                var KImg_Mask = uis.GetFromReference(UIPanel_SkillItem.KImg_Mask);
                if (KImg_Mask.GameObject.activeSelf)
                {
                    isAnyBought = true;
                    break;
                }
            }

            return isAnyBought;
        }

        bool IsAllBought()
        {
            bool isallBought = true;
            var KContainer_Skill = GetFromReference(UIPanel_BattleShop.KContainer_Skill);
            var list = KContainer_Skill.GetList();

            foreach (var ui in list.Children)
            {
                var uis = ui as UIPanel_SkillItem;

                var KImg_Mask = uis.GetFromReference(UIPanel_SkillItem.KImg_Mask);
                if (!KImg_Mask.GameObject.activeSelf)
                {
                    isallBought = false;
                    break;
                }
            }

            return isallBought;
        }

        void DisableAllStageIsSelected()
        {
            var tbskill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            var KContainer_Binding = GetFromReference(UIPanel_BattleShop.KContainer_Binding);
            var list = KContainer_Binding.GetList();
            int i = 0;
            foreach (var ui in list.Children)
            {
                i++;
                var uiS = ui as UIPanel_BindingItem;
                var isSelected = uiS.GetFromReference(UIPanel_BindingItem.KBg_IsSelected);
                isSelected.SetActive(false);
            }
        }


        private async UniTaskVoid CreateSkillsItem(int poolId, bool isRefresh = false)
        {
            var KContainer_Skill = GetFromReference(UIPanel_BattleShop.KContainer_Skill);
            var skillList = KContainer_Skill.GetList();
            skillList.Clear();
            int num = constant.Get("battleshop_refresh_num").constantValue;
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            if (parentUI.isLocked)
            {
                GetFromReference(KBtn_Lock)?.GetImage().SetSprite("icon_equip_skill_lock_2", false);
            }
            else
            {
                GetFromReference(KBtn_Lock)?.GetImage().SetSprite("icon_equip_skill_lock_1", false);
            }


            if (!parentUI.isLocked)
            {
                //parentUI.numIndexSkillIdDic.Clear();
                var newDic =
                    parentUI.numIndexSkillIdDic.ToDictionary(kvp => kvp.Key, kvp => 0);

                parentUI.numIndexSkillIdDic.Clear();
                parentUI.numIndexSkillIdDic = newDic;
            }

            //List<int> skills = parentUI.numIndexSkillIdDic.Values.Select(value => value).ToList();
            List<int> skills = parentUI.numIndexSkillIdDic.Values
                .Where(value => value != 0)
                .ToList();
            curIsBuyDic.Clear();
            for (int i = 0; i < num; i++)
            {
                var index = i;
                if (parentUI.numIndexSkillIdDic.TryGetValue(index, out var skillTempId))
                {
                    int skillId = 0;
                    if (skillTempId == 0)
                    {
                        skillId = GetOneSkillJudge(poolId, skills);
                        if (skillId == -1)
                        {
                            Log.Error($"当前没有可选择的技能");
                            continue;
                        }

                        skills.Add(skillId);
                        parentUI.numIndexSkillIdDic[index] = skillId;
                    }
                    else
                    {
                        skillId = skillTempId;
                    }

                    curIsBuyDic.Add(index, false);

                    const float SkillItemInterval = 27f;
                    var ui =
                        await skillList.CreateWithUITypeAsync(UIType.UIPanel_SkillItem, index, skillId, false) as
                            UIPanel_SkillItem;

                    var KImg_bg_quality = ui.GetFromReference(UIPanel_SkillItem.KImg_bg_quality);
                    var KImg_icon_binding = ui.GetFromReference(UIPanel_SkillItem.KImg_icon_binding);
                    var KImg_icon_skill = ui.GetFromReference(UIPanel_SkillItem.KImg_icon_skill);
                    var KTxt_name_skill = ui.GetFromReference(UIPanel_SkillItem.KTxt_name_skill);
                    var KSkillDescripitonText = ui.GetFromReference(UIPanel_SkillItem.KSkillDescripitonText);
                    var KTxt_consumeGold = ui.GetFromReference(UIPanel_SkillItem.KTxt_consumeGold);
                    var KImg_gold = ui.GetFromReference(UIPanel_SkillItem.KImg_gold);
                    var KTxt_Name_binding = ui.GetFromReference(UIPanel_SkillItem.KTxt_Name_binding);
                    var KContainer_Stars = ui.GetFromReference(UIPanel_SkillItem.KContainer_Stars);
                    var KImg_IsLock = ui.GetFromReference(UIPanel_SkillItem.KImg_IsLock);
                    var KImg_Mask = ui.GetFromReference(UIPanel_SkillItem.KImg_Mask);
                    var KTxt_Sold = ui.GetFromReference(UIPanel_SkillItem.KTxt_Sold);

                    ui.GetRectTransform().SetWidth(Screen.width - (SkillItemInterval * 2f));
                    UpdateFreeBuyDisplay(ui);

                    var itemList = KContainer_Stars.GetList();
                    int currentLevel = 0;
                    if (parentUI.skillsDic.ContainsKey(skillId))
                    {
                        currentLevel = parentUI.skillsDic[skillId];
                    }

                    var curSkill = player_skill.Get(skillId + currentLevel);

                    //var a = player_skill.Get(skillId);

                    var bindingId = curSkill.skillBindingId[0];
                    //var playerSkill = player_skill.Get(playerSkillGroup.default0);
                    var playerSkillQuality = player_skill_quality.Get(curSkill.skillQualityId);
                    var playerSkillBinding = player_skill_binding.Get(bindingId);
                    var bindingExp = playerSkillQuality.bindingExp;

                    var qulityStr = playerSkillQuality.pic + "_shop";

                    //KImg_bg_quality.GetImage().SetSpriteAsync(playerSkillQuality.pic, false).Forget();
                    KImg_bg_quality.GetImage().SetSpriteAsync(qulityStr, false).Forget();
                    KImg_icon_binding.GetImage().SetSpriteAsync(playerSkillBinding.pic, false).Forget();
                    KImg_icon_skill.GetImage().SetSpriteAsync(curSkill.icon, false).Forget();
                    KTxt_name_skill.GetTextMeshPro().SetTMPText(language.Get(curSkill.name).current);


                    Log.Debug($"{language.Get(curSkill.desc).current}");

                    string desc = string.Format(language.Get(curSkill.desc).current,
                        curSkill.descPara.ToArray());

                    KSkillDescripitonText.GetTextMeshPro().SetTMPText(desc);

                    int refreshPrice =
                        (int)Mathf.Ceil(playerSkillQuality.price *
                                        (GetPlayerData().playerData.buySkillRatios / 10000f));
                    refreshPrice = math.max(refreshPrice, 0);


                    int curMoney = GetPlayerMoney();
                    string refreshStr = UnityHelper.RichTextColor(refreshPrice.ToString(), "FFFFFF");
                    if (curMoney < refreshPrice)
                    {
                        refreshStr = UnityHelper.RichTextColor(refreshPrice.ToString(), redColor);
                    }

                    KTxt_consumeGold.GetTextMeshPro().SetTMPText(refreshStr);

                    KTxt_Name_binding.GetTextMeshPro().SetTMPText(language.Get(playerSkillBinding.name).current);
                    //var width = KTxt_Name_binding.GetTextMeshPro().Get().preferredWidth;
                    //width=width>250f?width:250f;
                    //KTxt_Name_binding.GetRectTransform().SetWidth(width);
                    KTxt_Sold.GetTextMeshPro().SetTMPText(language.Get("common_state_soldout").current);

                    KImg_IsLock.SetActive(false);
                    KImg_Mask.SetActive(false);
                    ui.GetXButton().SetEnabled(true);
                    //KContainer_Stars.


                    SetStars(itemList, playerSkillQuality.levelUpperlimit, currentLevel, playerSkillQuality.id);
                    Debug.Log($"skillid:{skillId},currentLevel:{currentLevel}");
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ui, async () =>
                    {
                        GuideOnClick();
                        //OnClickItem(skillId);
                        Debug.Log($"OnClick skillid:{skillId}");
                        var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
                        parentUI.isLocked = false;
                        GetFromReference(KBtn_Lock)?.GetImage().SetSprite("icon_equip_skill_lock_1", false);
                        LockSkillsItem(parentUI.isLocked);
                        //  JiYuUIHelper.DestoryAllTips();;
                        int skillPrice =
                            (int)Mathf.Ceil(playerSkillQuality.price *
                                            (GetPlayerData().playerData.buySkillRatios / 10000f));

                        skillPrice = math.max(skillPrice, 0);

                        int curMoney = GetPlayerMoney();

                        //bool isLevelUp = false;
                        int lastBindingLevel = 0;
                        int currentBindingLevel = 0;


                        if (parentUI.bindingsDic.ContainsKey(bindingId))
                        {
                            int lastexp = parentUI.bindingsDic[bindingId];
                            lastBindingLevel = JiYuUIHelper.GetBindingLevel(lastexp);
                        }

                        //var isStandAlone = XFramework.Common.Instance.Get<Global>().isStandAlone;)
                        var freebuyCount = GetPlayerData().playerData.skillFreeBuyCount +
                                           GetPlayerData().playerOtherData.skillTempFreeBuyCount;
                        if (curMoney < skillPrice && freebuyCount <= 0)
                        {
                            JiYuUIHelper.ClearCommonResource();
                            UIHelper.CreateAsync(UIType.UICommon_Resource,
                                language.Get("text_monry_lack").current).Forget();
                            //UIHelper.CreateOverLayTipsAsync().Forget();

                            //UIHelper.CreateAsync(UIType.UICommon_Resource, language.Get("text_monry_lack").current);
                            return;
                        }

                        if (curIsBuyDic.ContainsKey(index))
                        {
                            curIsBuyDic[index] = true;
                        }


                        if (parentUI.bindingsDic.ContainsKey(bindingId))
                        {
                            parentUI.bindingsDic[bindingId] += bindingExp;

                            if (parentUI.bindingsDic.ContainsKey(parentUI.maxExpBindingId))
                            {
                                if (parentUI.bindingsDic[bindingId] > parentUI.bindingsDic[parentUI.maxExpBindingId])
                                {
                                    parentUI.maxExpBindingId = bindingId;
                                }
                            }
                            else
                            {
                                parentUI.maxExpBindingId = bindingId;
                            }

                            //int curexp = parentUI.bindingsDic[bindingId];
                            //curLevel = JiYuUIHelper.GetBindingLevel(curexp);
                            //isLevelUp = curLevel - lastLevel >= 1 ? true : false;
                        }

                        if (parentUI.bindingsDic.ContainsKey(bindingId))
                        {
                            int lastexp = parentUI.bindingsDic[bindingId];
                            currentBindingLevel = JiYuUIHelper.GetBindingLevel(lastexp);
                        }

                        SetPlayerMoney(curMoney - skillPrice);
                        RefreshMoneyUI();
                        //int trueID = skillId + currentLevel;
                        //1.替换or增加羁绊技能
                        //2.武器技能根据阶级产生形变
                        //TODO：
                        AddOrReplaceBindingSkill(skillId, currentLevel);
                        ChangeWeaponSkillID(bindingId, lastBindingLevel, currentBindingLevel, playerQuery,
                            entityManager);

                        // UnityHelper.BeginTime();
                        // await UniTask.Yield();
                        // await UniTask.Yield();
                        // UnityHelper.StopTime();


                        foreach (var skillui in skillList.Children)
                        {
                            var skilluis = skillui as UIPanel_SkillItem;
                            // if (skilluis.skillId == skillId)
                            // {
                            //     continue;
                            // }
                            int currentLevel = 1;
                            if (parentUI.skillsDic.ContainsKey(skilluis.skillId))
                            {
                                currentLevel = parentUI.skillsDic[skilluis.skillId];
                            }

                            var curSkill = player_skill.Get(skilluis.skillId + currentLevel - 1);
                            var playerSkillQuality = player_skill_quality.Get(curSkill.skillQualityId);
                            skillPrice =
                                (int)Mathf.Ceil(playerSkillQuality.price *
                                                (GetPlayerData().playerData.buySkillRatios / 10000f));
                            skillPrice = math.max(skillPrice, 0);

                            var KTxt_consumeGold = skilluis.GetFromReference(UIPanel_SkillItem.KTxt_consumeGold);
                            int curMoney1 = GetPlayerMoney();
                            string refreshStr0 = UnityHelper.RichTextColor(skillPrice.ToString(), "FFFFFF");
                            if (curMoney1 < skillPrice)
                            {
                                refreshStr0 = UnityHelper.RichTextColor(skillPrice.ToString(), redColor);
                            }

                            KTxt_consumeGold.GetTextMeshPro().SetTMPText(refreshStr0);
                        }

                        var KTxt_RefreshMoney = GetFromReference(UIPanel_BattleShop.KTxt_RefreshMoney);
                        int refreshShop =
                            (int)Mathf.Ceil(battleshop_refresh_price *
                                            (GetPlayerData().playerData.refreshShopRatios / 10000f));
                        refreshShop = math.max(refreshShop, 0);

                        int curMoney0 = GetPlayerMoney();
                        string refreshStr = UnityHelper.RichTextColor(refreshShop.ToString(), "FFFFFF");
                        if (curMoney0 < refreshShop)
                        {
                            refreshStr = UnityHelper.RichTextColor(refreshShop.ToString(), redColor);
                        }

                        KTxt_RefreshMoney.GetTextMeshPro().SetTMPText(refreshStr);


                        if (parentUI.skillsDic.ContainsKey(skillId))
                        {
                            parentUI.skillsDic[skillId]++;
                        }
                        else
                        {
                            parentUI.skillsDic.Add(skillId, 1);
                        }

                        SetStars(itemList, playerSkillQuality.levelUpperlimit, parentUI.skillsDic[skillId],
                            playerSkillQuality.id);

                        parentUI.numIndexSkillIdDic[index] = 0;

                        KImg_Mask.SetActive(true);

                        JiYuTweenHelper.SetEaseAlphaAndScale(ui.GetFromReference(UIPanel_SkillItem.KImgSold), 0.25f, true,
                            3, 1);
                        ui.GetXButton().SetEnabled(false);
                        CreateBindingItem().Forget();
                        var KContainer_Skill = GetFromReference(UIPanel_BattleShop.KContainer_Skill);
                        var list = KContainer_Skill.GetList();

                        foreach (var ui in list.Children)
                        {
                            var uis = ui as UIPanel_SkillItem;
                            if (!curIsBuyDic.TryGetValue(uis.index, out var isBuy))
                            {
                                continue;
                            }

                            if (isBuy)
                            {
                                continue;
                            }

                            var KContainer_Stars0 = uis.GetFromReference(UIPanel_SkillItem.KContainer_Stars);
                            var KSkillDescripitonText = uis.GetFromReference(UIPanel_SkillItem.KSkillDescripitonText);

                            int nextLevel = 0;
                            if (parentUI.skillsDic.ContainsKey(uis.skillId))
                            {
                                nextLevel = parentUI.skillsDic[uis.skillId];
                            }

                            var curSkill = player_skill.Get(uis.skillId + nextLevel);

                            var playerSkillQuality0 = player_skill_quality.Get(curSkill.skillQualityId);

                            var itemList0 = KContainer_Stars0.GetList();

                            SetStars(itemList0, playerSkillQuality0.levelUpperlimit, nextLevel, playerSkillQuality0.id);

                            string desc = string.Format(language.Get(curSkill.desc).current,
                                curSkill.descPara.ToArray());
                            KSkillDescripitonText.GetTextMeshPro().SetTMPText(desc);
                            UpdateFreeBuyDisplay(uis);
                        }

                        //每购买【参数1】次技能，有【参数2】概率获得一个羁绊技能。

                        UpdatePlayerData(parentUI, out int needCount, out int property);
                        if (needCount > 0 && parentUI.buySkillCount % needCount == 0)
                        {
                            if (property / 10000 > 1)
                            {
                                int times = property / 10000 + 1;
                                for (int i = 0; i < times; i++)
                                {
                                    if (i == times - 1)
                                    {
                                        var curProperty = property % 10000;
                                        if (Random.Range(0, curProperty) > curProperty)
                                        {
                                            break;
                                        }
                                    }

                                    GetBindingSkillFromEvent(skills);
                                }
                            }
                            else
                            {
                                if (Random.Range(0, property) <= property)
                                {
                                    GetBindingSkillFromEvent(skills);
                                }
                            }
                        }
                    });
                }
            }

            var KText_NoSkills = GetFromReference(UIPanel_BattleShop.KText_NoSkills);
            KText_NoSkills.SetActive(skills.Count <= 0);


            DropSkillForRrefresh(skills, parentUI);

            LockSkillsItem(parentUI.isLocked);
            skillList.Sort((a, b) =>
            {
                var uia = a as UIPanel_SkillItem;
                var uib = b as UIPanel_SkillItem;
                return uia.index.CompareTo(uib.index);
            });

            var children = skillList.Children;
            foreach (var item in children)
            {
                JiYuTweenHelper.PlayUIImageTranstionFX(item.GetFromReference(UIPanel_SkillItem.KImg_bg_quality), cts.Token,
                    "A1DD01", JiYuTweenHelper.UIDir.Right, 0f, 0.5f);
                //item.GetComponent<CanvasGroup>().alpha = 0;
                //item.GetComponent<CanvasGroup>().DOFade(1, 0.6f).SetEase(Ease.InQuad).SetUpdate(true);
            }


            //JiYuUIHelper.ForceRefreshLayout(KContainer_Skill);
        }

        private void OnClickItem(int skillId)
        {
            //var playerSkillQuality = player_skill_quality.Get(skillId.skillQualityId);
            //var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            //  JiYuUIHelper.DestoryAllTips();;
            //int skillPrice =
            //    (int)Mathf.Ceil(playerSkillQuality.price *
            //                    (GetPlayerData().playerData.buySkillRatios / 10000f));

            //skillPrice = math.max(skillPrice, 0);

            //int curMoney = GetPlayerMoney();
        }

        private void UpdateFreeBuyDisplay(UIPanel_SkillItem ui)
        {
            var freebuyCount = GetPlayerData().playerData.skillFreeBuyCount +
                               GetPlayerData().playerOtherData.skillTempFreeBuyCount;
            if (freebuyCount > 0)
            {
                ui.GetFromReference(UIPanel_SkillItem.KBuySkillForMoney).SetActive(false);
                ui.GetFromReference(UIPanel_SkillItem.KBuySkillForFree).SetActive(true);
                ui.GetFromReference(UIPanel_SkillItem.KTxt_consumeFree).GetTextMeshPro()
                    .SetTMPText(language.GetOrDefault("common_free_text").current);
            }
            else
            {
                ui.GetFromReference(UIPanel_SkillItem.KBuySkillForMoney).SetActive(true);
                ui.GetFromReference(UIPanel_SkillItem.KBuySkillForFree).SetActive(false);
            }
        }


        /// <summary>
        /// 刷新x次技能获得一个随机的蓝色技能
        /// </summary>
        /// <param name="skills"></param>
        private void DropSkillForRrefresh(List<int> skills, UIPanel_RunTimeHUD parentUI)
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            //var playerData = entityManager.GetComponentData<PlayerData>(player);
            var gameEvents = entityManager.GetBuffer<GameEvent>(player, true);

            int refreshCount = 0;
            int needCout = 0;
            int addition = 0;
            foreach (var gameEvent in gameEvents)
            {
                if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_58)
                {
                    if (gameEvent.Boolean_11)
                    {
                        refreshCount = gameEvent.int3_7.x;
                        needCout = gameEvent.int3_7.y;
                        addition = gameEvent.int3_7.z;
                        break;
                    }
                }
            }

            if (refreshCount > 0)
            {
                if (parentUI.refreshSkillCount % refreshCount != 0) return;


                var tbBindingList = player_skill_binding.DataList;


                List<int> skillCanSelect = new List<int>();
                List<int> skillCanNotSelect = new List<int>();

                var skillConfig = player_skill.DataList;
                for (int i = 0; i < skills.Count; i++)
                {
                    int noselect1 = skills[i];
                    for (int j = 0; j < 5; j++)
                    {
                        //已经刷新出来的三个技能相关的所有等级都不能学
                        skillCanNotSelect.Add(noselect1 + j);
                    }
                }

                var uniqueSkills = skillConfig.Where(skill => !skillCanNotSelect.Contains(skill.id)).ToList();

                for (int i = 0; i < uniqueSkills.Count; i++)
                {
                    //如果已学技能
                    if (parentUI.skillsDic.ContainsKey(uniqueSkills[i].id))
                    {
                        int quality = 0;
                        for (int j = 0; j < skillConfig.Count; j++)
                        {
                            if (skillConfig[j].id == uniqueSkills[i].id)
                            {
                                quality = skillConfig[j].skillQualityId;
                                break;
                            }
                        }

                        if (quality == 1 && parentUI.skillsDic[uniqueSkills[i].id] < 4)
                        {
                            skillCanSelect.Add(uniqueSkills[i].id + parentUI.skillsDic[uniqueSkills[i].id]);
                        }
                    }
                    else
                    {
                        if (uniqueSkills[i].level == 1 && uniqueSkills[i].skillQualityId == 1)
                        {
                            skillCanSelect.Add(uniqueSkills[i].id);
                        }
                    }
                }

                foreach (var skill in skillCanSelect)
                {
                    Log.Debug($"id:{skill}");
                }

                if (Random.Range(1, 10001) <= addition)
                {
                    needCout++;
                }


                for (int i = 0; i < needCout; i++)
                {
                    int randomValue = Random.Range(0, skillCanSelect.Count);
                    int skillID = skillCanSelect[randomValue];
                    skillBuffer = entityManager.GetBuffer<Skill>(player);
                    AddNewSkil(skillID, 1);

                    AddLogicSkill(skillID);
                    skillCanSelect.RemoveAt(randomValue);
                }
            }
        }

        private void ChangeWeaponSkillID(int bindingId, int lastLevel, int curLevel, EntityQuery playerQuery,
            EntityManager entityManager)
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            var weaponID = playerData.playerOtherData.weaponSkillId;
            var weaponAddtionID = playerData.playerOtherData.weaponAdditionID;
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            if (!parentUI.bindingsDic.TryGetValue(bindingId, out var exp))
            {
                Log.Error($"没有该bindingId");
            }

            if (curLevel - lastLevel > 0)
            {
                if (lastLevel == 0)
                {
                    var weaponIdAddition = player_skill_binding.Get(bindingId).skillId;
                    AddNewSkil(weaponIdAddition, 0);
                    switch (bindingId)
                    {
                        case 1:
                            weaponAddtionID.x = weaponIdAddition;
                            break;
                        case 2:
                            weaponAddtionID.y = weaponIdAddition;
                            break;
                        case 3:
                            weaponAddtionID.z = weaponIdAddition;
                            break;
                        case 4:
                            weaponAddtionID.w = weaponIdAddition;
                            break;
                    }


                    Log.Error($"lastLevel == 0,weaponID:{weaponIdAddition}");
                }
                else if (lastLevel > 0 && lastLevel < 3)
                {
                    int weaponIdAddition = 0;
                    switch (bindingId)
                    {
                        case 1:
                            weaponIdAddition = weaponAddtionID.x;
                            break;
                        case 2:
                            weaponIdAddition = weaponAddtionID.y;
                            break;
                        case 3:
                            weaponIdAddition = weaponAddtionID.z;
                            break;
                        case 4:
                            weaponIdAddition = weaponAddtionID.w;
                            break;
                    }

                    var newWeaponBuffID = weaponIdAddition + 1;
                    RemoveOldSkill(weaponIdAddition);
                    AddNewSkil(newWeaponBuffID, 1);
                    switch (bindingId)
                    {
                        case 1:
                            weaponAddtionID.x = newWeaponBuffID;
                            break;
                        case 2:
                            weaponAddtionID.y = newWeaponBuffID;
                            break;
                        case 3:
                            weaponAddtionID.z = newWeaponBuffID;
                            break;
                        case 4:
                            weaponAddtionID.w = newWeaponBuffID;
                            break;
                    }

                    Log.Error($"lastLevel > 0 && lastLevel < 3,weaponID:{newWeaponBuffID}");
                }
                else
                {
                    if (!parentUI.isEvolve)
                    {
                        var newWeaponSkillID = weaponID + 4 + bindingId * 10000;


                        if (player_skill.Get(newWeaponSkillID).replaceSkillId != 0)
                        {
                            RemoveOldSkill(weaponID);
                        }

                        AddNewSkil(newWeaponSkillID, 0);
                        weaponID = newWeaponSkillID;
                        parentUI.isEvolve = true;
                        Log.Error($"lastLevel > 3,weaponID:{newWeaponSkillID}");
                    }
                }


                playerData = entityManager.GetComponentData<PlayerData>(player);
                playerData.playerOtherData.weaponSkillId = weaponID;
                playerData.playerOtherData.weaponAdditionID = weaponAddtionID;
                entityManager.SetComponentData(player, playerData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="currentLevel"></param>
        public void AddOrReplaceBindingSkill(int skillId, int currentLevel)
        {
            var addSkillID = skillId + currentLevel;
            Debug.LogError($"skillId:{skillId},currentLevel:{currentLevel}");
            Debug.LogError($"addSkillID:{addSkillID}");
            if (currentLevel > 0)
            {
                var curSkillID = addSkillID - 1;
                RemoveOldSkill(curSkillID);
                Debug.LogError($"curSkillID:{curSkillID}");
            }

            AddNewSkil(addSkillID, 1);
        }

        /// <summary>
        /// 添加新技能 包含商店技能
        /// </summary>
        /// <param name="newSkillID"></param>
        /// <param name="skilltype"></param>
        private void AddNewSkil(int newSkillID, int skilltype = 1)
        {
            if (player_skill.Get(newSkillID).battleShopSkill == 1)
            {
                var playerData = entityManager.GetComponentData<PlayerData>(player);

                int element = 0;
                var list = skill_effect.DataList;
                foreach (var item in list)
                {
                    if (item.skillId == newSkillID && item.elementList.Count > 0)
                    {
                        element = item.elementList[0];
                        break;
                    }
                }

                switch (skill_effectElement.Get(element).attrId)
                {
                    case 214200:
                        playerData.playerData.refreshShopRatios += skill_effectElement.Get(element).attrIdPara[0];
                        break;
                    case 214100:
                        playerData.playerData.buySkillRatios += skill_effectElement.Get(element).attrIdPara[0];
                        break;
                    case 214700:
                        playerData.playerData.skillTempRefreshCount += skill_effectElement.Get(element).attrIdPara[0];
                        break;
                }

                entityManager.SetComponentData(player, playerData);
            }
            else
            {
                if (player_skill.Get(newSkillID).chargedSkill.Count > 0)
                {
                    Debug.LogError("player_skill.Get(newSkillID).chargedSkill.Count > 0");
                    int chargeSkillID = (int)player_skill.Get(newSkillID).chargedSkill[0].y;
                    skillBuffer = entityManager.GetBuffer<Skill>(player);
                    skillBuffer.Add(new Skill
                        { CurrentTypeId = Skill.TypeId.Skill_1, Int32_0 = chargeSkillID, Int32_10 = skilltype });
                }

                skillBuffer = entityManager.GetBuffer<Skill>(player);
                skillBuffer.Add(new Skill
                    { CurrentTypeId = Skill.TypeId.Skill_1, Int32_0 = newSkillID, Int32_10 = skilltype });
            }
        }

        private void RemoveOldSkill(int oldSkillID)
        {
            if (player_skill.Get(oldSkillID).battleShopSkill == 1)
            {
                var playerData = entityManager.GetComponentData<PlayerData>(player);

                int element = 0;
                var list = skill_effect.DataList;
                foreach (var item in list)
                {
                    if (item.skillId == oldSkillID && item.elementList.Count > 0)
                    {
                        element = item.elementList[0];
                        break;
                    }
                }

                switch (skill_effectElement.Get(element).attrId)
                {
                    case 214200:
                        playerData.playerData.refreshShopRatios -= skill_effectElement.Get(element).attrIdPara[0];
                        break;
                    case 214100:
                        playerData.playerData.buySkillRatios -= skill_effectElement.Get(element).attrIdPara[0];
                        break;
                }

                entityManager.SetComponentData(player, playerData);
            }
            else
            {
                //移除旧技能
                skillBuffer = entityManager.GetBuffer<Skill>(player);
                for (int i = 0; i < skillBuffer.Length; i++)
                {
                    if (skillBuffer[i].Int32_0 == oldSkillID)
                    {
                        skillBuffer.RemoveAt(i);
                        break;
                    }
                }

                //移除旧trigger
                var triggers = entityManager.GetBuffer<Trigger>(player);
                for (int i = 0; i < triggers.Length; i++)
                {
                    if (triggers[i].Int32_10 == oldSkillID)
                    {
                        triggers.RemoveAt(i);
                        //var curTrigger = triggers[i];
                        //curTrigger.Boolean_22 = true;
                        //curTrigger.Boolean_11 = true;
                        //triggers[i] = curTrigger;
                    }
                }

                //移除旧元素buff
                var buffs = entityManager.GetBuffer<Buff>(player);
                for (int i = 0; i < buffs.Length; i++)
                {
                    if (buffs[i].Int32_20 == oldSkillID)
                    {
                        var buff = buffs[i];
                        buff.Boolean_4 = false;
                        buff.Single_3 = 0f;
                        buffs[i] = buff;
                    }
                }

                //移除旧事件
                var gameevents = entityManager.GetBuffer<GameEvent>(player);
                for (int i = 0; i < gameevents.Length; i++)
                {
                    if (gameevents[i].Int32_15 == oldSkillID)
                    {
                        var gameEvent = gameevents[i];
                        gameEvent.Boolean_6 = false;
                        gameEvent.Single_4 = 0f;
                        gameevents[i] = gameEvent;
                    }
                }
            }
        }

        private void GetBindingSkillFromEvent(List<int> skills)
        {
            var tbBindingList = player_skill_binding.DataList;
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            int maxValue = parentUI.bindingsDic.Max(pair => pair.Value);
            NativeList<int> skillCanSelect = new NativeList<int>(Allocator.Temp);
            NativeList<int> skillCanNotSelect = new NativeList<int>(Allocator.Temp);

            var skillList = player_skill.DataList;
            for (int i = 0; i < skills.Count; i++)
            {
                int noselect1 = skills[i];
                for (int j = 0; j < 5; j++)
                {
                    skillCanNotSelect.Add(noselect1 + j);
                }
            }

            var uniqueSkills = skillList.Where(num => !skillCanNotSelect.Contains(num.id)).ToList();
            for (int i = 0; i < uniqueSkills.Count; i++)
            {
                if (parentUI.skillsDic.ContainsKey(uniqueSkills[i].id))
                {
                    skillCanSelect.Add(uniqueSkills[i].id + parentUI.skillsDic[uniqueSkills[i].id] * 10);
                }
                else
                {
                    if (uniqueSkills[i].level == 1)
                    {
                        skillCanSelect.Add(uniqueSkills[i].id);
                    }
                }
            }

            int randomValue = Random.Range(0, skillCanSelect.Length);
            int skillID = skillCanSelect[randomValue];
            //var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            AddNewSkil(skillID, 1);
            AddLogicSkill(skillID);
        }

        /// <summary>
        /// 添加逻辑技能 无ui表现
        /// </summary>
        private void AddLogicSkill(int skillId)
        {
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            int lastBindingLevel = 0;
            int currentBindingLevel = 0;
            int currentLevel = 0;
            if (parentUI.skillsDic.ContainsKey(skillId))
            {
                currentLevel = parentUI.skillsDic[skillId];
            }

            var curSkill = player_skill.Get(skillId + currentLevel);
            var bindingId = curSkill.skillBindingId[0];
            var playerSkillQuality = player_skill_quality.Get(curSkill.skillQualityId);
            var playerSkillBinding = player_skill_binding.Get(bindingId);
            var bindingExp = playerSkillQuality.bindingExp;


            if (parentUI.bindingsDic.ContainsKey(bindingId))
            {
                int lastexp = parentUI.bindingsDic[bindingId];
                lastBindingLevel = JiYuUIHelper.GetBindingLevel(lastexp);
            }


            if (parentUI.bindingsDic.ContainsKey(bindingId))
            {
                {
                    parentUI.bindingsDic[bindingId] += bindingExp;

                    if (parentUI.bindingsDic.ContainsKey(parentUI.maxExpBindingId))
                    {
                        if (parentUI.bindingsDic[bindingId] > parentUI.bindingsDic[parentUI.maxExpBindingId])
                        {
                            parentUI.maxExpBindingId = bindingId;
                        }
                    }
                    else
                    {
                        parentUI.maxExpBindingId = bindingId;
                    }

                    //int curexp = parentUI.bindingsDic[bindingId];
                    //curLevel = JiYuUIHelper.GetBindingLevel(curexp);
                    //isLevelUp = curLevel - lastLevel >= 1 ? true : false;
                }
            }

            if (parentUI.bindingsDic.ContainsKey(bindingId))
            {
                int lastexp = parentUI.bindingsDic[bindingId];
                currentBindingLevel = JiYuUIHelper.GetBindingLevel(lastexp);
            }

            //1.替换or增加羁绊技能
            //2.武器技能根据阶级产生形变
            //TODO：
            AddOrReplaceBindingSkill(skillId, currentLevel);
            ChangeWeaponSkillID(bindingId, lastBindingLevel, currentLevel, playerQuery, entityManager);

            if (parentUI.skillsDic.ContainsKey(skillId))
            {
                parentUI.skillsDic[skillId]++;
            }
            else
            {
                parentUI.skillsDic.Add(skillId, 1);
            }

            CreateBindingItem().Forget();
        }

        private void OnSkillTriggerForClose()
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var skills = entityManager.GetBuffer<Skill>(player);
            var gameEvents = entityManager.GetBuffer<GameEvent>(player);
            for (int j = 0; j < gameEvents.Length; j++)
            {
                var gameEvent = gameEvents[j];
                if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_59 && gameEvent.Boolean_11)
                {
                    var uis = panelRunTimeHUD as UIPanel_RunTimeHUD;
                    var dic = uis.skillsDic;
                    var list = ConfigManager.Instance.Tables.Tbskill.DataList;
                    int skillCount = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].type == 2 && list[i].level == 1)
                        {
                            if (dic.ContainsKey(list[i].id))
                            {
                                skillCount++;
                            }
                        }
                    }

                    Log.Debug($"skillcount:{skillCount},oldcount:{gameEvent.int3_7.z}");
                    var chaStatesRW = entityManager.GetComponentData<ChaStats>(player);
                    var playerDataRW = entityManager.GetComponentData<PlayerData>(player);
                    UnityHelper.RemoveChangeProperty(ref chaStatesRW, ref playerDataRW,
                        (int)gameEvent.int3_7.y,
                        gameEvent.int3_7.z * gameEvent.int3_7.x);
                    UnityHelper.ChangeProperty(ref chaStatesRW, ref playerDataRW,
                        (int)gameEvent.int3_7.y,
                        gameEvent.int3_7.x * skillCount);
                    gameEvent.int3_7.z = skillCount;
                    gameEvents[j] = gameEvent;
                }
            }


            gameEvents = entityManager.GetBuffer<GameEvent>(player);
            for (int j = 0; j < gameEvents.Length; j++)
            {
                var gameEvent = gameEvents[j];
                if (gameEvent.CurrentTypeId == (GameEvent.TypeId)41 && gameEvent.Boolean_11)
                {
                    skills.Add(new Skill { Int32_0 = gameEvent.int3_7.x, Entity_5 = player, Int32_10 = 1 });
                    break;
                }
                else if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_43 && gameEvent.Boolean_11)
                {
                    var playerDataRW = entityManager.GetComponentData<PlayerData>(player);
                    playerDataRW.playerOtherData.skillTempFreeBuyCount = 0;
                }
            }
            //skills.Add(new Skill { Int32_0 =});
        }

        private void OnSkillTriggerForOpen(Entity player, ref DynamicBuffer<Skill> skills)
        {
            var gameEvents = entityManager.GetBuffer<GameEvent>(player);
            foreach (var gameEvent in gameEvents)
            {
                if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_42 && gameEvent.Boolean_11)
                {
                    Log.Debug("GameEvent.TypeId.GameEvent_42", Color.cyan);
                    BuffHelper.AddSkillByDcb(entityManager, gameEvent.int3_7.x, player, 1);
                    //break;
                }
                else if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_43 && gameEvent.Boolean_11)
                {
                    var playerDataRW = entityManager.GetComponentData<PlayerData>(player);
                    playerDataRW.playerOtherData.skillTempFreeBuyCount += (int)gameEvent.int3_7.x;
                }
                else if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_39 && gameEvent.Boolean_11)
                {
                    var playerDataRW = entityManager.GetComponentData<PlayerData>(player);
                    playerDataRW.playerData.skillRefreshCount += (int)gameEvent.int3_7.x;
                    //DisplayFreeRefresh();
                }
            }
            //skills.Add(new Skill { Int32_0 =});
        }

        private void UpdatePlayerData(UIPanel_RunTimeHUD parentUI, out int needCount, out int property)
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            //var playerData = entityManager.GetComponentData<PlayerData>(player);
            var gameEvents = entityManager.GetBuffer<GameEvent>(player, true);
            bool isNeedCount = false;
            needCount = 0;
            property = 0;
            foreach (var gameEvent in gameEvents)
            {
                if (gameEvent.CurrentTypeId == (GameEvent.TypeId)40)
                {
                    if (gameEvent.Boolean_11)
                    {
                        isNeedCount = true;
                        needCount = gameEvent.int3_7.x;
                        property = gameEvent.int3_7.y;
                        break;
                    }
                }
            }

            if (isNeedCount)
            {
                parentUI.buySkillCount++;
            }
        }

        void LockSkillsItem(bool isLock)
        {
            var KContainer_Skill = GetFromReference(UIPanel_BattleShop.KContainer_Skill);
            var list = KContainer_Skill.GetList();
            foreach (var ui in list.Children)
            {
                var uis = ui as UIPanel_SkillItem;

                var KImg_IsLock = uis.GetFromReference(UIPanel_SkillItem.KImg_IsLock);
                var KImg_LockMask = uis.GetFromReference(UIPanel_SkillItem.KImg_LockMask);
                var KImg_Mask = uis.GetFromReference(UIPanel_SkillItem.KImg_Mask);
                if (KImg_Mask.GameObject.activeSelf)
                {
                    continue;
                }

                KImg_IsLock.SetActive(isLock);
                //KImg_LockMask.SetActive(isLock);
                //uis.GetXButton().SetEnabled(!isLock);
            }
        }

        void SetStars(UIListComponent itemList, int levelUpperlimit, int currentLevel, int qulityId)
        {
            for (int j = 0; j < MaxStars; j++)
            {
                var index = j;
                Transform child = itemList.Get().GetChild(index);
                var uiItem = itemList.Create(child.gameObject, true);
                uiItem.SetActive(false);
            }

            for (int j = 0; j < levelUpperlimit; j++)
            {
                var index = j;
                Transform child = itemList.Get().GetChild(index);
                var uiItem = itemList.Create(child.gameObject, true);
                uiItem.SetActive(true);
                if (index <= currentLevel - 1)
                {
                    //TODO:需要根据品质改星星颜色
                    if (qulityId == 1)
                    {
                        uiItem.GetImage().SetSpriteAsync("icon_star_blue", false).Forget();
                    }
                    else if (qulityId == 2)
                    {
                        uiItem.GetImage().SetSpriteAsync("icon_star_pink", false).Forget();
                    }
                    else if (qulityId == 3)
                    {
                        uiItem.GetImage().SetSpriteAsync("icon_star_yellow", false).Forget();
                    }
                }
                else
                {
                    uiItem.GetImage().SetSpriteAsync("icon_star_gray", false).Forget();
                }
            }
        }

        private async UniTaskVoid CreateBindingItem()
        {
            var KContainer_Binding = GetFromReference(UIPanel_BattleShop.KContainer_Binding);
            var KContainer_Selcted = GetFromReference(UIPanel_BattleShop.KContainer_Selcted);
            var bindingList = KContainer_Binding.GetList();
            bindingList.Clear();
            var tbBindingList = player_skill_binding.DataList;

            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            int maxValue = parentUI.bindingsDic.Max(pair => pair.Value);

            int keyWithMaxValue = parentUI.bindingsDic.FirstOrDefault(pair => pair.Value == maxValue).Key;


            int queenEnable = 0;
            for (int i = 0; i < tbBindingList.Count; i++)
            {
                var index = i;
                var bindingID = tbBindingList[index].id;

                if (!parentUI.bindingsDic.TryGetValue(bindingID, out var exp))
                {
                    Log.Error($"没有该bindingId");
                }

                var ui =
                    await bindingList.CreateWithUITypeAsync(UIType.UIPanel_BindingItem, bindingID, false) as
                        UIPanel_BindingItem;

                ui.id = bindingID;
                var KImg_BindingIcon = ui.GetFromReference(UIPanel_BindingItem.KImg_BindingIcon);
                var KText_Stag = ui.GetFromReference(UIPanel_BindingItem.KText_Stag);
                var KContainer_Stags = ui.GetFromReference(UIPanel_BindingItem.KContainer_Stags);
                var KImg_Queen = ui.GetFromReference(UIPanel_BindingItem.KImg_Queen);
                var KBg_IsSelected = ui.GetFromReference(UIPanel_BindingItem.KBg_IsSelected);

                var itemList = KContainer_Stags.GetList();
                KBg_IsSelected.SetActive(false);
                SetStagsColor(exp, itemList);
                var bindingLevel = JiYuUIHelper.GetBindingLevel(exp);
                int nextLevelexp = 0;
                if (bindingLevel == player_skill_binding_rank.DataList[player_skill_binding_rank.DataList.Count - 1].id)
                {
                    nextLevelexp = player_skill_binding_rank.Get(bindingLevel).exp;
                }
                else
                {
                    nextLevelexp = player_skill_binding_rank.Get(bindingLevel + 1).exp;
                }


                KText_Stag.GetTextMeshPro().SetTMPText($"{exp}/{nextLevelexp}");
                KImg_Queen.SetActive(false);

                //parentUI.maxExpBindingId
                if (exp != 0 && exp == maxValue && parentUI.maxExpBindingId == bindingID)
                {
                    KImg_Queen.SetActive(true);
                }

                var strImg = player_skill_binding.Get(bindingID).pic + "_outline";
                KImg_BindingIcon.GetImage().SetSpriteAsync(strImg, false).Forget();
                //uiItem3.GetImage()
                //itemList.GetChildAt(3).SetActive(true);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(ui, () =>
                {
                    JiYuUIHelper.DestoryAllTips();
                    ;
                    if (KBg_IsSelected.GameObject.activeSelf)
                    {
                        KContainer_Selcted.SetActive(false);
                        KBg_IsSelected.SetActive(false);
                        strImg = player_skill_binding.Get(bindingID).pic + "_outline";
                        KImg_BindingIcon.GetImage().SetSpriteAsync(strImg, false).Forget();
                        return;
                    }

                    KImg_BindingIcon.GetImage().SetSpriteAsync(strImg, false).Forget();
                    DisableAllStageIsSelected();
                    KBg_IsSelected.SetActive(true);
                    OnClickBindings(bindingID).Forget();

                    KContainer_Selcted.SetActive(true);
                    JiYuUIHelper.ForceRefreshLayout(GetFromReference(KMid));
                    //TODO:
                });
            }

            bindingList.Sort((a, b) =>
            {
                var aui = a as UIPanel_BindingItem;
                var bui = b as UIPanel_BindingItem;
                return aui.id.CompareTo(bui.id);
            });
            //JiYuUIHelper.ForceRefreshLayout(KContainer_Binding);
        }

        struct BattleShopDrop
        {
            public battleshop_drop battleshop_drop;
            public int power;
        }

        int GetOneSkill(PlayerData playerData, int poolId, out int canSelectedcount)
        {
            canSelectedcount = default;
            //int num = constant.Get("battleshop_refresh_num").constantValue;
            var list = new List<BattleShopDrop>();
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            foreach (var item in battleshop_drop.DataList)
            {
                if (item.id == poolId && item.power > 0)
                {
                    int power = 0;
                    switch (item.quality)
                    {
                        case 1:
                            power = Mathf.CeilToInt(item.power *
                                                    (1 + playerData.playerData.skillWeightIncrease1 / 10000f));
                            break;
                        case 2:
                            power = Mathf.CeilToInt(item.power *
                                                    (1 + playerData.playerData.skillWeightIncrease2 / 10000f));
                            break;
                        case 3:
                            power = Mathf.CeilToInt(item.power *
                                                    (1 + playerData.playerData.skillWeightIncrease3 / 10000f));
                            break;
                    }

                    list.Add(new BattleShopDrop
                    {
                        battleshop_drop = item,
                        power = power
                    });
                }
            }

            //可选技能数量
            int canSelectSkillCount = 0;
            foreach (var pool in list)
            {
                var poolQuality = pool.battleshop_drop.quality;
                int maxLevel = player_skill_quality.Get(poolQuality).levelUpperlimit;
                List<skill> poolQualitySkills =
                    player_skill.DataList.Where(skill => skill.skillQualityId == poolQuality).ToList();
                foreach (var skill in poolQualitySkills)
                {
                    if (!(parentUI.skillsDic.ContainsKey(skill.id) && parentUI.skillsDic[skill.id] == maxLevel))
                    {
                        canSelectSkillCount++;
                    }
                }
            }

            if (canSelectSkillCount == 0)
            {
                return -1;
            }

            int totalPower = 0;
            foreach (var item in list)
            {
                totalPower += item.power;
            }

            int quality = 1;
            int randomValue = Random.Range(0, totalPower);
            int weightSum = 0;
            foreach (var item in list)
            {
                weightSum += item.power;
                // 检查随机值是否在当前物品的权重范围内
                if (randomValue <= weightSum)
                {
                    quality = item.battleshop_drop.quality;
                    break;
                }
            }

            List<skill> randomBeginSkills =
                player_skill.DataList.Where(skill =>
                {
                    bool canSelected = true;
                    // bool qualityValid = true;
                    //
                    // if (skill.type == 2 && skill.level == 1 &&
                    //     parentUI.bindingsDic.TryGetValue(skill.skillBindingId[0], out var exp))
                    // {
                    //     int bindingId = JiYuUIHelper.GetBindingLevel(exp);
                    //
                    //     int maxQualityId = 1;
                    //     for (int i = player_skill_quality.DataList.Count - 1; i >= 0; i--)
                    //     {
                    //         var skillQuality = player_skill_quality.DataList[i];
                    //
                    //         if (skillQuality.unlockRank <= bindingId)
                    //         {
                    //             maxQualityId = skillQuality.id;
                    //             break;
                    //         }
                    //     }
                    //
                    //     //Log.Debug($"skill{skill.id}bindingId{bindingId} quality{quality} maxQualityId{maxQualityId}");
                    //     if (quality > maxQualityId)
                    //     {
                    //         qualityValid = false;
                    //     }
                    // }

                    if (parentUI.skillsDic.TryGetValue(skill.id, out int grade))
                    {
                        int maxLevel = player_skill_quality.Get(skill.skillQualityId).levelUpperlimit;
                        if (grade >= maxLevel)
                        {
                            canSelected = false;
                        }
                    }

                    return skill.type == 2 && skill.level == 1 && canSelected;
                    //list.Any(x => x.quality == skill.skillQualityId);
                }).ToList();


            if (randomBeginSkills.Count <= 0)
            {
                Log.Error($"randomSkills0.Count 0 技能池没有符合的技能");
                return -1;
            }

            List<skill> randomSkills =
                player_skill.DataList.Where(skill =>
                {
                    bool canSelected = true;
                    bool qualityValid = true;

                    if (skill.type == 2 && skill.level == 1 &&
                        parentUI.bindingsDic.TryGetValue(skill.skillBindingId[0], out var exp))
                    {
                        int bindingId = JiYuUIHelper.GetBindingLevel(exp);

                        int maxQualityId = 1;
                        for (int i = player_skill_quality.DataList.Count - 1; i >= 0; i--)
                        {
                            var skillQuality = player_skill_quality.DataList[i];

                            if (skillQuality.unlockRank <= bindingId)
                            {
                                maxQualityId = skillQuality.id;
                                break;
                            }
                        }

                        //Log.Debug($"skill{skill.id}bindingId{bindingId} quality{quality} maxQualityId{maxQualityId}");
                        if (quality > maxQualityId)
                        {
                            qualityValid = false;
                        }
                    }

                    if (parentUI.skillsDic.TryGetValue(skill.id, out int grade))
                    {
                        int maxLevel = player_skill_quality.Get(skill.skillQualityId).levelUpperlimit;
                        if (grade >= maxLevel)
                        {
                            canSelected = false;
                        }
                    }

                    return skill.type == 2 && skill.level == 1 && canSelected && qualityValid &&
                           skill.skillQualityId == quality;

                    //list.Any(x => x.quality == skill.skillQualityId);
                }).ToList();

            // List<skill_group> randomSkills0 =
            //     player_skill_group.DataList.Where(skill =>
            //     {
            //         bool canSelected = true;
            //         var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            //         if (parentUI.skillsDic.TryGetValue(skill.id, out int grade))
            //         {
            //             int maxLevel = player_skill_quality.Get(skill.skillQualityId).levelUpperlimit;
            //             if (grade >= maxLevel)
            //             {
            //                 canSelected = false;
            //             }
            //         }
            //
            //         return skill.type == 2 && canSelected && list.Any(x => x.quality == skill.skillQualityId);
            //     }).ToList();


            //canSelectedcount = randomSkills.Count;

            // if (randomSkills.Count <= 0)
            // {
            //     Log.Error($"randomSkills0.Count -1");
            //     return -1;
            // }

            // List<skill> randomSkills =
            //     randomSkills0.Where(skill => { return skill.skillQualityId == quality; }).ToList();
            //Log.Error($"randomSkills {randomSkills.Count}");
            if (randomSkills.Count <= 0)
            {
                //Log.Error($"randomSkills0.Count 0 当前随机失败");
                return 0;
            }

            randomSkills = randomSkills.OrderBy(x => Random.value).ToList();

            var randomSkill = randomSkills[0];
            return randomSkill.id;

            //Log.Error($"没有符合条件的技能");
        }

        int GetOneSkillJudge(int poolId, List<int> currentRandomedSkills)
        {
            var playerData = GetPlayerData();
            //int num = constant.Get("battleshop_refresh_num").constantValue;

            for (int i = 0; i < 488; i++)
            {
                int before = GetOneSkill(playerData, poolId, out var canSelectedcount);

                if (before == -1)
                    return -1;

                if (before == 0)
                    continue;

                int count = currentRandomedSkills.Count(x => x == before);
                if (count <= 0)
                {
                    return before;
                }

                var playerSkillGroup = player_skill.Get(before);
                int maxLevel = player_skill_quality.Get(playerSkillGroup.skillQualityId).levelUpperlimit;
                var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
                int currentLevel = 0;
                if (parentUI.skillsDic.ContainsKey(before))
                {
                    currentLevel = parentUI.skillsDic[before];
                }

                if (count + currentLevel < maxLevel)
                {
                    return before;
                }
            }

            Debug.LogError($"pools upper!循环上限 池子里没有满足的技能 没选上");
            return -1;
        }


        void SetStagsColor(int exp, UIListComponent itemList)
        {
            Transform child0 = itemList.Get().GetChild(0);
            Transform child1 = itemList.Get().GetChild(1);
            Transform child2 = itemList.Get().GetChild(2);
            Transform child3 = itemList.Get().GetChild(3);
            var uiItem0 = itemList.Create(child0.gameObject, true).GetImage();
            var uiItem1 = itemList.Create(child1.gameObject, true).GetImage();
            var uiItem2 = itemList.Create(child2.gameObject, true).GetImage();
            var uiItem3 = itemList.Create(child3.gameObject, true).GetImage();
            //5E6B68 466B9E 815586 9E7749 975950
            uiItem0.SetColor("5E6B68");
            uiItem1.SetColor("5E6B68");
            uiItem2.SetColor("5E6B68");
            uiItem3.SetColor("5E6B68");

            var index = JiYuUIHelper.GetBindingLevel(exp);

            switch (index)
            {
                case 0:

                    break;
                case 1:
                    uiItem0.SetColor("466B9E");

                    break;
                case 2:
                    uiItem0.SetColor("815586");
                    uiItem1.SetColor("815586");

                    break;
                case 3:
                    uiItem0.SetColor("9E7749");
                    uiItem1.SetColor("9E7749");
                    uiItem2.SetColor("9E7749");
                    break;
                case 4:
                    uiItem0.SetColor("975950");
                    uiItem1.SetColor("975950");
                    uiItem2.SetColor("975950");
                    uiItem3.SetColor("975950");
                    break;
            }
        }

        public void InitJson()
        {
            constant = ConfigManager.Instance.Tables.Tbbattle_constant;
            //battle_exp = ConfigManager.Instance.Tables.Tbbattle_exp;
            language = ConfigManager.Instance.Tables.Tblanguage;
            battleshop_stage = ConfigManager.Instance.Tables.Tbbattleshop_stage;
            player_skill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            player_skill_binding_rank = ConfigManager.Instance.Tables.Tbskill_binding_rank;
            levelConfig = ConfigManager.Instance.Tables.Tblevel;
            user_variable = ConfigManager.Instance.Tables.Tbuser_variable;
            battleshop_drop = ConfigManager.Instance.Tables.Tbbattleshop_drop;
            player_skill_quality = ConfigManager.Instance.Tables.Tbskill_quality;
            player_skill = ConfigManager.Instance.Tables.Tbskill;
            skill_effect = ConfigManager.Instance.Tables.Tbskill_effectNew;
            skill_effectElement = ConfigManager.Instance.Tables.Tbskill_effectElement;
            tbguide = ConfigManager.Instance.Tables.Tbguide;
        }
        
        private void ResetTempFreeCount()
        {
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            playerData.playerData.skillTempRefreshCount = 0;
            entityManager.SetComponentData<PlayerData>(player, playerData);
        }
        protected override void OnClose()
        {
            ResetTempFreeCount();
            OnSkillTriggerForClose();
            cts.Cancel();
            cts.Dispose();
            base.OnClose();
        }
    }
}