//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Select_Tab_Btn)]
    internal sealed class UISubPanel_Select_Tab_BtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Select_Tab_Btn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Select_Tab_Btn>();
        }
    }

    public partial class UISubPanel_Select_Tab_Btn : UI, IAwake
    {
        public void Initialize()
        {
            this.GetFromReference(KImg_RedPoint).SetActive(false);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}