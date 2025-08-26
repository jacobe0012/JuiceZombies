//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Special_Bg)]
    internal sealed class UISubPanel_Shop_Special_BgEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Special_Bg;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Special_Bg>();
        }
    }

    public partial class UISubPanel_Shop_Special_Bg : UI, IAwake
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