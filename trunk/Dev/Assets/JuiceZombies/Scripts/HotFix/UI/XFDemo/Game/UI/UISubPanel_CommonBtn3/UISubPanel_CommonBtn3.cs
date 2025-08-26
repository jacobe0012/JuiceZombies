//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_CommonBtn3)]
    internal sealed class UISubPanel_CommonBtn3Event : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_CommonBtn3;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_CommonBtn3>();
        }
    }

    public partial class UISubPanel_CommonBtn3 : UI, IAwake
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