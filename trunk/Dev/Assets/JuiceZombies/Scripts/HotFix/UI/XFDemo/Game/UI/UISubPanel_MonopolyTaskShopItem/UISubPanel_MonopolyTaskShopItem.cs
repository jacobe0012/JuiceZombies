//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_MonopolyTaskShopItem)]
    internal sealed class UISubPanel_MonopolyTaskShopItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_MonopolyTaskShopItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_MonopolyTaskShopItem>();
        }
    }

    public partial class UISubPanel_MonopolyTaskShopItem : UI, IAwake<int>
    {
        public int taskId;

        public void Initialize(int args)
        {
            taskId = args;
            InitNode();

        }

        void InitNode()
        {
            var KRewardPos = GetFromReference(UISubPanel_MonopolyTaskShopItem.KRewardPos);
            var KText_Name = GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Name);
            var KText_BuyFinish = GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_BuyFinish);
            var KImg_Fill = GetFromReference(UISubPanel_MonopolyTaskShopItem.KImg_Fill);
            var KText_Fill = GetFromReference(UISubPanel_MonopolyTaskShopItem.KText_Fill);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}