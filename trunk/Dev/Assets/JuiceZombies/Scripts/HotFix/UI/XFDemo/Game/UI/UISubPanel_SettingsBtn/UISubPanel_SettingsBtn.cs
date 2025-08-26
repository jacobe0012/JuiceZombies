//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_SettingsBtn)]
    internal sealed class UISubPanel_SettingsBtnEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_SettingsBtn;

        public override bool IsFromPool => false;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_SettingsBtn>();
        }
    }

    public partial class UISubPanel_SettingsBtn : UI, IAwake
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