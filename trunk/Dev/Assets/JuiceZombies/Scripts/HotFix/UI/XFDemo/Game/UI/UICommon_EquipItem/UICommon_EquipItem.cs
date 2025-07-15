//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using Common;
using HotFix_UI;
using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UICommon_EquipItem)]
    internal sealed class UICommon_EquipItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_RewardItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_EquipItem>();
        }
    }


    public struct GameEquipStruct
    {
        public GameEquip equip;
        public long num;
        public int Type;
    }

    public partial class UICommon_EquipItem : UI, IAwake<MyGameEquip, UIPanel_Equipment.EquipPanelType>
    {
        public long uid;

        public MyGameEquip equip;
        //Type表示装备在装备界面还是合成界面 1装备界面 2合成界面 3合成界面点击一次 4在装备界面的通用合成材料 5已装备的装备

        public void Initialize(MyGameEquip myGameEquip, UIPanel_Equipment.EquipPanelType panelType)
        {
            uid = myGameEquip.equip.PartId;
            equip = myGameEquip;
            JiYuUIHelper.SetEquipIcon(myGameEquip, this, panelType);
            this.GetFromReference(KBtn_Item).GetRectTransform().SetScale3(1);
            var group= this.GetFromReference(KBtn_Item).GetComponent<CanvasGroup>();
            if (group!=null)
            {
                this.GetFromReference(KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
            }
            GetFromReference(KEffect).SetActive(false);
            //this.GetFromReference(KBtn_Item).GetComponent<CanvasGroup>().alpha = 1;
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}