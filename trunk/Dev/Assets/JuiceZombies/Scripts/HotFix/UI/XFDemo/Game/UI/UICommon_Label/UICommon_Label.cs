//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UICommon_Label)]
    internal sealed class UICommon_LabelEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Label;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Label>();
        }
    }

    public enum CommonLabelType
    {
        Prop = 9,
        Consume = 10,
        Paper = 7,
        GeneralMaterial = 8,
        Weapon = 1,
        Clothing = 2,
        Glove = 3,
        Pants = 4,
        Belt = 5,
        Shoes = 6,
        AllEquip = 0,
        CurCanCompound = 11, //当前可选可合成材料
        SamePosIdCompound = 12, //同部位装备
    }

    public partial class UICommon_Label : UI, IAwake<CommonLabelType, bool>
    {
        private const float MinInterval = 40f;
        public int count;
        public float interval;
        public float cellSize = 164;
        public int cellPerRow;

        public CommonLabelType type;

        List<UICommon_EquipItem> equipItems = new List<UICommon_EquipItem>();

        List<UICommon_RewardItem> rewardItems = new List<UICommon_RewardItem>();
        //Dictionary<GameEquip, long> MaterialDic = new Dictionary<GameEquip, long>();

        public void Initialize(CommonLabelType args, bool isShowTextLabel)
        {
            type = args;
            SetIntervalAndPadding();
            var KImg_Label = GetFromReference(UICommon_Label.KImg_Label);
            KImg_Label.SetActive(true);
            Init(args, isShowTextLabel);
            //0:道具 1:消耗品 2:图纸 3:通用合成材料 4~9:装备(武器~鞋子)
        }


        public void WearEquip(long uid)
        {
            foreach (var VARIABLE in equipItems)
            {
                if (VARIABLE.uid == uid)
                {
                    VARIABLE.SetActive(false);
                    return;
                }
            }
        }

        public void DestoryAllSelected()
        {
            foreach (var VARIABLE in equipItems)
            {
                var selected = VARIABLE.GetFromReference(UICommon_EquipItem.KImg_Selected);
                selected.SetActive(false);
            }
        }

        public void UnWearEquip(long uid)
        {
            foreach (var VARIABLE in equipItems)
            {
                if (VARIABLE.uid == uid)
                {
                    VARIABLE.SetActive(true);
                    return;
                }
            }
        }

        public void ChangeEquip(long lastUid, long uid)
        {
            UnWearEquip(lastUid);
            WearEquip(uid);
        }

        public void SortItems(bool isCompoundSort = false)
        {
            ResourcesSingleton.Instance.equipmentData.isCompoundSort = isCompoundSort;
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            var list = KContainer_Content.GetList();
            list.Sort(JiYuUIHelper.EquipUIComparer);
        }

        public void ForceRefreshLayout()
        {
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);

            JiYuUIHelper.ForceRefreshLayout(KContainer_Content);
        }

        public void RefreshRewardItems()
        {
            foreach (var item in rewardItems)
            {
                int rewardx = (int)item.trueReward.x;
                int rewardy = (int)item.trueReward.y;
                if (rewardx == 5)
                {
                    if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                    {
                        item.Initialize(new Vector3(rewardx, rewardy, ResourcesSingleton.Instance.items[rewardy]));
                    }
                    else
                    {
                        item.Dispose();
                    }
                }
            }
        }

        public void RefreshEquipItems()
        {
            foreach (var item in equipItems)
            {
                if (ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(item.uid, out var gameEquip))
                {
                    item.Initialize(gameEquip, UIPanel_Equipment.EquipPanelType.Main);
                }
            }
        }

        public void Init(CommonLabelType type, bool isShowTextLabel = true)
        {
            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;

            switch (type)
            {
                case CommonLabelType.Prop:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("item_page_1").current}");
                    break;
                case CommonLabelType.Consume:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("item_page_2").current}");
                    break;
                case CommonLabelType.Paper: //图纸
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_page_1").current}");
                    break;
                case CommonLabelType.GeneralMaterial: //通用合成材料
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_page_2").current}");
                    break;
                case CommonLabelType.Weapon:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_pos_1_name").current}");
                    break;
                case CommonLabelType.Clothing:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_pos_2_name").current}");
                    break;
                case CommonLabelType.Glove:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_pos_3_name").current}");
                    break;
                case CommonLabelType.Pants:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_pos_4_name").current}");
                    break;
                case CommonLabelType.Belt:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_pos_5_name").current}");
                    break;
                case CommonLabelType.Shoes:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_pos_6_name").current}");
                    break;
                case CommonLabelType.AllEquip:
                    KText_Label.GetTextMeshPro().SetTMPText("");
                    break;
                case CommonLabelType.SamePosIdCompound:
                    KText_Label.GetTextMeshPro().SetTMPText($"{language.Get("equip_merge_homie").current}");
                    break;
                case CommonLabelType.CurCanCompound:
                    KText_Label.GetTextMeshPro().SetTMPText("");
                    break;
            }

            if (!isShowTextLabel)
            {
                KText_Label.GetTextMeshPro().SetTMPText("");
            }


            var itemConfig = ConfigManager.Instance.Tables.Tbitem;

            if ((int)type >= 8 && (int)type <= 10)
            {
                foreach (var item in ResourcesSingleton.Instance.items)
                {
                    if (type == CommonLabelType.Prop && itemConfig.Get(item.Key).page == 1)
                    {
                        count++;
                    }
                    else if (type == CommonLabelType.Consume && itemConfig.Get(item.Key).page == 2)
                    {
                        count++;
                    }
                    else if (type == CommonLabelType.Paper && isPaper(item.Key))
                    {
                        count++;
                    }
                }
            }
            else
            {
                foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
                {
                    var equip = item.Value.equip;
                    int equipId = equip.EquipId * 100 + equip.Quality;
                    var equip_data = equip_dataConfig.Get(equipId);
                    if (type == CommonLabelType.AllEquip && equip.EquipId % 100 != 0)
                    {
                        count++;
                    }
                    else if (type == CommonLabelType.GeneralMaterial && equip.EquipId % 100 == 0)
                    {
                        count++;
                    }
                    else if ((int)type == equip_data.posId && equip.EquipId % 100 != 0)
                    {
                        count++;
                    }
                }
            }
        }

        public float GetHeight()
        {
            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var textHeight = KText_Label.GetRectTransform().Height();
            var verticalLayoutGroup = this.GetComponent<VerticalLayoutGroup>();
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            var gridLayoutGroup = KContainer_Content.GetComponent<GridLayoutGroup>();

            if (count <= 0)
            {
                return 0;
            }

            var row = count / cellPerRow;
            if (count % cellPerRow != 0)
            {
                row++;
            }

            float height = row * gridLayoutGroup.cellSize.y + (row - 1) * gridLayoutGroup.spacing.y + textHeight +
                           verticalLayoutGroup.spacing;


            return height;
        }

        void SetIntervalAndPadding()
        {
            //var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            var gridLayoutGroup = KContainer_Content.GetComponent<GridLayoutGroup>();
            var verticalLayoutGroup = this.GetComponent<VerticalLayoutGroup>();


            cellPerRow = (int)(Screen.width / cellSize);
            interval = (float)(Screen.width - cellSize * cellPerRow) / (float)(cellPerRow + 1);

            while (true)
            {
                if (interval < MinInterval)
                {
                    cellPerRow--;
                    interval = (float)(Screen.width - cellSize * cellPerRow) / (float)(cellPerRow + 1);
                    continue;
                }

                break;
            }

            //Log.Debug($"cellPerRow{cellPerRow}interval{interval}", Color.green);
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = cellPerRow;
            //var allCellPiexlPerRow = cellSize * cellPerRow;
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
            gridLayoutGroup.spacing = new Vector2(interval, interval);

            verticalLayoutGroup.padding.left = (int)interval;
        }

        bool isPaper(int id)
        {
            if (id >= 1020001 && id <= 1020006)
            {
                return true;
            }

            return false;
        }

        public async UniTask InitBag(CommonLabelType type, CancellationToken cct)
        {
            if (type == CommonLabelType.GeneralMaterial)
            {
                InitMaterial(type, cct);
                return;
            }

            var itemConfig = ConfigManager.Instance.Tables.Tbitem;
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            //var content = KContainer_Content?.GameObject?.transform;
            var list = KContainer_Content.GetList();
            list.Clear();
            foreach (var kv in ResourcesSingleton.Instance.items)
            {
                var item = itemConfig.Get(kv.Key);
                if (item.displayYn == 1)
                {
                    continue;
                }

                Vector3 reward = default;
                UICommon_RewardItem ui = null;

                if (type == CommonLabelType.Prop && item.page == 1 && !isPaper(kv.Key))
                {
                    reward = new Vector3(5, kv.Key, kv.Value);
                }
                else if (type == CommonLabelType.Consume && item.page == 2 && !isPaper(kv.Key))
                {
                    reward = new Vector3(5, kv.Key, kv.Value);
                }
                else if (type == CommonLabelType.Paper && isPaper(kv.Key))
                {
                    reward = new Vector3(5, kv.Key, kv.Value);
                }
                else continue;

                ui =
                    await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward, false) as UICommon_RewardItem;
                // ui = await UIHelper.CreateAsync(this, UIType.UICommon_RewardItem,
                //     reward, content, cct) as UICommon_RewardItem;
                ui.SetActive(true);
                rewardItems.Add(ui);
                JiYuUIHelper.SetRewardOnClick(reward, ui);
            }

            list.Sort(JiYuUIHelper.RewardUIComparer);
        }

        // bool IsWearThisEquip(long uid)
        // {
        //     bool isWear = false;
        //     foreach (var VARIABLE in ResourcesSingleton.Instance.equipmentData.isWearingEquipments)
        //     {
        //         if (VARIABLE.Value.PartId == uid)
        //         {
        //             isWear = true;
        //             break;
        //         }
        //     }
        //
        //     return isWear;
        // }

        public async UniTask InitEquip(CommonLabelType type, CancellationToken cct)
        {
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;

            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            var KImg_Label = GetFromReference(UICommon_Label.KImg_Label);
            KImg_Label.SetActive(type != CommonLabelType.AllEquip);
            //var content = KContainer_Content?.GameObject?.transform;
            var list = KContainer_Content.GetList();

            list.Clear();

            // for (int i = 0; i < ResourcesSingleton.Instance.equipmentData.equipments.Count; i++)
            // {
            //    
            // }

            foreach (var kv in ResourcesSingleton.Instance.equipmentData.equipments)
            {
                var item = kv.Value;
                var equipId = item.equip.EquipId * 100 + item.equip.Quality;
                var equip_data = equip_dataConfig.Get(equipId);
                if (item.equip.EquipId % 100 == 0)
                {
                    continue;
                }

                if (type == CommonLabelType.AllEquip) //按品质排序
                {
                    var ui =
                        await list.CreateWithUITypeAsync(UIType.UICommon_EquipItem, item,
                                UIPanel_Equipment.EquipPanelType.Main, false, cct) as
                            UICommon_EquipItem;

                    // var ui = await UIHelper.CreateAsync(this, UIType.UICommon_EquipItem,
                    //     item.Value, UIPanel_Equipment.EquipPanelType.Main, content, cct) as UICommon_EquipItem;
                    var KImg_Pos = ui.GetFromReference(UICommon_EquipItem.KImg_Pos);
                    KImg_Pos.SetActive(true);

                    JiYuUIHelper.SetEquipOnClick(item.equip.PartId, ui);
                    equipItems.Add(ui);
                    if (item.isWearing)
                    {
                        //Log.Debug($"ui.SetActive(false);{equip_data.posId}", Color.yellow);
                        ui.SetActive(false);
                    }
                    else
                    {
                        ui.SetActive(true);
                    }
                }
                else if ((int)type == equip_data.posId)
                {
                    var ui =
                        await list.CreateWithUITypeAsync(UIType.UICommon_EquipItem, item,
                                UIPanel_Equipment.EquipPanelType.Main, false, cct) as
                            UICommon_EquipItem;

                    var KImg_Pos = ui.GetFromReference(UICommon_EquipItem.KImg_Pos);
                    KImg_Pos.SetActive(true);

                    JiYuUIHelper.SetEquipOnClick(item.equip.PartId, ui);
                    equipItems.Add(ui);
                    if (item.isWearing)
                    {
                        //Log.Debug($"ui.SetActive(false);{equip_data.posId}", Color.yellow);
                        ui.SetActive(false);
                    }
                    else
                    {
                        ui.SetActive(true);
                    }
                }
            }

            list.Sort(JiYuUIHelper.EquipUIComparer);
        }

        public async UniTask InitEquipCompound(CommonLabelType type, CancellationToken cct)
        {
            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;

            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            //var content = KContainer_Content?.GameObject?.transform;
            var list = KContainer_Content.GetList();
            list.Clear();

            foreach (var kv in ResourcesSingleton.Instance.equipmentData.equipments)
            {
                var item = kv.Value;
                int equipId = item.equip.EquipId * 100 + item.equip.Quality;
                var equip_data = equip_dataConfig.Get(equipId);

                UICommon_EquipItem ui = null;
                bool isCreated = false;
                if (type == CommonLabelType.AllEquip && item.equip.EquipId % 100 != 0) //按品质排序
                {
                    isCreated = true;
                }
                else if ((int)type == equip_data.posId && item.equip.EquipId % 100 != 0)
                {
                    isCreated = true;
                }

                if (!isCreated)
                {
                    continue;
                }

                ui = await list.CreateWithUITypeAsync(UIType.UICommon_EquipItem, item,
                        UIPanel_Equipment.EquipPanelType.Compose, false, cct) as
                    UICommon_EquipItem;
                var KImg_Pos = ui.GetFromReference(UICommon_EquipItem.KImg_Pos);
                KImg_Pos.SetActive(true);
                var btn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);
                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                {
                    if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out var ui))
                    {
                        return;
                    }

                      JiYuUIHelper.DestoryAllTips();;
                  
                    var uiPanel = ui as UIPanel_Compound;
                    uiPanel.OnClickActionEvent(uiPanel.curSortBy, item.equip.PartId);
                    uiPanel.OnSelected(item.equip.PartId);
                });

                equipItems.Add(ui);
            }

            list.Sort(JiYuUIHelper.EquipUIComparer);
        }

        private bool CompareCanCompound(MyGameEquip myGameEquip1, MyGameEquip myGameEquip2)
        {
            //myGameEquip1 选择的equip
            if (myGameEquip1.equip.PartId == myGameEquip2.equip.PartId)
            {
                return false;
            }

            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;


            var equip_data1 = equip_dataConfig.Get(myGameEquip1.equip.EquipId * 100 + myGameEquip1.equip.Quality);
            var equip_data2 = equip_dataConfig.Get(myGameEquip2.equip.EquipId * 100 + myGameEquip2.equip.Quality);
            var equip_quality1 = equip_qualityConfig.Get(equip_data1.quality);
            var equip_quality2 = equip_qualityConfig.Get(equip_data2.quality);

            bool canCompound = false;


            var needList = equip_quality1.mergeRule;
            if (needList.Count <= 0)
            {
                return false;
            }

            var needQua = needList[0];
            //var needCount = needList[1];

            if (equip_quality1.mergeSelf == 1)
            {
                // if (equip_data1.sYn == 1)
                // {
                //     if (myGameEquip1.equip.EquipId == myGameEquip2.equip.EquipId &&
                //         needQua == myGameEquip2.equip.Quality &&
                //         equip_data1.posId == equip_data2.posId)
                //     {
                //         canCompound = true;
                //     }
                // }
                // else
                // {
                if (myGameEquip1.equip.EquipId == myGameEquip2.equip.EquipId &&
                    needQua == myGameEquip2.equip.Quality &&
                    equip_data1.posId == equip_data2.posId)
                {
                    canCompound = true;
                }
                //}
            }
            else
            {
                // if (equip_data1.sYn == 1)
                // {
                //     if (needQua == myGameEquip2.equip.Quality &&
                //         equip_data1.posId == equip_data2.posId)
                //     {
                //         canCompound = true;
                //     }
                // }
                // else
                // {
                if (needQua == myGameEquip2.equip.Quality &&
                    equip_data1.posId == equip_data2.posId && equip_data2.sYn != 1)
                {
                    canCompound = true;
                }
                //}
            }

            return canCompound;
        }

        public void LockOrUnlockUnSelectedEquip(bool isLock)
        {
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            var list = KContainer_Content.GetList();
            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out var uiCompound))
            {
                return;
            }

            var uiPanelCompound = uiCompound as UIPanel_Compound;
            foreach (var ui in list.Children)
            {
                var uiitem = ui as UICommon_EquipItem;

                if (uiPanelCompound.selectedEquips.ContainsKey(uiitem.uid))
                {
                    continue;
                }

                var btn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);
                var KImg_Mask = ui.GetFromReference(UICommon_EquipItem.KImg_Mask);
                var KImg_MaskIcon = ui.GetFromReference(UICommon_EquipItem.KImg_MaskIcon);

                if (isLock)
                {
                    KImg_MaskIcon.GetImage().SetSpriteAsync("pic_equip_locked", false).Forget();
                    KImg_Mask.SetActive(true);
                    btn.GetXButton().SetEnabled(false);
                }
                else
                {
                    KImg_Mask.SetActive(false);
                    btn.GetXButton().SetEnabled(true);
                }
            }
        }

        public async UniTask InitEquipCompoundSelected(CommonLabelType type, long uid, CancellationToken cct)
        {
            if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid, out var myGameEquip))
            {
                Log.Error($"uid:{uid} is not exist");
                return;
            }

            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equipId = myGameEquip.equip.EquipId * 100 + myGameEquip.equip.Quality;
            var equip_data = equip_dataConfig.Get(equipId);
            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            //var content = KContainer_Content?.GameObject?.transform;
            var list = KContainer_Content.GetList();
            list.Clear();

            foreach (var kv in ResourcesSingleton.Instance.equipmentData.equipments)
            {
                var item = kv.Value;
                var equipId0 = item.equip.EquipId * 100 + item.equip.Quality;
                var kv_equip_data = equip_dataConfig.Get(equipId0);
                if (kv_equip_data.posId == equip_data.posId)
                {
                    if (type == CommonLabelType.CurCanCompound && CompareCanCompound(myGameEquip, item))
                    {
                        var ui =
                            await list.CreateWithUITypeAsync(UIType.UICommon_EquipItem, item,
                                    UIPanel_Equipment.EquipPanelType.ComposeSelected, false, cct) as
                                UICommon_EquipItem;
                        var btn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);
                        btn.GetXButton().SetEnabled(true);
                        var KImg_Mask = ui.GetFromReference(UICommon_EquipItem.KImg_Mask);
                        var KImg_MaskIcon = ui.GetFromReference(UICommon_EquipItem.KImg_MaskIcon);
                        KImg_MaskIcon.GetImage().SetSpriteAsync("pic_equip_greencheckmark", false).Forget();
                        KImg_Mask.SetActive(false);
                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn,
                            () =>
                            {
                                KImg_MaskIcon.GetImage().SetSpriteAsync("pic_equip_greencheckmark", false).Forget();
                                KImg_Mask.SetActive(!KImg_Mask.GameObject.activeSelf);
                                if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out var ui))
                                {
                                    return;
                                }

                                var uiPanel = ui as UIPanel_Compound;
                                uiPanel.OnSelected(item.equip.PartId);
                            });

                        equipItems.Add(ui);
                    }
                    else if (type == CommonLabelType.SamePosIdCompound && !CompareCanCompound(myGameEquip, item) &&
                             item.equip.EquipId % 100 != 0 &&
                             item.equip.PartId != uid)
                    {
                        var ui =
                            await list.CreateWithUITypeAsync(UIType.UICommon_EquipItem, item,
                                    UIPanel_Equipment.EquipPanelType.ComposeSelected, false, cct) as
                                UICommon_EquipItem;
                        var btn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);
                        btn.GetXButton().SetEnabled(true);
                        var KImg_Mask = ui.GetFromReference(UICommon_EquipItem.KImg_Mask);
                        var KImg_MaskIcon = ui.GetFromReference(UICommon_EquipItem.KImg_MaskIcon);
                        KImg_MaskIcon.GetImage().SetSpriteAsync("pic_equip_locked", false).Forget();
                        KImg_Mask.SetActive(true);

                        equipItems.Add(ui);
                    }
                    else if (type == CommonLabelType.CurCanCompound && !CompareCanCompound(myGameEquip, item) &&
                             item.equip.PartId == uid)
                    {
                        var ui =
                            await list.CreateWithUITypeAsync(UIType.UICommon_EquipItem, item,
                                    UIPanel_Equipment.EquipPanelType.ComposeSelected, false, cct) as
                                UICommon_EquipItem;
                        var btn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);
                        btn.GetXButton().SetEnabled(true);
                        var KImg_Mask = ui.GetFromReference(UICommon_EquipItem.KImg_Mask);
                        var KImg_MaskIcon = ui.GetFromReference(UICommon_EquipItem.KImg_MaskIcon);
                        KImg_MaskIcon.GetImage().SetSpriteAsync("pic_equip_greencheckmark", false).Forget();
                        KImg_Mask.SetActive(true);

                        JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, () =>
                        {
                            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Compound, out var ui))
                            {
                                return;
                            }

                            var uiPanel = ui as UIPanel_Compound;

                            var lastSortBy = uiPanel.curSortBy;
                            uiPanel.curSortBy = CommonLabelType.Paper;
                            uiPanel.OnClickActionEvent(lastSortBy);
                        });
                    }
                }
            }

            list.Sort((obj11, obj21) =>
            {
                var obj111 = obj11 as UICommon_EquipItem;
                var obj211 = obj21 as UICommon_EquipItem;
                ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(obj111.uid, out var obj100);
                ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(obj211.uid, out var obj200);
                var obj1 = obj100.equip;
                var obj2 = obj200.equip;
                var equipData = ConfigManager.Instance.Tables.Tbequip_data;
                var objId1 = obj1.EquipId * 100 + obj1.Quality;
                var objId2 = obj2.EquipId * 100 + obj2.Quality;

                var equipobj1 = equipData.Get(objId1);
                var equipobj2 = equipData.Get(objId2);
                // uid
                if (obj1.PartId == uid && obj2.PartId != uid)
                    return -1;
                else if (obj1.PartId != uid && obj2.PartId == uid)
                    return 1;

                if (obj1.EquipId % 100 == 0 && obj2.EquipId % 100 != 0)
                    return -1;
                else if (obj1.EquipId % 100 != 0 && obj2.EquipId % 100 == 0)
                    return 1;

                // 品质由高到低
                if (obj1.Quality > obj2.Quality)
                    return -1;
                else if (obj1.Quality < obj2.Quality)
                    return 1;

                // S在前，普通在后
                if (equipobj1.sYn == 1 && equipobj2.sYn == 0)
                    return -1;
                else if (equipobj1.sYn == 0 && equipobj2.sYn == 1)
                    return 1;

                // 部位id由小到大
                if (equipobj1.posId < equipobj2.posId)
                    return -1;
                else if (equipobj1.posId > equipobj2.posId)
                    return 1;

                // 等级由高到低
                if (obj1.EquipLevel > obj2.EquipLevel)
                    return -1;
                else if (obj1.EquipLevel < obj2.EquipLevel)
                    return 1;

                // 装备id由大到小
                if (obj1.EquipId > obj2.EquipId)
                    return -1;
                else if (obj1.EquipId < obj2.EquipId)
                    return 1;

                // uid从小到大
                if (obj1.PartId < obj2.PartId)
                    return -1;
                else if (obj1.PartId > obj2.PartId)
                    return 1;

                return 0;
            });
        }


        protected override void OnClose()
        {
            base.OnClose();
        }


        private async UniTask InitMaterial(CommonLabelType type, CancellationToken cct)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var KText_Label = GetFromReference(UICommon_Label.KText_Label);
            var KContainer_Content = GetFromReference(UICommon_Label.KContainer_Content);
            //var content = KContainer_Content?.GameObject?.transform;
            var list = KContainer_Content.GetList();


            foreach (var item in ResourcesSingleton.Instance.equipmentData.isMaterials)
            {
                // var rewardz = CmdHelper.GetMergeCmd((int)item.Key.z, item.Value);
                // var rewardMerged = new Vector3(item.Key.x, item.Key.y, rewardz);
                var reward = new Vector3(item.Key.x, item.Key.y, item.Value);

                var ui =
                    await list.CreateWithUITypeAsync(UIType.UICommon_RewardItem, reward, false, cct) as
                        UICommon_RewardItem;
                // var ui =
                //     await UIHelper.CreateAsync(this, UIType.UICommon_RewardItem, rewardMerged, content, cct) as
                //         UICommon_RewardItem;
                ui.SetActive(true);
                JiYuUIHelper.SetRewardOnClick(reward, ui);
                rewardItems.Add(ui);
            }

            list.Sort(JiYuUIHelper.RewardUIComparer);
            //equipItems.Add(ui);
        }
    }
}