//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Select_Tab_Btn2)]
    internal sealed class UISubPanel_Select_Tab_Btn2Event : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Select_Tab_Btn2;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Select_Tab_Btn2>();
        }
    }

    public partial class UISubPanel_Select_Tab_Btn2 : UI, IAwake<int>
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