//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf;
using Google.Protobuf.Collections;
using HotFix_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UIPanel_Equipment;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Compound)]
    internal sealed class UIPanel_CompoundEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Compound;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Compound>();
        }
    }

    public partial class UIPanel_Compound : UI, IAwake
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private UIPanel_Equipment panelEquipment;
        private List<UICommon_Label> UICommon_Labels = new List<UICommon_Label>();
        private bool isDone;
        public CommonLabelType curSortBy = CommonLabelType.Consume;
        public int selectedNum;
        public Dictionary<long, int> selectedEquips = new Dictionary<long, int>();
        private bool isShakeEnd;
        public  MyGameEquip equipMain;

        public void Initialize()
        {
            //SubPanel_CommonBtn
            JiYuUIHelper.SortEquipments(true);
            JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui);
            panelEquipment = ui as UIPanel_Equipment;
            InitPanel();
            GetFromReference(KMid).GetRectTransform().SetAnchoredPositionY(230);
        }

        public async UniTaskVoid OnSelected(long uid)
        {
            this.GetFromReference(KTextNoEquip).SetActive(false);

            if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid, out var myGameEquip))
            {
                Log.Error($"uid:{uid} is not exist");
                return;
            }

            
            bool add = false;

            if (selectedEquips.ContainsKey(uid))
            {
                add = false;
                selectedNum--;
                selectedEquips.Remove(uid);
            }
            else
            {
                add = true;
                selectedNum++;
                selectedEquips.Add(uid, selectedNum);
            }


            long uid0 = 0;
            foreach (var kv in selectedEquips)
            {
                if (kv.Value == 1)
                {
                    uid0 = kv.Key;
                    break;
                }
            }

            if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid0, out var myGameEquip0))
            {
                Log.Error($"uid:{uid} is not exist");
                return;
            }

            var equip0 = myGameEquip0.equip;
            long equipUid0 = equip0.PartId;
            int equipId0 = equip0.EquipId;
            int equipQuality0 = equip0.Quality;
            int equipLevel0 = equip0.EquipLevel;
            int equipId = equip0.EquipId * 100 + equip0.Quality;
            //Log.Error($"selectedNum{selectedNum}selectedEquips{selectedEquips.Count}");
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;
            var player_skillConfig = ConfigManager.Instance.Tables.Tbskill;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;

            var equip_data0 = equip_dataConfig.Get(equipId);
            var equip_quality0 = equip_qualityConfig.Get(equip_data0.quality);
            var needList0 = equip_quality0.mergeRule;
            var mergeSelf0 = equip_quality0.mergeSelf;

            int needQua = 0;
            int needCount = 0;
            bool isMaxError = false;
            if (needList0.Count > 1)
            {
                needQua = needList0[0];
                needCount = needList0[1];
                //Log.Error($"needList.Count{needList0.Count} is not enough");
            }
            else
            {
                isMaxError = true;
            }


            var KItemsPos = GetFromReference(UIPanel_Compound.KItemsPos);
            var KContent = GetFromReference(UIPanel_Compound.KContent);
            var KBtn_Compound = GetFromReference(UIPanel_Compound.KBtn_Compound);
            var KImg_LeftLine = GetFromReference(UIPanel_Compound.KImg_LeftLine);
            var KText_MaxQualityError = GetFromReference(UIPanel_Compound.KText_MaxQualityError);
            var KCompoundResult = GetFromReference(UIPanel_Compound.KCompoundResult);
            var lableList = KContent.GetList();
            var itemList = KItemsPos.GetList();
            //var  = KCompoundResult.GetList();
            var resultList = KCompoundResult.GetScrollRect().Content.GetList();

            // var uiItem3 = itemList.GetChildAt(3).GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
            // itemList.GetChildAt(1).SetActive(true);
            // uiItem3.SetActive(true);
            // uiItem3.Initialize(myGameEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);
            GetFromReference(KMid).GetRectTransform().SetAnchoredPositionY(230);
            if (add)
            {
                GetFromReference(KMid).GetRectTransform().SetAnchoredPositionY(130);
                if (selectedNum != 1)
                {
                    foreach (var parent in itemList.Children)
                    {
                        var ui = parent.GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
                        var KAphla = ui.GetFromReference(UICommon_EquipItem.KAphla);
                        var KBtn_Item = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);

                        if (KAphla.GameObject.activeSelf)
                        {
                            if (parent.GameObject.transform.GetSiblingIndex() == 1)
                            {
                                KImg_LeftLine.SetActive(true);
                                //KImg_RightLine.SetActive(false);
                            }


                            ui.Initialize(myGameEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);
                            KBtn_Item.GetXButton().SetEnabled(true);
                            KAphla.SetActive(false);
                            parent.SetActive(true);
                            ui.SetActive(true);
                            break;
                        }
                    }
                }
                else
                {
                    //name
                    var reward = new Vector3(11, equipId0 * 100 + equipQuality0, 1);
                    var curName = JiYuUIHelper.GetRewardName(reward);
                    //var KText_EquipName = this.GetFromReference(UIPanel_Compound.KText_EquipName);
                    //KText_EquipName.SetActive(true);
                    //KText_EquipName.GetTextMeshPro().SetTMPText(curName);

                    //KText_EquipName.GetTextMeshPro().SetFontStyle(FontStyles.Bold);
                    //

                    var uiItem = itemList.GetChildAt(0)
                        .GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
                    itemList.GetChildAt(0).SetActive(true);
                    uiItem.SetActive(true);
                    uiItem.Initialize(myGameEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);

                    if (!isMaxError)
                    {
                        //
                        var itemAphla1 = itemList.GetChildAt(1);
                        var aphlaui1 = itemAphla1.GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
                        var KImg_IsS1 = aphlaui1.GetFromReference(UICommon_EquipItem.KImg_IsS);
                        var KImg_Quality1 = aphlaui1.GetFromReference(UICommon_EquipItem.KImg_Quality);
                        var KText_Grade1 = aphlaui1.GetFromReference(UICommon_EquipItem.KText_Grade);
                        var KAphla1 = aphlaui1.GetFromReference(UICommon_EquipItem.KAphla);
                        var KImg_ItemIcon1 = aphlaui1.GetFromReference(UICommon_EquipItem.KImg_ItemIcon);
                        var KBtn_Item1 = aphlaui1.GetFromReference(UICommon_EquipItem.KBtn_Item);
                        itemAphla1.SetActive(true);
                        aphlaui1.SetActive(true);
                        var itemAphlaEquip1 = new EquipDto()
                        {
                            PartId = myGameEquip0.equip.PartId,
                            //RoleId = myGameEquip0.equip.RoleId,
                            EquipId = myGameEquip0.equip.EquipId,
                            //PosId = myGameEquip0.equip.PosId,
                            Quality = myGameEquip0.equip.Quality,
                            EquipLevel = myGameEquip0.equip.EquipLevel,
                            //MainEntryId = myGameEquip0.equip.MainEntryId,
                            //MainEntryInit = myGameEquip0.equip.MainEntryInit,
                            //MainEntryGrow = myGameEquip0.equip.MainEntryGrow,
                        };
                        var itemAphlaMyGameEquip1 = new MyGameEquip
                        {
                            equip = itemAphlaEquip1,
                            isWearing = myGameEquip0.isWearing,
                            canCompound = myGameEquip0.canCompound,
                        };
                        itemAphlaMyGameEquip1.equip.EquipLevel = 1;

                        if (mergeSelf0 == 1)
                        {
                            aphlaui1.Initialize(itemAphlaMyGameEquip1,
                                UIPanel_Equipment.EquipPanelType.ComposeSelected);
                            // KImg_IsS1.SetActive(true);
                            // KImg_Quality1.SetActive(true);
                            // KText_Grade1.SetActive(true);
                        }
                        else
                        {
                            itemAphlaMyGameEquip1.equip.Quality = needQua;
                            aphlaui1.Initialize(itemAphlaMyGameEquip1,
                                UIPanel_Equipment.EquipPanelType.ComposeSelected);
                            KImg_ItemIcon1.GetImage().SetSpriteAsync(equip_posConfig.Get(equip_data0.posId).pic, false)
                                .Forget();
                            KImg_IsS1.SetActive(false);
                            KImg_Quality1.SetActive(false);
                            KText_Grade1.SetActive(false);
                        }

                        KBtn_Item1.GetXButton().SetEnabled(false);
                        KAphla1.SetActive(true);


                        if (needCount >= 2)
                        {
                            var itemAphla2 = itemList.GetChildAt(2);
                            var aphlaui2 = itemAphla2.GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);

                            var KImg_IsS = aphlaui2.GetFromReference(UICommon_EquipItem.KImg_IsS);
                            var KImg_Quality = aphlaui2.GetFromReference(UICommon_EquipItem.KImg_Quality);
                            var KText_Grade = aphlaui2.GetFromReference(UICommon_EquipItem.KText_Grade);
                            var KAphla2 = aphlaui2.GetFromReference(UICommon_EquipItem.KAphla);
                            var KImg_ItemIcon2 = aphlaui2.GetFromReference(UICommon_EquipItem.KImg_ItemIcon);
                            var KBtn_Item2 = aphlaui2.GetFromReference(UICommon_EquipItem.KBtn_Item);
                            itemAphla2.SetActive(true);
                            aphlaui2.SetActive(true);
                            var itemAphlaEquip = new EquipDto()
                            {
                                PartId = myGameEquip0.equip.PartId,
                                //RoleId = myGameEquip0.equip.RoleId,
                                EquipId = myGameEquip0.equip.EquipId,
                                //PosId = myGameEquip0.equip.PosId,
                                Quality = myGameEquip0.equip.Quality,
                                EquipLevel = myGameEquip0.equip.EquipLevel,
                                //MainEntryId = myGameEquip0.equip.MainEntryId,
                                //MainEntryInit = myGameEquip0.equip.MainEntryInit,
                                //MainEntryGrow = myGameEquip0.equip.MainEntryGrow,
                            };
                            var itemAphlaMyGameEquip = new MyGameEquip
                            {
                                equip = itemAphlaEquip,
                                isWearing = myGameEquip0.isWearing,
                                canCompound = myGameEquip0.canCompound,
                            };
                            itemAphlaMyGameEquip.equip.EquipLevel = 1;

                            if (mergeSelf0 == 1)
                            {
                                aphlaui2.Initialize(itemAphlaMyGameEquip,
                                    UIPanel_Equipment.EquipPanelType.ComposeSelected);
                                // KImg_IsS.SetActive(true);
                                // KImg_Quality.SetActive(true);
                                // KText_Grade.SetActive(true);
                            }
                            else
                            {
                                itemAphlaMyGameEquip.equip.Quality = needQua;
                                aphlaui2.Initialize(itemAphlaMyGameEquip,
                                    UIPanel_Equipment.EquipPanelType.ComposeSelected);
                                KImg_ItemIcon2.GetImage()
                                    .SetSpriteAsync(equip_posConfig.Get(equip_data0.posId).pic, false)
                                    .Forget();
                                KImg_IsS.SetActive(false);
                                KImg_Quality.SetActive(false);
                                KText_Grade.SetActive(false);
                            }

                            KBtn_Item2.GetXButton().SetEnabled(false);
                            KAphla2.SetActive(true);
                        }


                        //
                        var uiItem3 = itemList.GetChildAt(3)
                            .GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
                        itemList.GetChildAt(3).SetActive(true);
                        uiItem3.SetActive(true);
                        var newequip = new EquipDto()
                        {
                            PartId = myGameEquip.equip.PartId,
                            //RoleId = myGameEquip.equip.RoleId,
                            EquipId = myGameEquip.equip.EquipId,
                            //PosId = myGameEquip.equip.PosId,
                            Quality = myGameEquip.equip.Quality,
                            //Level = myGameEquip.equip.EquipLevel,
                            //MainEntryId = myGameEquip.equip.MainEntryId,
                            //MainEntryInit = myGameEquip.equip.MainEntryInit,
                            //MainEntryGrow = myGameEquip.equip.MainEntryGrow,
                        };
                        var upQuaEquip = new MyGameEquip
                        {
                            equip = newequip,
                            isWearing = myGameEquip.isWearing,
                            canCompound = myGameEquip.canCompound,
                        };
                        upQuaEquip.equip.Quality++;
                        uiItem3.Initialize(upQuaEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);

                        KText_MaxQualityError.SetActive(false);

                        //合成预览词条列表
                        KCompoundResult.SetActive(true);
                        resultList.Clear();
                        //var item3 = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTIT, 1, false);

                        //var KHorizontal3 = item3.GetFromReference(UISubPanel_TTIT.KHorizontal);
                        //var KText_AllRow3 = item3.GetFromReference(UISubPanel_TTIT.KText_AllRow);

                        //KHorizontal3.SetActive(false);
                        //KText_AllRow3.SetActive(true);
                        //equipId = upQuaEquip.equip.EquipId * 100 + upQuaEquip.equip.Quality;
                        var name = JiYuUIHelper.GetRewardName(new Vector3(11, equipId, 1));
                        SetTitle(name);
                        //KText_AllRow3.GetTextMeshPro().SetTMPText(name);
                        //KText_AllRow3.GetRectTransform().SetOffsetWithLeft(0);
                        //KText_AllRow3.GetRectTransform().SetOffsetWithRight(0);
                        //item3.GetImage().SetEnabled(false);
                        //KText_AllRow3.GetTextMeshPro().Get().alignment = TextAlignmentOptions.Center;
                        //KText_AllRow3.GetTextMeshPro().SetFontStyle(FontStyles.Bold);
                        var item = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTIT, 2, false);
                        var KText_Left = item.GetFromReference(UISubPanel_TTIT.KText_Left);
                        var KText_Mid = item.GetFromReference(UISubPanel_TTIT.KText_Mid);
                        //var KImg_Mid = item.GetFromReference(UISubPanel_TTIT.KImg_Mid);
                        var KText_Right = item.GetFromReference(UISubPanel_TTIT.KText_Right);
                        var KHorizontal = item.GetFromReference(UISubPanel_TTIT.KHorizontal);
                        var KText_AllRow = item.GetFromReference(UISubPanel_TTIT.KText_AllRow);

                        KHorizontal.SetActive(true);
                        KText_AllRow.SetActive(false);

                        //KText_AllRow3.GetRectTransform().SetHeight(  KText_AllRow3.GetTextMeshPro().Get().preferredHeight);
                        var item1 = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTIT, 3, false);
                        var KText_Left1 = item1.GetFromReference(UISubPanel_TTIT.KText_Left);
                        var KText_Mid1 = item1.GetFromReference(UISubPanel_TTIT.KText_Mid);
                        //var KImg_Mid1 = item1.GetFromReference(UISubPanel_TTIT.KImg_Mid);
                        var KText_Right1 = item1.GetFromReference(UISubPanel_TTIT.KText_Right);
                        var KHorizontal1 = item1.GetFromReference(UISubPanel_TTIT.KHorizontal);
                        var KText_AllRow1 = item1.GetFromReference(UISubPanel_TTIT.KText_AllRow);

                        KHorizontal1.SetActive(true);
                        KText_AllRow1.SetActive(false);
                        item1.GetImage().SetEnabled(true);
                        switch (equip_data0.mainEntryId)
                        {
                            case 103010:
                                Log.Debug("atk", Color.cyan);
                                KText_Left.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_atk").current);
                                //KImg_Mid.GetImage().SetSpriteAsync("icon_equip_tips_atk", false).Forget();
                                break;
                            case 102010:
                                Log.Debug("attr_hp", Color.cyan);
                                KText_Left.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_hp").current);
                                //KImg_Mid.GetImage().SetSpriteAsync("icon_equip_tips_heart", false).Forget();
                                break;
                            case 402:
                                KText_Left.GetTextMeshPro().SetTMPText(languageConfig.Get("attr_speed").current);
                                //KImg_Mid.GetImage().SetSpriteAsync("icon_equip_tips_move", false).Forget();
                                break;
                        }

                        var atkNum0 = equip_data0.mainEntryInit + (equipLevel0 - 1) * equip_data0.mainEntryGrow;
                        KText_Mid.GetTextMeshPro().SetTMPText(atkNum0.ToString());
                        equipId = upQuaEquip.equip.EquipId * 100 + upQuaEquip.equip.Quality;
                        var newequip_data = equip_dataConfig.Get(equipId);

                        var newequip_quality = equip_qualityConfig.Get(newequip_data.quality);
                        var newquality = qualityConfig.Get(newequip_quality.type);
                        var atkNum = newequip_data.mainEntryInit +
                                     (upQuaEquip.equip.EquipLevel - 1) * newequip_data.mainEntryGrow;
                        KText_Right.GetTextMeshPro().SetTMPText(atkNum.ToString());

                        KText_Left1.GetTextMeshPro()
                            .SetTMPText(languageConfig.Get("attr_info_level_upperlimit").current);
                        KText_Mid1.GetTextMeshPro()
                            .SetTMPText(equip_quality0.levelMax.ToString());
                        KText_Right1.GetTextMeshPro()
                            .SetTMPText(newequip_quality.levelMax.ToString());

                        var item2 = await resultList.CreateWithUITypeAsync(UIType.UISubPanel_TTIT, 4, false);

                        var KHorizontal2 = item2.GetFromReference(UISubPanel_TTIT.KHorizontal);
                        var KText_AllRow2 = item2.GetFromReference(UISubPanel_TTIT.KText_AllRow);

                        KHorizontal2.SetActive(false);
                        KText_AllRow2.SetActive(true);
                        item2.GetImage().SetEnabled(true);
                        var skillGroup = newequip_data.minorSkillGroup;

                        int skillid = -1;
                        for (int i = 0; i < skillGroup.Count; i++)
                        {
                            if ((int)skillGroup[i].x == newquality.id)
                            {
                                skillid = (int)skillGroup[i].y;
                                break;
                            }
                        }

                        var playerSkill = player_skillConfig.GetOrDefault((int)skillid);


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
                        //string skillDes = default;
                        // if (playerSkill == null)
                        // {
                        //     skillDes = "key is not exist";
                        // }
                        // else
                        // {
                        //     skillDes = playerSkill.desc;
                        // }

                        KText_AllRow2.GetTextMeshPro().SetTMPText(languageConfig.Get(skillDes).current);
                        item2.GetRectTransform().SetHeight(KText_AllRow2.GetTextMeshPro().Get().preferredHeight);

                        resultList.Sort((a, b) =>
                        {
                            var uia = a as UISubPanel_TTIT;
                            var uib = b as UISubPanel_TTIT;
                            return uia.index.CompareTo(uib.index);
                        });
                        await UniTask.Yield();
                        KCompoundResult.GetScrollRect().SetVerticalNormalizedPosition(1);
                    }
                    else
                    {
                        var uiItem3 = itemList.GetChildAt(3)
                            .GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
                        itemList.GetChildAt(3).SetActive(false);
                        uiItem3.SetActive(false);
                        KText_MaxQualityError.SetActive(true);
                    }
                }
            }
            else
            {
                foreach (var parent in itemList.Children)
                {
                    var ui = parent.GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);

                    if (ui.uid == uid)
                    {
                        if (parent.GameObject.transform.GetSiblingIndex() == 1)
                        {
                            KImg_LeftLine.SetActive(false);
                            //KImg_RightLine.SetActive(false);
                        }


                        var KImg_IsS = ui.GetFromReference(UICommon_EquipItem.KImg_IsS);
                        var KImg_Quality = ui.GetFromReference(UICommon_EquipItem.KImg_Quality);
                        var KText_Grade = ui.GetFromReference(UICommon_EquipItem.KText_Grade);
                        var KAphla = ui.GetFromReference(UICommon_EquipItem.KAphla);
                        var KImg_ItemIcon = ui.GetFromReference(UICommon_EquipItem.KImg_ItemIcon);
                        var KBtn_Item = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);

                        var itemAphlaEquip = new EquipDto()
                        {
                            PartId = myGameEquip0.equip.PartId,
                            //RoleId = myGameEquip0.equip.RoleId,
                            EquipId = myGameEquip0.equip.EquipId,
                            //PosId = myGameEquip0.equip.PosId,
                            Quality = myGameEquip0.equip.Quality,
                            EquipLevel = myGameEquip0.equip.EquipLevel,
                            //MainEntryId = myGameEquip0.equip.MainEntryId,
                            //MainEntryInit = myGameEquip0.equip.MainEntryInit,
                            //MainEntryGrow = myGameEquip0.equip.MainEntryGrow,
                        };
                        var itemAphlaMyGameEquip = new MyGameEquip
                        {
                            equip = itemAphlaEquip,
                            isWearing = myGameEquip0.isWearing,
                            canCompound = myGameEquip0.canCompound,
                        };
                        parent.SetActive(true);
                        ui.SetActive(true);
                        itemAphlaMyGameEquip.equip.EquipLevel = 1;
                        if (mergeSelf0 == 1)
                        {
                            ui.Initialize(itemAphlaMyGameEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);

                            // KImg_IsS.SetActive(true);
                            // KImg_Quality.SetActive(true);
                            // KText_Grade.SetActive(true);
                        }
                        else
                        {
                            itemAphlaMyGameEquip.equip.Quality = needQua;
                            ui.Initialize(itemAphlaMyGameEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);
                            KImg_ItemIcon.GetImage().SetSpriteAsync(equip_posConfig.Get(equip_data0.posId).pic, false)
                                .Forget();
                            KImg_IsS.SetActive(false);
                            KImg_Quality.SetActive(false);
                            KText_Grade.SetActive(false);
                        }

                        KBtn_Item.GetXButton().SetEnabled(false);
                        KAphla.SetActive(true);
                        // parent.SetActive(false);
                        // ui.SetActive(false);
                        break;
                    }
                }

                // var uiItem =
                //     itemList.GetChildAt(selectedNum).SetActive(false);
                // uiItem.SetActive(false);
                //uiItem.Initialize(myGameEquip, UIPanel_Equipment.EquipPanelType.ComposeSelected);
            }


            if (selectedNum > needCount)
            {
                var KBtnBg = GetFromReference(UIPanel_Compound.KBtnBg);
                KBtnBg.GetImage().SetSpriteAsync("compoundBtnGreen", false);
                KBtn_Compound.GetImage().SetSpriteAsync("btnLight", false);

                KBtn_Compound.GetButton().SetEnabled(true);
                foreach (var ui in lableList.Children)
                {
                    var uilable = ui as UICommon_Label;
                    if (uilable.type == CommonLabelType.CurCanCompound)
                    {
                        uilable.LockOrUnlockUnSelectedEquip(true);
                        break;
                    }
                }
            }
            else
            {
                KBtn_Compound.GetButton().SetEnabled(false);
                foreach (var ui in lableList.Children)
                {
                    var uilable = ui as UICommon_Label;
                    if (uilable.type == CommonLabelType.CurCanCompound)
                    {
                        uilable.LockOrUnlockUnSelectedEquip(false);
                        break;
                    }
                }
            }
        }

        public void SetTitle(string str)
        {
            this.GetFromReference(KTextTitle).SetActive(true);
            this.GetTextMeshPro(KTextTitle).SetTMPText(str);
        }


        void InitTopPanel()
        {
            var KItemsPos = GetFromReference(UIPanel_Compound.KItemsPos);
            //var KText_EquipName = this.GetFromReference(UIPanel_Compound.KText_EquipName);

            var KBtn_Compound = GetFromReference(UIPanel_Compound.KBtn_Compound);
            var KBtnBg = GetFromReference(UIPanel_Compound.KBtnBg);
            var KImg_LeftLine = GetFromReference(UIPanel_Compound.KImg_LeftLine);
            var KText_MaxQualityError = GetFromReference(UIPanel_Compound.KText_MaxQualityError);
            var KCompoundResult = GetFromReference(UIPanel_Compound.KCompoundResult);

            //KText_EquipName.SetActive(false);
            KBtnBg.GetImage().SetSpriteAsync("compoundBtnRed", false);
            KBtn_Compound.GetImage().SetSpriteAsync("btnNoLight", false);
            KBtn_Compound.GetButton().SetEnabled(false);
            KImg_LeftLine.SetActive(false);
            KText_MaxQualityError.SetActive(false);
            KCompoundResult.SetActive(false);
            var itemList = KItemsPos.GetList();
            var ui0 = itemList.GetChildAt(0).GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
            var ui3 = itemList.GetChildAt(3).GetChild<UICommon_EquipItem>(UIType.UICommon_EquipItem);
            var ui31 = itemList.GetChildAt(3);
            var ui1 = itemList.GetChildAt(1);
            var ui2 = itemList.GetChildAt(2);

            ui0.SetActive(false);
            ui3.SetActive(false);
            ui1.SetActive(false);
            ui2.SetActive(false);
            ui31.SetActive(true);
        }


        public void RefreshCompoundPanel()
        {
            //TODO:测试刷新页面
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out var ui))
            {
                var uiPanel = ui as UIPanel_Compound;
                var lastSortBy = uiPanel.curSortBy;
                uiPanel.curSortBy = CommonLabelType.Paper;
                uiPanel.OnClickActionEvent(lastSortBy);
                uiPanel.RefreshCompound();
            }
        }

        public void RefreshCompound()
        {
            var KBtn_AllCompound = GetFromReference(UIPanel_Compound.KBtn_AllCompound);
            JiYuUIHelper.SetGrayOrNot(KBtn_AllCompound, "common_button_gray2", "icon_btn_yellow_3",
                !CanAllCompound(out var _));
        }

        async UniTaskVoid InitPanel()
        {
            //equip_merge_all全部装备
            //WebMessageHandlerOld.Instance.AddHandler(CMD.EQUIPALLCOMPOUND, OnEquipAllCompoundResponse);

            var attr_variableConfig = ConfigManager.Instance.Tables.Tbattr_variable;
            var equip_levelConfig = ConfigManager.Instance.Tables.Tbequip_level;
            var player_skill = ConfigManager.Instance.Tables.Tbskill;
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;

            var KScrollView_item = GetFromReference(UIPanel_Compound.KScrollView_item);
            var KContent = GetFromReference(UIPanel_Compound.KContent);
            var KCommon_Bottom = GetFromReference(UIPanel_Compound.KCommon_Bottom);


            var KBtn_Sort = GetFromReference(UIPanel_Compound.KBtn_Sort);
            var KText_Sort = GetFromReference(UIPanel_Compound.KText_Sort);
            var KBtn_AllCompound = GetFromReference(UIPanel_Compound.KBtn_AllCompound);
            var KText_AllCompound = GetFromReference(UIPanel_Compound.KText_AllCompound);
            var KSortList = GetFromReference(UIPanel_Compound.KSortList);
            var KTip = GetFromReference(UIPanel_Compound.KTip);
            var KIsSelected = GetFromReference(UIPanel_Compound.KIsSelected);
            var KText_IsSelectedTitle = GetFromReference(UIPanel_Compound.KText_IsSelectedTitle);
            var KBtn_IsSelected = GetFromReference(UIPanel_Compound.KBtn_IsSelected);
            var KText_IsSelected = GetFromReference(UIPanel_Compound.KText_IsSelected);
            var KBtn_Compound = GetFromReference(UIPanel_Compound.KBtn_Compound);
            var KText_Compound = GetFromReference(UIPanel_Compound.KText_Compound);
            var KText_MaxQualityError = GetFromReference(UIPanel_Compound.KText_MaxQualityError);
            var KImg_LeftLine = GetFromReference(UIPanel_Compound.KImg_LeftLine);
            var KItemsPos = GetFromReference(UIPanel_Compound.KItemsPos);
            var KText_AddItem = GetFromReference(UIPanel_Compound.KText_AddItem);
            var KCompoundResult = GetFromReference(UIPanel_Compound.KCompoundResult);
            var KTextNoEquip = GetFromReference(UIPanel_Compound.KTextNoEquip);
            var KTextTitle = GetFromReference(UIPanel_Compound.KTextTitle);

            var KBtn_Close = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_Close);
            var KBtn_TitleInfo = KCommon_Bottom.GetFromReference(UICommon_Bottom.KBtn_TitleInfo);
            var KText_BottomTitle = KCommon_Bottom.GetFromReference(UICommon_Bottom.KText_BottomTitle);


            KTextNoEquip.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_tip").current);
            KText_AllCompound.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_mergeclick").current);
            KText_BottomTitle.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_title").current);
            KText_IsSelectedTitle.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_option").current);
            KText_IsSelected.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_cancel").current);
            KText_Compound.GetTextMeshPro().SetTMPText(languageConfig.Get("common_state_merge").current);
            KText_AddItem.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_tip").current);
            KText_Sort.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_all").current);
            KText_MaxQualityError.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_max_text").current);

            KTip.SetActive(false);
            KIsSelected.SetActive(false);
            KBtn_Compound.GetButton().SetEnabled(false);
            KImg_LeftLine.SetActive(false);
            KText_MaxQualityError.SetActive(false);
            KCompoundResult.SetActive(false);
            KTextTitle.SetActive(false);
            KTextNoEquip.SetActive(true);
            JiYuUIHelper.SetGrayOrNot(KBtn_AllCompound, "common_button_gray2", "icon_btn_yellow_3",
                !CanAllCompound(out var _));

            var mygameequip = new MyGameEquip
            {
                equip = new EquipDto()
                {
                    PartId = -1,
                    //RoleId = 0,
                    EquipId = 101,
                    //PosId = 0,
                    Quality = 1,
                    EquipLevel = 1,
                    //MainEntryId = 0,
                    //MainEntryInit = 0,
                    //MainEntryGrow = 0,
                },
                isWearing = false,
                canCompound = false
            };

            var itemList = KItemsPos.GetList();
            itemList.Clear();
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                Transform child = itemList.Get().GetChild(index);
                var childPos = child.GetChild(1);
                var parent = itemList.Create(child.gameObject, true);
                var ui = await UIHelper.CreateAsync(parent, UIType.UICommon_EquipItem, mygameequip,
                    UIPanel_Equipment.EquipPanelType.Main,
                    childPos) as UICommon_EquipItem;
                var uiBtn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);

                parent.AddChild(ui);
                var rect = ui.GetRectTransform();
                rect.SetAnchoredPosition(0, 0);
                rect.SetPivot(new Vector2(0.5f, 0.5f));
                rect.SetAnchorMax(Vector2.one);
                rect.SetAnchorMin(Vector2.zero);
                rect.SetOffset(0, 0, 0, 0);
                switch (index)
                {
                    case 0:
                        ui.SetActive(false);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(uiBtn, () =>
                        {
                            var lastSortBy = curSortBy;
                            curSortBy = CommonLabelType.Paper;
                            OnClickActionEvent(lastSortBy);
                        });
                        break;
                    case 1:
                        parent.SetActive(false);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(uiBtn, () => { OnSelected(ui.uid); });
                        break;
                    case 2:
                        parent.SetActive(false);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(uiBtn, () => { OnSelected(ui.uid); });
                        break;
                    case 3:
                        ui.SetActive(false);
                        break;
                }
            }

            // foreach (var VARIABLE in itemList.Children)
            // {
            //     VARIABLE.SetActive(false);
            // }


            KBtn_Close.GetRectTransform().SetScale(Vector2.one);


            var sortList = KSortList.GetList();
            sortList.Clear();

            for (int i = 0; i < 7; i++)
            {
                int index = i;
                var ui =
                    await sortList.CreateWithUITypeAsync(UIType.UISubPanel_CommonBtn,
                        false) as UISubPanel_CommonBtn;
                var KText_Mid = ui.GetFromReference(UISubPanel_CommonBtn.KText_Mid);
                var KBtn_Common = ui.GetFromReference(UISubPanel_CommonBtn.KBtn_Common);
                var KImg_Btn = ui.GetFromReference(UISubPanel_CommonBtn.KImg_Btn);
                KText_Mid.SetActive(true);

                if (index != 0)
                {
                    var equipPos = equip_posConfig.Get(index);
                    KText_Mid.GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardTextIconName(equipPos.pic) +
                                                          languageConfig.Get(equipPos.name).current);
                    KImg_Btn.GetImage().SetAlpha(0.5f);
                }
                else
                {
                    KImg_Btn.GetImage().SetAlpha(1f);
                    KText_Mid.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_all").current);

                    //KText_Mid.GetTextMeshPro().SetTMPText(JiYuUIHelper.GetRewardTextIconName(equipPos.pic) + languageConfig.Get(equipPos.name).current);
                }

                KImg_Btn.GetImage().SetSprite("common_blue_button_8", false);

                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Common,
                    () =>
                    {
                        if (curSortBy == (CommonLabelType)index)
                        {
                            KTip.SetActive(false);
                            return;
                        }

                        sortList.GetChildAt((int)curSortBy).GetFromReference(UISubPanel_CommonBtn.KImg_Btn)
                            .GetImage()
                            .SetAlpha(0.5f);
                        //curSortBy = (CommonLabelType)index;

                        OnClickActionEvent((CommonLabelType)index);
                        KTip.SetActive(false);
                        KImg_Btn.GetImage().SetAlpha(1f);
                        if (index != 0)
                        {
                            var equipPos = equip_posConfig.Get(index);
                            KText_Sort.GetTextMeshPro().SetTMPText(languageConfig.Get(equipPos.name).current);
                        }
                        else
                        {
                            KText_Sort.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_all").current);
                        }
                    });
            }

            KTip.GetRectTransform().SetWidth(300);
            KTip.GetRectTransform().SetHeight(641);

            OnClickActionEvent(CommonLabelType.AllEquip);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Close, () => { Close(); });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Sort,
                () =>
                {
                    JiYuUIHelper.DestoryAllTips();
                    ;
                    KTip.SetActive(!KTip.GameObject.activeSelf);
                });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_IsSelected,
                () =>
                {
                    //KIsSelected.SetActive(false);
                    var lastSortBy = curSortBy;
                    curSortBy = CommonLabelType.Paper;
                    OnClickActionEvent(lastSortBy);
                });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_Compound,
                async () =>
                {
                    long uid0 = 0;
                    foreach (var kv in selectedEquips)
                    {
                        if (kv.Value == 1)
                        {
                            uid0 = kv.Key;
                            break;
                        }
                    }

                    ByteValueList valueList = new ByteValueList();
                    var list = new List<MyGameEquip>();
                    foreach (var item in selectedEquips)
                    {
                        if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(item.Key,
                                out var myGameEquip))
                        {
                            Log.Error($"uid:{item.Key} is not exist");
                            continue;
                        }

                        list.Add(myGameEquip);

                        valueList.Values.Add(myGameEquip.equip.ToByteString());
                    }

                    ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid0,
                        out var myGameEquip0);

                    equipMain = myGameEquip0;
                    var ui =
                        await UIHelper.CreateAsync<List<MyGameEquip>>(UIType.UIPanel_CompoundNormalDongHua, list) as
                            UIPanel_CompoundNormalDongHua;
                    //await SetEffect(list, myGameEquip0, ui, cts);
                    //SetSuccess(ui, list, valueList, myGameEquip0).Forget();
                });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_AllCompound,
                async () =>
                {
                    JiYuUIHelper.DestoryAllTips();
                    ;
                    OnAllCompoundClick();
                });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(KBtn_TitleInfo,
                async () =>
                {
                    //if (JiYuUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
                    //{
                    //    JiYuUIHelper.DestoryAllTips();
                    //    ;
                    //    return;
                    //}
                    //UIManager

                 
                    var itemTips = GetFromReference(KCommon_ItemTips);
                    itemTips.SetActive(!itemTips.GameObject.activeSelf);
                    var KTxt_Des = GetFromReference(KText_Des);
                    KTxt_Des.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_tips").current);

                    // var titleRect = KTxt_Title.GetRectTransform();
                    // var desRect = KTxt_Des.GetRectTransform();
                    //
                    // titleRect.SetHeight(KTxt_Title.GetTextMeshPro().Get().preferredHeight + 10f);
                    // desRect.SetHeight(KTxt_Des.GetTextMeshPro().Get().preferredHeight + 10f);
                    // titleRect.SetOffsetWithLeft(15f);
                    // titleRect.SetOffsetWithRight(-15f);
                    // desRect.SetOffsetWithLeft(15f);
                    // desRect.SetOffsetWithRight(-15f);
                    // var contentHeight = KTxt_Title.GetRectTransform().Height() +
                    //                     KTxt_Des.GetRectTransform().Height() + 22f;
                    // KContent.GetRectTransform().SetHeight(contentHeight);

                    //await UniTask.Yield();

                    // Log.Error($"{title.GetTextMeshPro().Get().maxHeight} {des.GetTextMeshPro().Get().maxHeight}");
                    // Log.Error(
                    //     $"{title.GetTextMeshPro().Get().preferredHeight} {des.GetTextMeshPro().Get().preferredHeight}");
                    //JiYuUIHelper.SetTipPosAndResize(KBtn_TitleInfo, tipUI);
                    // 

                    //

                    //await UniTask.DelayFrame(3);
                });


            this.GetXButton().OnClick.Add(() =>
            {
                JiYuUIHelper.DestoryAllTips();
                ;
                KTip.SetActive(false);
            });
        }



        private async UniTask<AsyncUnit> SetEffect(List<MyGameEquip> list, MyGameEquip myGameEquip0,
            UIPanel_CompoundNormalDongHua ui, CancellationTokenSource cts)
        {
            if (ui == null)
            {
                return AsyncUnit.Default;
            }

            ui.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosTwo).SetActive(false);
            ui.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosThree).SetActive(false);
            if (list.Count == 2)
            {
                var two = ui.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosTwo);
                two.SetActive(true);

                for (int i = 0; i < list.Count; i++)
                {
                    var equipData = list[i];
                    if (i == 0)
                    {
                        equipData = myGameEquip0;
                    }
                    else
                    {
                    }

                    var go = two.GetRectTransform().GetChild(i).gameObject;
                    var item = two.GetList().Create(go, true);
                    JiYuUIHelper.SetEquipIcon(equipData, item, EquipPanelType.Compose);

                    Effect1(cts, i, item).Forget();
                }

                return AsyncUnit.Default;
            }
            else
            {
                var three = ui.GetFromReference(UIPanel_CompoundNormalDongHua.KEquipPosThree);
                three.SetActive(true);

                for (int i = 0; i < list.Count; i++)
                {
                    var equipData = list[i];
                    if (i == 0)
                    {
                        equipData = myGameEquip0;
                    }

                    var go = three.GetRectTransform().GetChild(i).gameObject;
                    var item = three.GetList().Create(go, true);
                    JiYuUIHelper.SetEquipIcon(equipData, item, EquipPanelType.Compose);

                    Effect2(cts, i, item).Forget();
                }
            }

            await UniTask.Delay(1000, cancellationToken: cts.Token);
            return AsyncUnit.Default;
        }

        private async UniTask<AsyncUnit> Effect2(CancellationTokenSource cts, int i, UI item)
        {
            try
            {
                await UniTask.Delay(1, cancellationToken: cts.Token);
                if (i == 0)
                {
                    //item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<Transform>()
                        .DOScale(1.5f, 1.8f).SetEase(Ease.InQuad);
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()
                        .DOAnchorPosY(372f, 1.8f).SetEase(Ease.InQuad);
                }
                else
                {
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha =
                        1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>()
                        .DOFade(0, 1.8f).SetEase(Ease.InQuad);
                    StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                        item.GetRectTransform().Rotation());
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                item.GetComponent<RectTransform>()?.DOComplete();
                item.GetComponent<CanvasGroup>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        private async UniTask<AsyncUnit> Effect1(CancellationTokenSource cts, int i, UI item)
        {
            try
            {
                await UniTask.Delay(1, cancellationToken: cts.Token);
                if (i == 0)
                {
                    //item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<Transform>()
                        .DOScale(1.5f, 1.8f).SetEase(Ease.InQuad);
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<RectTransform>()
                        .DOAnchorPosY(372f, 1.8f).SetEase(Ease.InQuad);
                }
                else
                {
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>().alpha =
                        1;
                    item.GetFromReference(UICommon_RewardItem.KBtn_Item).GetComponent<CanvasGroup>()
                        .DOFade(0, 1.8f).SetEase(Ease.InQuad);
                    StartShake(item, item.GetRectTransform().AnchoredPosition3D(),
                        item.GetRectTransform().Rotation());
                }

                return AsyncUnit.Default;
            }
            catch (OperationCanceledException)
            {
                item.GetComponent<RectTransform>()?.DOComplete();
                item.GetComponent<CanvasGroup>()?.DOComplete();
                Log.Debug("Animation cancelled", Color.yellow);
                return AsyncUnit.Default;
            }
        }

        public void StartShake(UI ui, Vector3 originalPosition, Quaternion originalRotation,
            float rotationStrength = 8f, float shakeDuration = 2f, float shakeStrength = 18f, int vibrato = 8,
            float randomness = 45f)
        {
            isShakeEnd = false;
            var uiElement = ui.GetComponent<RectTransform>();
            // 停止之前的动画（避免叠加）
            // 创建一个 Sequence 来同时执行位置和旋转抖动
            Sequence shakeSequence = DOTween.Sequence();

            // 添加位置抖动（左右）
            shakeSequence.Join(uiElement.DOShakePosition(
                duration: shakeDuration,
                strength: new Vector3(shakeStrength, 0, 0), // 只在 X 轴抖动
                vibrato: vibrato,
                randomness: randomness,
                snapping: false,
                fadeOut: true
            ));

            // 添加旋转抖动（正负 m 度，围绕 Z 轴）
            shakeSequence.Join(uiElement.DOShakeRotation(
                duration: shakeDuration,
                strength: new Vector3(0, 0, rotationStrength), // 只围绕 Z 轴旋转
                vibrato: vibrato,
                randomness: randomness,
                fadeOut: true
            ));


            // 确保动画结束时恢复原始位置和旋转
            shakeSequence.OnComplete(() =>
            {
                uiElement.anchoredPosition3D = originalPosition;
                uiElement.rotation = originalRotation;
                isShakeEnd = true;
            });
        }

      


        private void OnAllCompoundClick()
        {
            CanAllCompound(out var list);
            UIHelper.CreateAsync<List<MyGameEquip>>(UIType.UIPanel_CompoundDongHua, list).Forget();
        }

        bool CanAllCompound(out List<MyGameEquip> canCompoundList)
        {
            canCompoundList = ResourcesSingleton.Instance.equipmentData.equipments.Values.Where(
                a => a.canCompound && a.equip.Quality <= 3).ToList();

            // foreach (var kv in ResourcesSingleton.Instance.equipmentData.equipments)
            // {
            //     if (kv.Value.canCompound && kv.Value.equip.Quality <= 3)
            //     {
            //         canAllCompound = true;
            //         break;
            //     }
            // }

            return canCompoundList.Count > 0;
        }

        private async UniTaskVoid SpawnSelectedCompound(long uid, CancellationToken cct)
        {
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;

            var KText_Sort = GetFromReference(UIPanel_Compound.KText_Sort);
            var KContent = GetFromReference(UIPanel_Compound.KContent);
            //KText_Sort.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_all").current);

            var list = KContent.GetList();
            list.Clear();
            List<UniTask> tasks = new List<UniTask>();
            ResourcesSingleton.Instance.equipmentData.isCompoundSort = false;

            //按品质排序的装备
            var ui =
                await list.CreateWithUITypeAsync(UIType.UICommon_Label, CommonLabelType.CurCanCompound, false, false,
                        cct) as
                    UICommon_Label;
            var ui0 =
                await list.CreateWithUITypeAsync(UIType.UICommon_Label, CommonLabelType.SamePosIdCompound, true, false,
                        cct) as
                    UICommon_Label;


            var task = ui.InitEquipCompoundSelected(CommonLabelType.CurCanCompound, uid, cct);
            var task0 = ui0.InitEquipCompoundSelected(CommonLabelType.SamePosIdCompound, uid, cct);

            tasks.Add(task);
            tasks.Add(task0);
            SortLabels();
            await UniTask.WhenAll(tasks);
            ForceRefreshLayout();
            isDone = true;
            tasks.Clear();
        }

        void ForceRefreshLayout()
        {
            var KContent = GetFromReference(UIPanel_Compound.KContent);
            JiYuUIHelper.ForceRefreshLayout(KContent);
        }

        private void SortLabels()
        {
            var KContent = GetFromReference(UIPanel_Compound.KContent);
            var list = KContent.GetList();
            list.Sort((aui, bui) =>
            {
                var a = aui as UICommon_Label;
                var b = bui as UICommon_Label;
                return a.type.CompareTo(b.type);
            });
        }

        private async UniTaskVoid SpawnItems(CommonLabelType type, CancellationToken cct)
        {
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;

            var KText_Sort = GetFromReference(UIPanel_Compound.KText_Sort);
            var KContent = GetFromReference(UIPanel_Compound.KContent);
            //KText_Sort.GetTextMeshPro().SetTMPText(languageConfig.Get("equip_merge_all").current);

            var list = KContent.GetList();
            list.Clear();
            List<UniTask> tasks = new List<UniTask>();
            ResourcesSingleton.Instance.equipmentData.isCompoundSort = true;
            if (panelEquipment.HasCommonLabel(type))
            {
                //按品质排序的装备
                var ui =
                    await list.CreateWithUITypeAsync(UIType.UICommon_Label, type, false, false, cct) as
                        UICommon_Label;

                // var ui = await UIHelper.CreateAsync(this, UIType.UICommon_Label, type, false,
                //     KContent.GameObject.transform, cct) 

                var task = ui.InitEquipCompound(type, cct);
                tasks.Add(task);
            }
            else
            {
                isDone = true;
                tasks.Clear();
                return;
            }


            await UniTask.WhenAll(tasks);
            ForceRefreshLayout();
            isDone = true;
            tasks.Clear();
        }

        //按品质或者部位排序
        // private async UniTaskVoid OnClickEquip(CancellationToken cct)
        // {
        //     //DestroyLables();
        //     if (panelEquipment.EquipIsEmpty())
        //         return;
        //
        //     // var KBottom = GetFromReference(UIPanel_Equipment.KBottom);
        //     // var KText_SortBy = GetFromReference(UIPanel_Equipment.KText_SortBy);
        //     List<UniTask> tasks = new List<UniTask>();
        //
        //     //按照品质排序
        //     if (isAllEquip)
        //     {
        //         if (HasCommonLabel((CommonLabelType)10))
        //         {
        //             KText_SortBy.GetTextMeshPro().SetTMPText(tblanguage.Get("equip_sort_quality").current);
        //             //按品质排序的装备
        //             var ui = await UIHelper.CreateAsync(this, UIType.UICommon_Label, (CommonLabelType)10,
        //                     KBottom.GameObject.transform, cct) as
        //                 UICommon_Label;
        //             var task = ui.InitEquip((CommonLabelType)10, cct);
        //             tasks.Add(task);
        //             UICommon_Labels.Add(ui);
        //         }
        //     } //按照部位排序
        //     else
        //     {
        //         KText_SortBy.GetTextMeshPro().SetTMPText(tblanguage.Get("equip_sort_position").current);
        //
        //         for (int i = 4; i < 10; i++)
        //         {
        //             if (!HasCommonLabel((CommonLabelType)i))
        //                 continue;
        //             var ui =
        //                 await UIHelper.CreateAsync(this, UIType.UICommon_Label, (CommonLabelType)i,
        //                         KBottom.GameObject.transform, cct) as
        //                     UICommon_Label;
        //             UICommon_Labels.Add(ui);
        //             var task = ui.InitEquip((CommonLabelType)i, cct);
        //             tasks.Add(task);
        //         }
        //     }
        //
        //     for (int i = 2; i < 4; i++)
        //     {
        //         if (!HasCommonLabel((CommonLabelType)i))
        //             continue;
        //         var ui =
        //             await UIHelper.CreateAsync(this, UIType.UICommon_Label, (CommonLabelType)i,
        //                     KBottom.GameObject.transform, cct) as
        //                 UICommon_Label;
        //         UICommon_Labels.Add(ui);
        //         var task = ui.InitBag((CommonLabelType)i, cct);
        //         tasks.Add(task);
        //     }
        //
        //     SetBottomPos(UICommon_Labels, false);
        //     await UniTask.WhenAll(tasks);
        //
        //     await UniTask.Yield();
        //     SetBottomPos(UICommon_Labels, true);
        //     await UniTask.Yield();
        //     isDone = true;
        //     tasks.Clear();
        // }

        public void OnClickActionEvent(CommonLabelType type, long uid = 0)
        {
            if (curSortBy == type && uid == 0)
            {
                return;
            }

            var KScrollView_item = GetFromReference(UIPanel_Compound.KScrollView_item);
            var KIsSelected = GetFromReference(UIPanel_Compound.KIsSelected);
            var KBtn_Compound = GetFromReference(UIPanel_Compound.KBtn_Compound);
            var KTextNoEquip = GetFromReference(UIPanel_Compound.KTextNoEquip);
            var KTextTitle = GetFromReference(UIPanel_Compound.KTextTitle);

            var scrollRect = KScrollView_item.GetScrollRect();

            scrollRect.SetVerticalNormalizedPosition(1);
            KBtn_Compound.GetButton().SetEnabled(false);
            if (uid == 0)
            {
                KIsSelected.SetActive(false);
                KTextNoEquip.SetActive(true);
                KTextTitle.SetActive(false);
                curSortBy = type;
            }
            else
            {
                KIsSelected.SetActive(true);
                KTextNoEquip.SetActive(false);
                KTextTitle.SetActive(true);
            }

            InitTopPanel();

            selectedNum = 0;
            selectedEquips.Clear();

            isDone = false;

            //清空内容

            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
            }

            this.cts = new CancellationTokenSource();
            if (uid != 0)
            {
                SpawnSelectedCompound(uid, cts.Token).Forget();
            }
            else
            {
                SpawnItems(type, cts.Token).Forget();
            }
        }


        public override void OnFocus()
        {
        }

        public override void OnBlur()
        {
            //Log.Error($"UIPanel_Equipment OnBlur");
        }

        private void RefreshEquipPanel()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
            {
                var uiequip = ui as UIPanel_Equipment;

                var lastId = uiequip.lastModuleId;
                uiequip.lastModuleId = -1;
                uiequip.OnClickActionEvent(lastId, false);
                uiequip.RefreshAllWearEquip();
            }
        }

        private async void OnEquipAllCompoundResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            EquipResDto resultCraft = new EquipResDto();
            resultCraft.MergeFrom(e.data);
            //RepeatedField<string> repeatedField = new RepeatedField<string>();
            if (e.data.IsEmpty)
            {
                Log.Debug("OnEquipAllCompoundResponse.IsEmpty", Color.red);

                return;
            }

            Log.Debug($"resultCraft {resultCraft}", Color.green);
            //JiYuUIHelper.TurnStrReward2List()
            //resultCraft.EquipDtoList[0].


            NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
            var data = new CompoundSucData
            {
                isAllCompound = true,
                Equips = null
            };
            UIHelper.CreateAsync(UIType.UIPanel_CompoundSuc, data);
            await UniTask.Delay(1000);
            RefreshCompoundPanel();
        }

        protected override void OnClose()
        {
            if (this.cts != null)
            {
                this.cts.Cancel();
                this.cts.Dispose();
            }

            //WebMessageHandlerOld.Instance.RemoveHandler(CMD.EQUIPALLCOMPOUND, OnEquipAllCompoundResponse);

            selectedEquips.Clear();
            RefreshEquipPanel();

            base.OnClose();
        }
    }
}