//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Achieve_Text)]
    internal sealed class UISubPanel_Achieve_TextEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Achieve_Text;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Achieve_Text>();
        }
    }

    public partial class UISubPanel_Achieve_Text : UI, IAwake
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