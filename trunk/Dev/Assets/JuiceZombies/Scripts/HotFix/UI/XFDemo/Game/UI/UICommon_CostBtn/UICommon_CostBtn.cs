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
    [UIEvent(UIType.UICommon_CostBtn)]
    internal sealed class UICommon_CostBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_CostBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_CostBtn>();
        }
    }

    public partial class UICommon_CostBtn : UI, IAwake
    {
        public void Initialize()
        {
            InitNode();
        }

        void InitNode()
        {
            var KText_Cost = GetFromReference(UICommon_CostBtn.KText_Cost);
            var KCommon_Btn = GetFromReference(UICommon_CostBtn.KCommon_Btn);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}