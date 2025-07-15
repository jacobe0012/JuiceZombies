//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Activity_Equip_Btn)]
    internal sealed class UISubPanel_Activity_Equip_BtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Activity_Equip_Btn;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Activity_Equip_Btn>();
        }
    }

    public partial class UISubPanel_Activity_Equip_Btn : UI, IAwake
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