//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

using UnityEngine;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Pre_Row)]
    internal sealed class UISubPanel_Shop_Pre_RowEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Pre_Row;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Pre_Row>();
        }
    }

    public partial class UISubPanel_Shop_Pre_Row : UI, IAwake
    {
        public void Initialize()
        {
        }

        public void DebugTest()
        {
            Debug.Log("DebugTest");
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}