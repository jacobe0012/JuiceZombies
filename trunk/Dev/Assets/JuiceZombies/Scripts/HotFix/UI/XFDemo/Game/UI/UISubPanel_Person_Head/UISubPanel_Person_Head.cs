//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Person_Head)]
    internal sealed class UISubPanel_Person_HeadEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Person_Head;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_Head>();
        }
    }

    public partial class UISubPanel_Person_Head : UI, IAwake
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