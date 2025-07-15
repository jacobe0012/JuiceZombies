//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_MonopolyShopItem)]
    internal sealed class UISubPanel_MonopolyShopItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_MonopolyShopItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_MonopolyShopItem>();
        }
    }

    public partial class UISubPanel_MonopolyShopItem : UI, IAwake<int>
    {
        public int sort;

        public void Initialize(int args)
        {
            sort = args;
            InitNode();
        }

        void InitNode()
        {
            var KRewardPos = GetFromReference(UISubPanel_MonopolyShopItem.KRewardPos);
            var KText_Desc = GetFromReference(UISubPanel_MonopolyShopItem.KText_Desc);
            var KText_Name = GetFromReference(UISubPanel_MonopolyShopItem.KText_Name);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}