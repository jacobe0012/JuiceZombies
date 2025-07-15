//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UICommon_PlainTextTip)]
    internal sealed class UICommon_PlainTextTipEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UICommon_PlainTextTip;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Mid;

        public override UI OnCreate()
        {
            return UI.Create<UICommon_PlainTextTip>();
        }
    }

    public partial class UICommon_PlainTextTip : UI, IAwake
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