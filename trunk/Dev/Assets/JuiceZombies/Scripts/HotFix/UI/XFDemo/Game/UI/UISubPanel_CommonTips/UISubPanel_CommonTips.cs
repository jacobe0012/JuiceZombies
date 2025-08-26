//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_CommonTips)]
    internal sealed class UISubPanel_CommonTipsEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_CommonTips;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_CommonTips>();
        }
    }

    public partial class UISubPanel_CommonTips : UI, IAwake
    {
        public void Initialize()
        {
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}