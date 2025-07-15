//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UICommon_Select_Tab)]
    internal sealed class UICommon_Select_TabEvent : AUIEvent
    {
        public override string Key => UIPathSet.UICommon_Select_Tab;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UICommon_Select_Tab>();
        }
    }

    public partial class UICommon_Select_Tab : UI, IAwake
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