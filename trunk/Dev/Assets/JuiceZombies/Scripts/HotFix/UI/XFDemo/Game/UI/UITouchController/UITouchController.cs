//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UITouchController)]
    internal sealed class UITouchControllerEvent : AUIEvent
    {
        public override string Key => UIPathSet.UITouchController;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UITouchController>();
        }
    }

    public partial class UITouchController : UI, IAwake
    {
        public void Initialize()
        {
            var KBase = this.GetFromReference(UITouchController.KBase);
            var KStick = this.GetFromReference(UITouchController.KStick);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}