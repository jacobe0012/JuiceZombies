//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_DamageInfoItem)]
    internal sealed class UISubPanel_DamageInfoItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_DamageInfoItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_DamageInfoItem>();
        }
    }

    public partial class UISubPanel_DamageInfoItem : UI, IAwake<int>
    {
        public int index;

        public void Initialize(int args)
        {
            index = args;
            InitNode();
        }

        void InitNode()
        {
            var KImg_WeaponIcon = GetFromReference(UISubPanel_DamageInfoItem.KImg_WeaponIcon);
            var KText_Name = GetFromReference(UISubPanel_DamageInfoItem.KText_Name);
            var KText_Num = GetFromReference(UISubPanel_DamageInfoItem.KText_Num);
            var KImg_Filled = GetFromReference(UISubPanel_DamageInfoItem.KImg_Filled);
            var KText_Ratios = GetFromReference(UISubPanel_DamageInfoItem.KText_Ratios);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}