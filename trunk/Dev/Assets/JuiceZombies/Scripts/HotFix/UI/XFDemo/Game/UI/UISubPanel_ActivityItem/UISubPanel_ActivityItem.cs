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
    [UIEvent(UIType.UISubPanel_ActivityItem)]
    internal sealed class UISubPanel_ActivityItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_ActivityItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_ActivityItem>();
        }
    }

    public partial class UISubPanel_ActivityItem : UI, IAwake<int>
    {
        public int tagFuncId;

        public void Initialize(int id)
        {
            tagFuncId = id;
            InitNode();
        }

        void InitNode()
        {
            
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}