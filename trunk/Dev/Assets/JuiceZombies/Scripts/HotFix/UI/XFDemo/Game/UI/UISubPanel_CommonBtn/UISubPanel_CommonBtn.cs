//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using HotFix_UI;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_CommonBtn)]
    internal sealed class UISubPanel_CommonBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_CommonBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_CommonBtn>();
        }
    }

    public partial class UISubPanel_CommonBtn : UI, IAwake<int>
    {
        public int index;
        public string icon;
        public string text;
        public void Initialize(int args)
        {
            index = args;
           
        }

        

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}