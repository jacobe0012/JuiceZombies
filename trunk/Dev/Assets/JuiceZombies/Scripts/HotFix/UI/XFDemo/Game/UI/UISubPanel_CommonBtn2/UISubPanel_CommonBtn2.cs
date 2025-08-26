//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_CommonBtn2)]
    internal sealed class UISubPanel_CommonBtn2Event : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_CommonBtn2;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_CommonBtn2>();
        }
    }

    public partial class UISubPanel_CommonBtn2 : UI, IAwake
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