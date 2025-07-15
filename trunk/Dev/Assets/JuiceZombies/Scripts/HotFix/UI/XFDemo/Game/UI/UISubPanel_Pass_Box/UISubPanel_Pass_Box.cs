//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Pass_Box)]
    internal sealed class UISubPanel_Pass_BoxEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Pass_Box;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Pass_Box>();
        }
    }

    public partial class UISubPanel_Pass_Box : UI, IAwake
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