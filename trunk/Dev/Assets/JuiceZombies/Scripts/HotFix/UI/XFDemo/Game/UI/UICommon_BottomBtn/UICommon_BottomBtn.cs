//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UICommon_BottomBtn)]
    internal sealed class UICommon_BottomBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_BottomBtn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_BottomBtn>();
        }
    }

    public partial class UICommon_BottomBtn : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            id = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}