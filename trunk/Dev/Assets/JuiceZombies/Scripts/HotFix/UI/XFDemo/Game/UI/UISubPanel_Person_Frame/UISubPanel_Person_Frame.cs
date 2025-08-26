//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Person_Frame)]
    internal sealed class UISubPanel_Person_FrameEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Person_Frame;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_Frame>();
        }
    }

    public partial class UISubPanel_Person_Frame : UI, IAwake
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