//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using Main;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_EquipTips)]
    internal sealed class UICommon_EquipTipsEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_EquipTips;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_EquipTips>();
        }
    }

    public partial class UICommon_EquipTips : UI, IAwake<MyGameEquip>
    {
        private GameEquip _gameEquip;
        private long timerId;
        public Vector3 reward;

        public async void Initialize(MyGameEquip myGameEquip)
        {
            var attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;
            var equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;
            var player_skill = ConfigManager.Instance.Tables.Tbskill;
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;

            var KText_EquipName = GetFromReference(UICommon_EquipTips.KText_EquipName);
            var KText_Level = GetFromReference(UICommon_EquipTips.KText_Level);
            var KText_LevelNum = GetFromReference(UICommon_EquipTips.KText_LevelNum);
            var KText_Atk = GetFromReference(UICommon_EquipTips.KText_Atk);
            var KText_AtkNum = GetFromReference(UICommon_EquipTips.KText_AtkNum);
            var KText_LevelupCost = GetFromReference(UICommon_EquipTips.KText_LevelupCost);
            var KText_Equip = GetFromReference(UICommon_EquipTips.KText_Equip);
            var KText_LevelUp = GetFromReference(UICommon_EquipTips.KText_LevelUp);
            var KText_AllLevelUp = GetFromReference(UICommon_EquipTips.KText_AllLevelUp);
            var KItemPos = GetFromReference(UICommon_EquipTips.KItemPos);
            var KImg_Left = GetFromReference(UICommon_EquipTips.KImg_Left);
            var KContentEquipTips = GetFromReference(UICommon_EquipTips.KContentEquipTips);
            var KBtn_Close = GetFromReference(UICommon_EquipTips.KBtn_Close);
            var KBtn_Equip = GetFromReference(UICommon_EquipTips.KBtn_Equip);
            var KBtn_LevelUp = GetFromReference(UICommon_EquipTips.KBtn_LevelUp);
            var KBtn_AllLevelUp = GetFromReference(UICommon_EquipTips.KBtn_AllLevelUp);
            var KBtn_Decrease = GetFromReference(UICommon_EquipTips.KBtn_Decrease);
            var KText_TitleInfo = GetFromReference(UICommon_EquipTips.KText_TitleInfo);
            var KImg_TopArraw = GetFromReference(UICommon_EquipTips.KImg_TopArraw);
            var KText_LevelMaxInfo = GetFromReference(UICommon_EquipTips.KText_LevelMaxInfo);
            var KImg_TopTitle = GetFromReference(UICommon_EquipTips.KImg_TopTitle);
            var KBottom = GetFromReference(UICommon_EquipTips.KBottom);
            var KImg_BottomArraw = GetFromReference(UICommon_EquipTips.KImg_BottomArraw);
            var skillList = KContentEquipTips.GetList();

            //Log.Error($"reward {myGameEquip.reward}");
            //Log.Error($"reward1 {myGameEquip.equip.EquipId}");
            if (myGameEquip.reward.x > 0)
            {
               
                reward = myGameEquip.reward;
               
                var equip_data0 = equip_dataConfig.Get((int)reward.y);
                var equip_quality0 = equip_qualityConfig.Get(equip_data0.quality);
                var quality0 = qualityConfig.Get(equip_quality0.type);
                var equipLevel0 = 1;
                var titleStr0 = JiYuUIHelper.GetRewardName(reward);
                KText_EquipName.GetTextMeshPro().SetTMPText(titleStr0);
                KText_LevelNum.SetActive(true);
                KText_LevelNum.GetTextMeshPro().SetTMPText(equipLevel0.ToString());
                KText_Level.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_info_level").current);

                switch (equip_data0.mainEntryId)
                {
                    case 103010:
                        KText_Atk.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_atk").current);
                        KImg_Left.GetImage().SetSpriteAsync("icon_equip_tips_atk", false).Forget();
                        break;
                    case 102010:
                        KText_Atk.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_hp").current);
                        KImg_Left.GetImage().SetSpriteAsync("icon_equip_tips_heart", false).Forget();
                        break;
                    case 107010:
                        KText_Atk.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_speed").current);
                        KImg_Left.GetImage().SetSpriteAsync("icon_equip_tips_move", false).Forget();
                        break;
                }

                var atkNum0 = equip_data0.mainEntryInit + (equipLevel0 - 1) * equip_data0.mainEntryGrow;
                KText_AtkNum.GetTextMeshPro().SetTMPText(atkNum0.ToString());


                var skillGroup0 = equip_data0.minorSkillGroup;

                int curQuaIndex0 = -1;
                for (int i = 0; i < skillGroup0.Count; i++)
                {
                    if ((int)skillGroup0[i].x == quality0.id)
                    {
                        curQuaIndex0 = i;
                        break;
                    }
                }

                skillList.Clear();
                for (int i = 0; i < skillGroup0.Count; i++)
                {
                    var skill = skillGroup0[i];
                    var qualityId = (int)skill.x;
                    var ui = await skillList.CreateWithUITypeAsync(UIType.UISubPanel_EquipSkill, false);
                    // var ui = await UIHelper.CreateAsync(this, UIType.UISubPanel_EquipSkill,
                    //     KContentEquipTips.GameObject.transform);
                    var img = ui.GetFromReference(UISubPanel_EquipSkill.KImg_Unlock);
                    var text = ui.GetFromReference(UISubPanel_EquipSkill.KText_Content);
                    //var mask = ui.GetFromReference(UISubPanel_EquipSkill.KMask);
                    var playerSkill = player_skill.GetOrDefault((int)skill.y);

                    string skillDes = default;
                    if (playerSkill == null)
                    {
                        skillDes = "key is not exist";
                    }
                    else
                    {
                        skillDes = string.Format(languageConfig.Get(playerSkill.desc).current,
                            playerSkill.descPara.ToArray());
                    }

                    if (curQuaIndex0 >= i)
                    {
                        img.GetImage().SetSpriteAsync(qualityConfig.Get(qualityId).unlock, true).Forget();
                        text.GetTextMeshPro().SetTMPText(skillDes);
                        text.GetTextMeshPro().SetAlpha(1);
                        img.GetImage().SetAlpha(1);
                    }
                    else
                    {
                        img.GetImage().SetSpriteAsync(qualityConfig.Get(qualityId).lock0, true).Forget();
                        text.GetTextMeshPro()
                            .SetTMPText(UnityHelper.RichTextColor(skillDes, "31324E"));
                        text.GetTextMeshPro().SetAlpha(0.5f);
                        img.GetImage().SetAlpha(0.5f);
                    }
                }

                JiYuUIHelper.ForceRefreshLayout(KContentEquipTips);

                return;
            }

            //DestorySubPanel();
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPWEARORUNWEAR, OnEquipWearResponse);
            // WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPUNWEAR, OnEquipWearResponse);
            // WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPWEAR, OnEquipWearResponse);
            var equip = myGameEquip.equip;
            Log.Debug($"{myGameEquip.equip.ToString()}", Color.green);
            // WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPUNWEAR, OnEquipWearResponse);
            // WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPWEAR, OnEquipWearResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPDOWNGRADE, OnEquipUpGradeResponse);
            WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPUPGRADE, OnEquipUpGradeResponse);

            WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPALLUPGRADE, OnEquipAllUpGradeResponse);
            //WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYPROPERTY, OnQueryPropertyResponse);
            //DestorySubPanel();


            long equipUid = equip.PartId;
            int equipId = equip.EquipId;
            int equipQuality = equip.Quality;
            int equipLevel = equip.EquipLevel;
            equipId = equip.EquipId > 10000 ? equip.EquipId : equip.EquipId * 100 + equip.Quality;
            //equipId = ;

            var equip_data = equip_dataConfig.Get(equipId);
            var equip_quality = equip_qualityConfig.Get(equip_data.quality);
            var quality = qualityConfig.Get(equip_quality.type);
            var equip_pos = equip_posConfig.Get(equip_data.posId);
            var equip_level = equip_levelConfig.Get(equipLevel);


            KImg_BottomArraw.SetActive(false);
            KImg_TopArraw.SetActive(false);
            KImg_TopTitle.SetActive(true);
            KBottom.SetActive(true);
            KBtn_Decrease.SetActive(true);

            var hex = quality.fontColor;
            var langkey = quality.name;

            var titleStr = JiYuUIHelper.GetRewardName(new Vector3(11, equipId, equipQuality), true);

            //title?.GetTextMeshPro().SetTMPText(titleStr11);
            //Log.Debug($"equipLevel{equipLevel}", Color.green);
            KText_EquipName.GetTextMeshPro().SetTMPText(titleStr);


            KText_LevelupCost.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_levelup_cost").current);
            KText_LevelUp.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_levelup").current);
            KText_AllLevelUp.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_levelupclick").current);


            KText_Level.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_info_level").current);
            KText_LevelMaxInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_levelup_max").current);
            KText_TitleInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_detail_title").current);

            if (equipLevel > 0)
            {
                KText_LevelNum.SetActive(true);
                KText_LevelNum.GetTextMeshPro().SetTMPText(equipLevel.ToString());
            }

            // JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_AllLevelUp,
            //     () => { NetWorkManager.Instance.SendMessage(CMD.EQUIPDOWNGRADE, equip); });
            bool isWearedPos = false;
            isWearedPos = ResourcesSingleton.Instance.equipmentData.isWearingEquipments.ContainsKey(equip_data.posId);
            if (!isWearedPos)
            {
                KText_Equip.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_common").current);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Equip,
                    () =>
                    {
                        //Log.Debug($"PartId{equip.PartId}", Color.green);

                        var wearReq = new WearRequest
                        {
                            Type = 1,
                            EquipIds = { equip.PartId }
                        };

                        NetWorkManager.Instance.SendMessage(CMD.EQUIPWEARORUNWEAR, wearReq);
                        //NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY);
                        ResourcesSingleton.Instance.equipmentData.isWearingEquipments.TryAdd(equip_data.posId,
                            myGameEquip);
                        ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid, out var isequip);
                        isequip.isWearing = true;
                        JiYuUIHelper.WearOrUnWearEquipProperty(isequip.equip, true);
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                        {
                            var uiscript = ui as UIPanel_Equipment;
                            uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.Wear,
                                equipUid, -1);
                            uiscript.PlayEquipAnimation(1);
                        }

                        Close();
                    });
            }
            else
            {
                bool isCurEquip = ResourcesSingleton.Instance.equipmentData.isWearingEquipments[equip_data.posId].equip
                    .PartId == equipUid;

                if (isCurEquip)
                {
                    KText_Equip.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_remove").current);

                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Equip,
                        () =>
                        {
                            //Log.Debug($"PartId{equip.PartId}", Color.green);
                            var wearReq = new WearRequest
                            {
                                Type = 2,
                                EquipIds = { equip.PartId }
                            };

                            NetWorkManager.Instance.SendMessage(CMD.EQUIPWEARORUNWEAR, wearReq);
                            //NetWorkManager.Instance.SendMessage(CMD.EQUIPUNWEAR, uid);
                            //NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY);
                            ResourcesSingleton.Instance.equipmentData.isWearingEquipments.TryRemove(equip_data.posId,
                                out var f);
                            ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid, out var isequip);
                            isequip.isWearing = false;

                            JiYuUIHelper.WearOrUnWearEquipProperty(isequip.equip, false);
                            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                            {
                                var uiscript = ui as UIPanel_Equipment;
                                uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.UnWear,
                                    equipUid, -1);
                                uiscript.PlayEquipAnimation(1);
                            }


                            Close();
                        });
                }
                else
                {
                    KText_Equip.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_change").current);
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Equip,
                        () =>
                        {
                            Log.Debug($"PartId{equip.PartId}", Color.green);
                            var lastuid = ResourcesSingleton.Instance.equipmentData
                                .isWearingEquipments[equip_data.posId]
                                .equip.PartId;
                            // LongValue uid = new LongValue()
                            // {
                            //     Value = equip.PartId
                            // };
                            // LongValue lastUID = new LongValue()
                            // {
                            //     Value = lastuid
                            // };

                            var wearReq = new WearRequest
                            {
                                Type = 3,
                                EquipIds = { equip.PartId, lastuid }
                            };

                            NetWorkManager.Instance.SendMessage(CMD.EQUIPWEARORUNWEAR, wearReq);
                            // NetWorkManager.Instance.SendMessage(CMD.EQUIPUNWEAR, lastUID);
                            // NetWorkManager.Instance.SendMessage(CMD.EQUIPWEAR, uid);
                            //NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY);
                            ResourcesSingleton.Instance.equipmentData.isWearingEquipments[equip_data.posId] =
                                myGameEquip;
                            ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(lastuid, out var isequip0);
                            isequip0.isWearing = false;
                            ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid, out var isequip);
                            isequip.isWearing = true;
                            int oldLevel = isequip0.equip.EquipLevel;
                            int newLevel = isequip.equip.EquipLevel;

                            isequip.equip.EquipLevel = oldLevel;
                            JiYuUIHelper.ChangeEquipProperty(isequip0.equip, isequip.equip);
                            isequip0.equip.EquipLevel = newLevel;

                            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                            {
                                var uiscript = ui as UIPanel_Equipment;
                                uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.Change,
                                    equipUid, lastuid);
                                uiscript.PlayEquipAnimation(1);
                            }

                            Close();
                        });
                }
            }

            //TODO:属性id
            switch (equip_data.mainEntryId)
            {
                case 103010:
                    KText_Atk.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_atk").current);
                    KImg_Left.GetImage().SetSpriteAsync("icon_equip_tips_atk", false).Forget();
                    break;
                case 102010:
                    KText_Atk.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_hp").current);
                    KImg_Left.GetImage().SetSpriteAsync("icon_equip_tips_heart", false).Forget();
                    break;
                case 107010:
                    KText_Atk.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_speed").current);
                    KImg_Left.GetImage().SetSpriteAsync("icon_equip_tips_move", false).Forget();
                    break;
            }


            var skillGroup = equip_data.minorSkillGroup;

            int curQuaIndex = -1;
            for (int i = 0; i < skillGroup.Count; i++)
            {
                if ((int)skillGroup[i].x == quality.id)
                {
                    curQuaIndex = i;
                    break;
                }
            }


            //var skillList = KContentEquipTips.GetList();
            skillList.Clear();
            for (int i = 0; i < skillGroup.Count; i++)
            {
                var skill = skillGroup[i];
                var qualityId = (int)skill.x;
                var ui = await skillList.CreateWithUITypeAsync(UIType.UISubPanel_EquipSkill, false);
                // var ui = await UIHelper.CreateAsync(this, UIType.UISubPanel_EquipSkill,
                //     KContentEquipTips.GameObject.transform);
                var img = ui.GetFromReference(UISubPanel_EquipSkill.KImg_Unlock);
                var text = ui.GetFromReference(UISubPanel_EquipSkill.KText_Content);
                //var mask = ui.GetFromReference(UISubPanel_EquipSkill.KMask);
                var playerSkill = player_skill.GetOrDefault((int)skill.y);


                string skillDes = default;
                if (playerSkill == null)
                {
                    skillDes = "key is not exist";
                }
                else
                {
                    skillDes = string.Format(languageConfig.Get(playerSkill.desc).current,
                        playerSkill.descPara.ToArray());
                }

                if (curQuaIndex >= i)
                {
                    img.GetImage().SetSpriteAsync(qualityConfig.Get(qualityId).unlock, true).Forget();
                    text.GetTextMeshPro().SetTMPText(skillDes);
                    text.GetTextMeshPro().SetAlpha(1);
                    img.GetImage().SetAlpha(1);
                }
                else
                {
                    img.GetImage().SetSpriteAsync(qualityConfig.Get(qualityId).lock0, true).Forget();
                    text.GetTextMeshPro()
                        .SetTMPText(UnityHelper.RichTextColor(skillDes, "31324E"));
                    text.GetTextMeshPro().SetAlpha(0.5f);
                    img.GetImage().SetAlpha(0.5f);
                }
            }

            JiYuUIHelper.ForceRefreshLayout(KContentEquipTips);

            RefreshLevelUp(0, myGameEquip);


            //TODO:装备详情 装备已经达到等级上限 缺多语言
            //attr_hp生命
            //等级attr_info_level
            //攻击attr_atk
            //attr_speed 移动速度
            //升级消耗equip_levelup_cost
            //common_state_common装备
            //common_state_levelup升级
            //一键升级common_state_levelupclick
            //common_state_remove卸下
            //common_state_leveldown降级
            //common_state_qualitydown降品
            //common_state_cancel取消
            //common_state_equipping装备中
            //common_state_change更换
            //common_state_merge合成
            //common_state_mergeclick一键合成


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_LevelUp,
                () =>
                {
                    if (!TryGetLevelUpCosts(equipUid, out var costs))
                    {
                        return;
                    }

                    if (JiYuUIHelper.TryReduceReward(costs))
                    {
                        NetWorkManager.Instance.SendMessage(CMD.EQUIPUPGRADE, equip);
                        //NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY);
                        JiYuUIHelper.WearOrUnWearEquipProperty(equip, false);
                        if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                                out var gameEquip))
                        {
                            gameEquip.equip.EquipLevel++;
                            JiYuUIHelper.WearOrUnWearEquipProperty(equip, true);
                            RefreshLevelUp(equipUid).Forget();
                            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                            {
                                var uiscript = ui as UIPanel_Equipment;
                                uiscript.SortItems(equip_data.posId);
                                uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.None, -1,
                                    -1);
                            }
                        }
                    }
                    else
                    {
                        JiYuUIHelper.ClearCommonResource();
                        var str = languageConfig.Get("equip_levelup_error").current;
                        UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                        Log.Debug("资源不足", Color.red);
                    }
                });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_AllLevelUp,
                () =>
                {
                    if (!TryGetLevelUpCosts(equipUid, out var costs11))
                    {
                        return;
                    }

                    NetWorkManager.Instance.SendMessage(CMD.EQUIPALLUPGRADE, equip);
                    //NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY);
                    if (JiYuUIHelper.TryReduceReward(costs11))
                    {
                        if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                                out var gameEquip))
                        {
                            JiYuUIHelper.WearOrUnWearEquipProperty(equip, false);
                            gameEquip.equip.EquipLevel++;
                        }
                    }
                    else
                    {
                        JiYuUIHelper.ClearCommonResource();
                        var str = languageConfig.Get("equip_levelup_error").current;
                        UIHelper.CreateAsync(UIType.UICommon_Resource, str);
                        Log.Debug("资源不足", Color.red);
                        return;
                    }

                    while (true)
                    {
                        if (!TryGetLevelUpCosts(equipUid, out var costs))
                        {
                            break;
                        }

                        if (JiYuUIHelper.TryReduceReward(costs))
                        {
                            if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                                    out var gameEquip))
                            {
                                gameEquip.equip.EquipLevel++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    JiYuUIHelper.WearOrUnWearEquipProperty(equip, true);
                    //TODO:刷新tip界面
                    RefreshLevelUp(equipUid);
                    if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                    {
                        var uiscript = ui as UIPanel_Equipment;
                        uiscript.SortItems(equip_data.posId);
                        uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.None,
                            -1,
                            -1);
                    }
                });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () => { Close(); });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Decrease,
                () =>
                {
                    if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                            out var gameEquip))
                    {
                        UIHelper.CreateAsync(UIType.UIPanel_EquipDownGrade, gameEquip);
                        Close();
                    }
                });

            //StartTimer();
        }

        void OnQueryPropertyResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            Log.Error("OnInitPlayerPropertyResponse");
            var battleProperty = new BattleProperty();
            battleProperty.MergeFrom(e.data);


            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            var playerData = new PlayerData();
            var chaStats = new ChaStats();
            JiYuUIHelper.InitPlayerProperty(ref playerData, ref chaStats, battleProperty);

            ResourcesSingleton.Instance.playerProperty.playerData = playerData;
            ResourcesSingleton.Instance.playerProperty.chaProperty = chaStats.chaProperty;

            Log.Debug($"{ResourcesSingleton.Instance.playerProperty.chaProperty}", Color.red);
        }


        async UniTaskVoid RefreshLevelUp(long equipuid = 0, MyGameEquip myGameEquip0 = null)
        {
            var attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;
            var equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;
            var player_skill = ConfigManager.Instance.Tables.Tbskill;
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;

            MyGameEquip myGame = default;
            if (equipuid == 0)
            {
                myGame = myGameEquip0;
            }
            else
            {
                if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipuid, out var myGameEquip))
                {
                    myGame = myGameEquip;
                }
            }

            var equip = myGame.equip;
            long equipUid = equip.PartId;
            int equipId = equip.EquipId;
            int equipQuality = equip.Quality;
            int equipLevel = equip.EquipLevel;
            equipId = equip.EquipId > 10000 ? equip.EquipId : equip.EquipId * 100 + equip.Quality;

            //equipId = equip.EquipId * 100 + equip.Quality;
            var equip_data = equip_dataConfig.Get(equipId);
            var equip_quality = equip_qualityConfig.Get(equip_data.quality);
            var quality = qualityConfig.Get(equip_quality.type);
            var equip_pos = equip_posConfig.Get(equip_data.posId);
            var equip_level = equip_levelConfig.Get(equipLevel);


            var KText_EquipName = GetFromReference(UICommon_EquipTips.KText_EquipName);
            var KText_Level = GetFromReference(UICommon_EquipTips.KText_Level);
            var KText_LevelNum = GetFromReference(UICommon_EquipTips.KText_LevelNum);
            var KText_Atk = GetFromReference(UICommon_EquipTips.KText_Atk);
            var KText_AtkNum = GetFromReference(UICommon_EquipTips.KText_AtkNum);
            var KText_LevelupCost = GetFromReference(UICommon_EquipTips.KText_LevelupCost);
            var KText_Equip = GetFromReference(UICommon_EquipTips.KText_Equip);
            var KText_LevelUp = GetFromReference(UICommon_EquipTips.KText_LevelUp);
            var KText_AllLevelUp = GetFromReference(UICommon_EquipTips.KText_AllLevelUp);

            var KBtn_Close = GetFromReference(UICommon_EquipTips.KBtn_Close);
            var KContentEquipTips = GetFromReference(UICommon_EquipTips.KContentEquipTips);

            var KBtn_Equip = GetFromReference(UICommon_EquipTips.KBtn_Equip);
            var KBtn_LevelUp = GetFromReference(UICommon_EquipTips.KBtn_LevelUp);
            var KBtn_AllLevelUp = GetFromReference(UICommon_EquipTips.KBtn_AllLevelUp);


            var KisMaxLevel = GetFromReference(UICommon_EquipTips.KisMaxLevel);
            var KnotMaxLevel = GetFromReference(UICommon_EquipTips.KnotMaxLevel);


            var atkNum = equip_data.mainEntryInit + (equipLevel - 1) * equip_data.mainEntryGrow;
            KText_AtkNum.GetTextMeshPro().SetTMPText(atkNum.ToString());

            if (equipLevel > 0)
            {
                KText_LevelNum.SetActive(true);
                KText_LevelNum.GetTextMeshPro().SetTMPText(equipLevel.ToString());
            }


            var KItemPos = GetFromKeyOrPath(UICommon_EquipTips.KItemPos);
            var uiList = KItemPos.GetList();

            uiList.Clear();

            if (!TryGetLevelUpCosts(equipUid, out var costs))
            {
                KisMaxLevel.SetActive(true);
                KnotMaxLevel.SetActive(false);
                KBtn_LevelUp.SetActive(false);
                KBtn_AllLevelUp.SetActive(false);
                //TODO:装备已经达到等级上限 多语言
                //KText_LevelMaxInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_info_level").current);
                return;
            }

            KisMaxLevel.SetActive(false);
            KnotMaxLevel.SetActive(true);
            KBtn_LevelUp.SetActive(true);
            KBtn_AllLevelUp.SetActive(true);
            foreach (var cost in costs)
            {
                //Log.Error("创建Item");
                var ui = await uiList.CreateWithUITypeAsync(UIType.UISubPanel_EquipLevelConsumeItem, cost, false);

                // var ui = await UIHelper.CreateAsync(KItemPos, UIType.UISubPanel_EquipLevelConsumeItem,
                //     KItemPos.GameObject.transform);
                //KItemPos.AddChild(ui);
                //ui.SetParent(KItemPos, true);
                var icon = ui.GetFromReference(UISubPanel_EquipLevelConsumeItem.KIcon_Consume);
                JiYuUIHelper.SetIconOnly(cost, icon);
                var text = ui.GetFromReference(UISubPanel_EquipLevelConsumeItem.KText_Consume);
                var count = JiYuUIHelper.GetRewardCount(cost);
                //TODO:count  k处
                var countStr = JiYuUIHelper.ReturnFormatResourceNum(count);
                var countCostStr = JiYuUIHelper.ReturnFormatResourceNum((long)cost.z);
                string textstr = count >= (long)cost.z
                    ? $"{countStr}/{countCostStr}"
                    : $"{UnityHelper.RichTextColor(countStr, "FF0000")}/{countCostStr}";
                text.GetTextMeshPro().SetTMPText(textstr);
            }

            uiList.Sort((obj11, obj22) =>
            {
                Tblanguage language = ConfigManager.Instance.Tables.Tblanguage;
                Tbuser_variable user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
                Tbitem item = ConfigManager.Instance.Tables.Tbitem;
                Tbequip_data equip_data = ConfigManager.Instance.Tables.Tbequip_data;
                Tbequip_quality equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
                Tbquality quality = ConfigManager.Instance.Tables.Tbquality;

                var obj111 = obj11 as UISubPanel_EquipLevelConsumeItem;
                var obj211 = obj22 as UISubPanel_EquipLevelConsumeItem;
                var obj1 = obj111.reward;
                var obj2 = obj211.reward;
                var obj1rewardx = (int)obj1.x;
                var obj1rewardy = (int)obj1.y;
                var obj1rewardz = (int)obj1.z;
                var obj2rewardx = (int)obj2.x;
                var obj2rewardy = (int)obj2.y;
                var obj2rewardz = (int)obj2.z;

                if (obj1rewardx == 11 && obj2rewardx != 11)
                    return -1;
                else if (obj1rewardx != 11 && obj2rewardx == 11)
                    return 1;

                if (obj1rewardx != 5 && obj2rewardx == 5)
                    return -1;
                else if (obj1rewardx == 5 && obj2rewardx != 5)
                    return 1;

                if (obj1rewardx == 11 && obj2rewardx == 11)
                {
                    if (!JiYuUIHelper.IsCompositeEquipReward(obj1) &&
                        JiYuUIHelper.IsCompositeEquipReward(obj2))
                        return -1;
                    else if (JiYuUIHelper.IsCompositeEquipReward(obj1) &&
                             !JiYuUIHelper.IsCompositeEquipReward(obj2))
                        return 1;

                    if (equip_data.Get(obj1rewardy).quality >
                        equip_data.Get(obj2rewardy).quality)
                        return -1;
                    else if (equip_data.Get(obj1rewardy).quality <
                             equip_data.Get(obj2rewardy).quality)
                        return 1;

                    if (equip_data.Get(obj1rewardy).sYn == 1 &&
                        equip_data.Get(obj2rewardy).sYn != 1)
                        return -1;
                    else if (equip_data.Get(obj1rewardy).sYn != 1 &&
                             equip_data.Get(obj2rewardy).sYn == 1)
                        return 1;


                    if (equip_data.Get(obj1rewardy).posId <
                        equip_data.Get(obj2rewardy).posId)
                        return -1;
                    else if (equip_data.Get(obj1rewardy).posId >
                             equip_data.Get(obj2rewardy).posId)
                        return 1;

                    if (obj1rewardy > obj2rewardy)
                        return -1;
                    else if (obj1rewardy < obj2rewardy)
                        return 1;
                }

                if (JiYuUIHelper.IsResourceReward(obj1) && JiYuUIHelper.IsResourceReward(obj2))
                {
                    if (obj1rewardx < obj2rewardx)
                        return -1;
                    else if (obj1rewardx > obj2rewardx)
                        return 1;
                }

                if (obj1rewardx == 5 && obj2rewardx == 5)
                {
                    if (item.Get(obj1rewardy).sort < item.Get(obj2rewardy).sort)
                        return -1;
                    else if (item.Get(obj1rewardy).sort > item.Get(obj2rewardy).sort)
                        return 1;
                    if (obj1rewardy < obj2rewardy)
                        return -1;
                    else if (obj1rewardy > obj2rewardy)
                        return 1;
                }

                return 0;
            });
            //uiList.Sort();
            JiYuUIHelper.ForceRefreshLayout(KItemPos);
            //KItemPos.Dispose();
            //KItemPos.Dispose();

            //Type optionType = optionTypes[index];
            //this.AddChild(KItemPos);
        }

        bool TryGetLevelUpCosts(long equipuid, out List<Vector3> costs)
        {
            ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipuid, out var myGameEquip);
            var attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;
            var equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;
            var player_skill = ConfigManager.Instance.Tables.Tbskill;
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;

            var equip = myGameEquip.equip;
            long equipUid = equip.PartId;
            int equipId = equip.EquipId;
            int equipQuality = equip.Quality;
            int equipLevel = equip.EquipLevel;
            equipId = equip.EquipId > 10000 ? equip.EquipId : equip.EquipId * 100 + equip.Quality;

            //equipId = equip.EquipId * 100 + equip.Quality;

            var equip_data = equip_dataConfig.Get(equipId);
            var equip_level = equip_levelConfig.Get(equipLevel);
            var equip_quality = equip_qualityConfig.Get(equip_data.quality);
            costs = new List<Vector3>();

            if (equipLevel >= equip_quality.levelMax)
            {
                return false;
            }


            switch (equip_data.posId)
            {
                case 1:
                    costs = equip_level.cost1;
                    break;
                case 2:
                    costs = equip_level.cost2;
                    break;
                case 3:
                    costs = equip_level.cost3;
                    break;
                case 4:
                    costs = equip_level.cost4;
                    break;
                case 5:
                    costs = equip_level.cost5;
                    break;
                case 6:
                    costs = equip_level.cost6;
                    break;
            }

            return true;
        }

        public void OnEquipWearResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPWEAR, OnEquipWearResponse);
            // GameEquip gameEquip = new GameEquip();
            // gameEquip.MergeFrom(e.data);
            Log.Debug("OnEquipWearOrUnwearResponse", Color.red);
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            //Log.Debug($"gameEquip{gameEquip}", Color.red);
        }

        public void OnEquipAllUpGradeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPALLUPGRADE, OnEquipAllUpGradeResponse);
            GameEquip gameEquip = new GameEquip();
            gameEquip.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            Log.Debug($"gameEquip{gameEquip}", Color.red);
        }

        public void OnEquipUpGradeResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPUPGRADE, OnEquipUpGradeResponse);
            GameEquip gameEquip = new GameEquip();
            gameEquip.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (!ResourcesSingleton.Instance.equipmentData.equipments.ContainsKey(gameEquip.PartId))
            {
                Log.Error($"装备升级后返回不存在的装备uid", Color.red);
                return;
            }


            Log.Debug($"gameEquip{gameEquip}", Color.red);
        }


        protected override void OnClose()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
            {
                var uiequip = ui as UIPanel_Equipment;
                uiequip.DestorySelected();
                //uiequip.lastClickTipItem = btnui;
            }

            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYPROPERTY, OnQueryPropertyResponse);
            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPUNWEAR, OnEquipWearResponse);
            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPWEAR, OnEquipWearResponse);

            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPUPGRADE, OnEquipUpGradeResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPDOWNGRADE, OnEquipUpGradeResponse);
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPALLUPGRADE, OnEquipAllUpGradeResponse);


            base.OnClose();
        }
    }
}