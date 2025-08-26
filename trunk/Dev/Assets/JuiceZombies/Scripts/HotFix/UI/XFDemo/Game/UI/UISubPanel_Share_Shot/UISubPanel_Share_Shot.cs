//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Share_Shot)]
    internal sealed class UISubPanel_Share_ShotEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Share_Shot;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Share_Shot>();
        }
    }

    public partial class UISubPanel_Share_Shot : UI, IAwake
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