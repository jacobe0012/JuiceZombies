//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Fund_Point)]
    internal sealed class UISubPanel_Shop_Fund_PointEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Fund_Point;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Fund_Point>();
        }
    }

    public partial class UISubPanel_Shop_Fund_Point : UI, IAwake
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