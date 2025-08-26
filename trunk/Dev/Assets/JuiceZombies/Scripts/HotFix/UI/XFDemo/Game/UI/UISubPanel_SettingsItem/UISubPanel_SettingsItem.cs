//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_SettingsItem)]
    internal sealed class UISubPanel_SettingsItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_SettingsItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_SettingsItem>();
        }
    }

    public partial class UISubPanel_SettingsItem : UI, IAwake
    {
        public bool isEnable = false;

        public void Initialize()
        {
        }

        public void SetEnable(bool enable)
        {
            var KText = GetFromReference(UISubPanel_SettingsItem.KText);
            var KIcon = GetFromReference(UISubPanel_SettingsItem.KIcon);

            isEnable = enable;
            if (enable)
            {
                KIcon.GetImage().SetAlpha(1);
                KText.GetTextMeshPro().SetAlpha(1);
            }
            else
            {
                KIcon.GetImage().SetAlpha(0.5f);
                KText.GetTextMeshPro().SetAlpha(0.5f);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}