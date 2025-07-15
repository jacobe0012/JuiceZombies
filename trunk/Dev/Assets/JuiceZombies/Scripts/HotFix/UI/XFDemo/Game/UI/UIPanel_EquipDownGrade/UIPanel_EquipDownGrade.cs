//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_EquipDownGrade)]
    internal sealed class UIPanel_EquipDownGradeEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_EquipDownGrade;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_EquipDownGrade>();
        }
    }

    public partial class UIPanel_EquipDownGrade : UI, IAwake<MyGameEquip>
    {
        private UICommon_EquipItem topEquipUi;
        List<Vector3> downGradeRewards = new List<Vector3>();
        List<Vector3> downQualityRewards = new List<Vector3>();
        int needQuality = default;

        public async void Initialize(MyGameEquip myGameEquip)
        {
            await JiYuUIHelper.InitBlur(this);
            InitPanel(myGameEquip);
        }

        public void OnEquipDownGradePanelResponse(object sender, WebMessageHandler.Execute e)
        {
            WebMessageHandler.Instance.RemoveHandler(CMD.EQUIPDOWNGRADE, OnEquipDownGradePanelResponse);
            GameEquip gameEquip = new GameEquip();
            gameEquip.MergeFrom(e.data);

            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            if (!ResourcesSingleton.Instance.equipmentData.equipments.ContainsKey(gameEquip.PartId))
            {
                Log.Error($"װ�������󷵻ز����ڵ�װ��uid", Color.red);
                return;
            }

            // ResourcesSingleton.Instance.equipmentData.equipments[gameEquip.PartId] = gameEquip;
            //
            //
            // foreach (var VARIABLE in ResourcesSingleton.Instance.equipmentData.isWearingEquipments)
            // {
            //     if (VARIABLE.Value.PartId == gameEquip.PartId)
            //     {
            //         ResourcesSingleton.Instance.equipmentData.isWearingEquipments[VARIABLE.Key] = gameEquip;
            //     }
            // }

            Log.Debug($"gameEquip{gameEquip}", Color.red);
        }

        private void RefreshEquipPanel()
        {
            //TODO:����ˢ��ҳ��
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
            {
                var uiscript = ui as UIPanel_Equipment;
                var temp = uiscript.lastModuleId;
                uiscript.lastModuleId = -1;
                uiscript.OnClickActionEvent(temp);
                //uiscript.lastModuleId = temp;
                uiscript.RefreshAllWearEquip();

                var KMask = GetFromReference(UIPanel_EquipDownGrade.KMask);
                KMask.GetXButton().SetEnabled(true);
            }
        }

        async UniTaskVoid InitPanel(MyGameEquip myGameEquip)
        {
            var equip = myGameEquip.equip;

            WebMessageHandler.Instance.AddHandler(CMD.EQUIPDOWNGRADE, OnEquipDownGradePanelResponse);

            var attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;
            var equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;

            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;


            long equipUid = equip.PartId;
            int equipId = equip.EquipId;
            int equipQuality = equip.Quality;
            int equipLevel = equip.EquipLevel;
            equipId = equip.EquipId * 100 + equip.Quality;
            var equip_data = equip_dataConfig.Get(equipId);
            var equip_quality = equip_qualityConfig.Get(equip_data.quality);
            var quality = qualityConfig.Get(equip_quality.type);
            var equip_pos = equip_posConfig.Get(equip_data.posId);
            var equip_level = equip_levelConfig.Get(equipLevel);


            var KMask = GetFromReference(UIPanel_EquipDownGrade.KMask);
            var KUpPos = GetFromReference(UIPanel_EquipDownGrade.KUpPos);
            var KText_DownGTitle = GetFromReference(UIPanel_EquipDownGrade.KText_DownGTitle);
            var KText_DownGInfo = GetFromReference(UIPanel_EquipDownGrade.KText_DownGInfo);
            var KContent_DownG = GetFromReference(UIPanel_EquipDownGrade.KContent_DownG);
            var KBtn_DownGrade = GetFromReference(UIPanel_EquipDownGrade.KBtn_DownGrade);
            var KText_DownGrade = GetFromReference(UIPanel_EquipDownGrade.KText_DownGrade);
            var KText_DownQTitle = GetFromReference(UIPanel_EquipDownGrade.KText_DownQTitle);
            var KText_DownQInfo = GetFromReference(UIPanel_EquipDownGrade.KText_DownQInfo);
            var KBtn_DownQuality = GetFromReference(UIPanel_EquipDownGrade.KBtn_DownQuality);
            var KText_DownQuality = GetFromReference(UIPanel_EquipDownGrade.KText_DownQuality);
            var KContent_DownQ = GetFromReference(UIPanel_EquipDownGrade.KContent_DownQ);
            var KText_DownGError = GetFromReference(UIPanel_EquipDownGrade.KText_DownGError);
            var KText_DownQError = GetFromReference(UIPanel_EquipDownGrade.KText_DownQError);


            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_DownGrade,
                () =>
                {
                    NetWorkManager.Instance.SendMessage(CMD.EQUIPDOWNGRADE, equip);

                    JiYuUIHelper.AddReward(downGradeRewards, true, true);
                    if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                            out var gameEquip))
                    {
                        return;
                    }

                    JiYuUIHelper.WearOrUnWearEquipProperty(equip, false);
                    gameEquip.equip.EquipLevel = 1;
                    JiYuUIHelper.WearOrUnWearEquipProperty(equip, true);
                    topEquipUi?.Initialize(gameEquip, UIPanel_Equipment.EquipPanelType.Main);
                    RefreshLevelUp(equipUid).Forget();
                    RefreshEquipPanel();
                    // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                    // {
                    //     var uiscript = ui as UIPanel_Equipment;
                    //     uiscript.SortItems(equip_data.posId);
                    //     //await UniTask.Delay(500);
                    //     uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.None, -1,
                    //         -1);
                    // }
                });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_DownQuality,
                async () =>
                {
                    JiYuUIHelper.AddReward(downQualityRewards, true, true);
                    if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                            out var gameEquip))
                    {
                        return;
                    }

                    var KImg_Quality = topEquipUi.GetFromReference(UICommon_EquipItem.KImg_Quality);
                    var KText_Grade = topEquipUi.GetFromReference(UICommon_EquipItem.KText_Grade);
                    var KBg_Item = topEquipUi.GetFromReference(UICommon_EquipItem.KBg_Item);
                    KImg_Quality.SetActive(false);
                    KBg_Item.GetImage().SetSpriteAsync(quality.frame, false).Forget();
                    KImg_Quality.SetActive(false);
                    KText_Grade.SetActive(false);
                    KText_DownQError.SetActive(true);
                   
                    JiYuUIHelper.SetGrayOrNot(KBtn_DownQuality, "common_button_gray1", "common_blue_button_6", true);
                    KMask.GetXButton().SetEnabled(false);
                    var downQList = KContent_DownQ.GetList();
                    downQList.Clear();
                    //await UniTask.Delay(1000);

                    NetWorkManager.Instance.SendMessage(CMD.EQUIPDOWNQUALITY, equip);
                    NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP, equip);

                    await UniTask.Delay(1000);
                    RefreshEquipPanel();

                    // if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(equipUid,
                    //         out var gameEquip))
                    // {
                    //     gameEquip.equip.Level = 1;
                    //     gameEquip.equip.Quality = needQuality;
                    //     topEquipUi?.Initialize(gameEquip, UIPanel_Equipment.EquipPanelType.Main);
                    //     RefreshLevelUp(equipUid).Forget();
                    //
                    //     // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                    //     // {
                    //     //     var uiscript = ui as UIPanel_Equipment;
                    //     //     uiscript.Initialize(uiscript.tempStructList);
                    //     //     // uiscript.SortItems(equip_data.posId);
                    //     //     // //await UniTask.Delay(500);
                    //     //     // uiscript.RefreshMainRedDot(equip_data.posId, UIPanel_Equipment.WearEquipType.None, -1,
                    //     //     //     -1);
                    //     // }
                    // }
                });

            KText_DownGTitle.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_leveldown_title").current);
            KText_DownGInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_leveldown_desc").current);
            KText_DownQTitle.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_quality_title").current);
            KText_DownQInfo.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_qualitydown_desc").current);
            KText_DownGrade.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_leveldown_tab").current);
            KText_DownQuality.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_qualitydown_tab").current);

            KText_DownGError.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_leveldown_error").current);
            KText_DownQError.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_qualitydown_error").current);

            RefreshLevelUp(equipUid).Forget();

            topEquipUi = await UIHelper.CreateAsync(this, UIType.UICommon_EquipItem, myGameEquip,
                UIPanel_Equipment.EquipPanelType.Main,
                KUpPos.GameObject.transform) as UICommon_EquipItem;
            var reddot = topEquipUi.GetFromReference(UICommon_EquipItem.KImg_RedDot);
            reddot.SetActive(false);
            this.AddChild(topEquipUi);
            var rect = topEquipUi.GetRectTransform();
            rect.SetAnchoredPosition(0, 0);
            rect.SetPivot(new Vector2(0.5f, 0.5f));
            rect.SetAnchorMax(Vector2.one);
            rect.SetAnchorMin(Vector2.zero);
            rect.SetOffset(0, 0, 0, 0);
            rect.SetScale2(1.3f);

            KMask.GetXButton().OnClick.Add(() => { Close(); });
        }


        async UniTaskVoid RefreshLevelUp(long uid)
        {
            if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid, out var myGameEquip))
            {
                Log.Error($"{uid} is not exist in equipments");
                return;
            }

            var equip = myGameEquip.equip;

            var attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;
            var equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;


            long equipUid = equip.PartId;
            int equipId = equip.EquipId;
            int equipQuality = equip.Quality;
            int equipLevel = equip.EquipLevel;
            equipId = equip.EquipId * 100 + equip.Quality;
            var equip_data = equip_dataConfig.Get(equipId);
            var equip_quality = equip_qualityConfig.Get(equip_data.quality);
            var quality = qualityConfig.Get(equip_quality.type);
            var equip_pos = equip_posConfig.Get(equip_data.posId);
            var equip_level = equip_levelConfig.Get(equipLevel);


            var KMask = GetFromReference(UIPanel_EquipDownGrade.KMask);
            var KUpPos = GetFromReference(UIPanel_EquipDownGrade.KUpPos);
            var KText_DownGTitle = GetFromReference(UIPanel_EquipDownGrade.KText_DownGTitle);
            var KText_DownGInfo = GetFromReference(UIPanel_EquipDownGrade.KText_DownGInfo);
            var KContent_DownG = GetFromReference(UIPanel_EquipDownGrade.KContent_DownG);
            var KBtn_DownGrade = GetFromReference(UIPanel_EquipDownGrade.KBtn_DownGrade);
            var KText_DownGrade = GetFromReference(UIPanel_EquipDownGrade.KText_DownGrade);
            var KText_DownQTitle = GetFromReference(UIPanel_EquipDownGrade.KText_DownQTitle);
            var KText_DownQInfo = GetFromReference(UIPanel_EquipDownGrade.KText_DownQInfo);
            var KBtn_DownQuality = GetFromReference(UIPanel_EquipDownGrade.KBtn_DownQuality);
            var KText_DownQuality = GetFromReference(UIPanel_EquipDownGrade.KText_DownQuality);
            var KContent_DownQ = GetFromReference(UIPanel_EquipDownGrade.KContent_DownQ);
            var KText_DownGError = GetFromReference(UIPanel_EquipDownGrade.KText_DownGError);
            var KText_DownQError = GetFromReference(UIPanel_EquipDownGrade.KText_DownQError);

            bool isDownGError = false;
            if (equipLevel == 1)
            {
                isDownGError = true;
            }

            bool isDownQError = false;
            if (equip_quality.subtype == 1)
            {
                isDownQError = true;
            }

            var downGList = KContent_DownG.GetList();
            downGList.Clear();
            //JiYuUIHelper.SetGrayOrNot(KBtn_DownGrade, "common_button_gray1", "common_blue_button_6", false);
         
            if (!isDownGError)
            {
                JiYuUIHelper.SetGrayOrNot(KBtn_DownGrade, "common_button_gray1", "common_blue_button_6", false);
                KText_DownGError.SetActive(false);
                Dictionary<Vector2, long> costs = new Dictionary<Vector2, long>();
                int tempLevel = equipLevel;
                while (true)
                {
                    if (tempLevel - 1 <= 0)
                    {
                        break;
                    }

                    tempLevel--;
                    Dictionary<Vector2, long> tempcosts = new Dictionary<Vector2, long>();
                    List<Vector3> list = new List<Vector3>();
                    switch (equip_data.posId)
                    {
                        case 1:
                            list = equip_levelConfig.Get(tempLevel).cost1;
                            break;
                        case 2:
                            list = equip_levelConfig.Get(tempLevel).cost2;
                            break;
                        case 3:
                            list = equip_levelConfig.Get(tempLevel).cost3;
                            break;
                        case 4:
                            list = equip_levelConfig.Get(tempLevel).cost4;
                            break;
                        case 5:
                            list = equip_levelConfig.Get(tempLevel).cost5;
                            break;
                        case 6:
                            list = equip_levelConfig.Get(tempLevel).cost6;
                            break;
                    }

                    foreach (var VARIABLE in list)
                    {
                        tempcosts.TryAdd(new Vector2(VARIABLE.x, VARIABLE.y), (long)VARIABLE.z);
                    }

                    foreach (var VARIABLE in tempcosts)
                    {
                        if (costs.ContainsKey(VARIABLE.Key))
                        {
                            costs[VARIABLE.Key] += VARIABLE.Value;
                        }
                        else
                        {
                            costs.Add(VARIABLE.Key, VARIABLE.Value);
                        }
                    }

                    tempcosts.Clear();
                    tempcosts = null;
                }

                downGradeRewards = new List<Vector3>();
                foreach (var cost in costs)
                {
                    downGradeRewards.Add(new Vector3(cost.Key.x, cost.Key.y, cost.Value));
                }

                JiYuUIHelper.SortRewards(downGradeRewards);
                foreach (var cost in downGradeRewards)
                {
                    //Log.Error($"cost{cost}");
                    var ui0 = await downGList.CreateWithUITypeAsync(UIType.UICommon_RewardItem,
                        cost, false);
                    ui0.GetRectTransform().SetScale2(0.92f);
                }

                costs.Clear();
                costs = null;
            }
            else
            {
                KText_DownGError.SetActive(true);
                JiYuUIHelper.SetGrayOrNot(KBtn_DownGrade, "common_button_gray1", "common_blue_button_6", true);
            }


            var downQList = KContent_DownQ.GetList();
            downQList.Clear();
            if (!isDownQError)
            {
                JiYuUIHelper.SetGrayOrNot(KBtn_DownQuality, "common_button_gray1", "common_blue_button_6", false);
                KText_DownQError.SetActive(false);
                Dictionary<Vector3, long> costs = new Dictionary<Vector3, long>();
                int tempLevel = equipLevel;
                int tempEquipQuality = equip_quality.id;
                int tempSubQuality = equip_quality.subtype;
                while (true)
                {
                    if (tempLevel - 1 <= 0)
                    {
                        break;
                    }

                    tempLevel--;
                    Dictionary<Vector3, long> tempcosts = new Dictionary<Vector3, long>();
                    List<Vector3> list = new List<Vector3>();
                    switch (equip_data.posId)
                    {
                        case 1:
                            list = equip_levelConfig.Get(tempLevel).cost1;
                            break;
                        case 2:
                            list = equip_levelConfig.Get(tempLevel).cost2;
                            break;
                        case 3:
                            list = equip_levelConfig.Get(tempLevel).cost3;
                            break;
                        case 4:
                            list = equip_levelConfig.Get(tempLevel).cost4;
                            break;
                        case 5:
                            list = equip_levelConfig.Get(tempLevel).cost5;
                            break;
                        case 6:
                            list = equip_levelConfig.Get(tempLevel).cost6;
                            break;
                    }

                    foreach (var VARIABLE in list)
                    {
                        tempcosts.TryAdd(new Vector3(VARIABLE.x, VARIABLE.y, 0), (long)VARIABLE.z);
                    }

                    foreach (var VARIABLE in tempcosts)
                    {
                        if (costs.ContainsKey(VARIABLE.Key))
                        {
                            costs[VARIABLE.Key] += VARIABLE.Value;
                        }
                        else
                        {
                            costs.Add(VARIABLE.Key, VARIABLE.Value);
                        }
                    }

                    tempcosts.Clear();
                    tempcosts = null;
                }

                needQuality = default;
                while (true)
                {
                    if (tempSubQuality - 1 <= 0)
                    {
                        break;
                    }

                    tempSubQuality--;
                    tempEquipQuality--;
                    //Dictionary<Vector3, long> tempcosts = new Dictionary<Vector3, long>();
                    var mergeRule = equip_qualityConfig.Get(tempEquipQuality).mergeRule;
                    needQuality = mergeRule[0];
                    var needCount = mergeRule[1];
                    Vector3 mergeedReward = default;

                    foreach (var VARIABLE in equip_dataConfig.DataList)
                    {
                        if (VARIABLE.id % 100 == 0 && VARIABLE.posId == equip_data.posId &&
                            VARIABLE.quality == needQuality)
                        {
                            mergeedReward.x = 11;
                            mergeedReward.y = VARIABLE.id;
                            mergeedReward.z = VARIABLE.quality;

                            if (costs.ContainsKey(mergeedReward))
                            {
                                costs[mergeedReward] += needCount;
                            }
                            else
                            {
                                costs.Add(mergeedReward, needCount);
                            }

                            break;
                        }
                    }
                }

                costs.TryAdd(new Vector3(11, equipId, 0), needQuality);

                downQualityRewards = new List<Vector3>();
                foreach (var cost in costs)
                {
                    Vector3 mergeedReward = new Vector3(cost.Key.x, cost.Key.y, cost.Value);

                    if (cost.Key.z != 0)
                    {
                        var mergeint = CmdHelper.GetMergeCmd((int)cost.Key.z, (int)cost.Value);

                        mergeedReward = new Vector3(cost.Key.x, cost.Key.y, mergeint);
                    }

                    downQualityRewards.Add(mergeedReward);
                }

                JiYuUIHelper.SortRewards(downQualityRewards);

                foreach (var cost in downQualityRewards)
                {
                    var ui0 = await downQList.CreateWithUITypeAsync(UIType.UICommon_RewardItem,
                        cost, false);
                    ui0.GetRectTransform().SetScale2(0.92f);
                }


                costs.Clear();
                costs = null;
            }
            else
            {
                KText_DownQError.SetActive(true);
                JiYuUIHelper.SetGrayOrNot(KBtn_DownQuality, "common_button_gray1", "common_blue_button_6", true);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}