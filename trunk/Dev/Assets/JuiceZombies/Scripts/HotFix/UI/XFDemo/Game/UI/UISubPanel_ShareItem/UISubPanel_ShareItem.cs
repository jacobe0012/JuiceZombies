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
    [UIEvent(UIType.UISubPanel_ShareItem)]
    internal sealed class UISubPanel_ShareItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_ShareItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_ShareItem>();
        }
    }

    public partial class UISubPanel_ShareItem : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            id = args;
            InitNode();
        }

        void InitNode()
        {
            var KImg_IconBtn = GetFromReference(UISubPanel_ShareItem.KImg_IconBtn);
            var KText_IconBtn = GetFromReference(UISubPanel_ShareItem.KText_IconBtn);
            var KImg_RedDot = GetFromReference(UISubPanel_ShareItem.KImg_RedDot);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}