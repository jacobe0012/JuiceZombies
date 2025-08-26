//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_1404_MonthCard)]
    internal sealed class UISubPanel_Shop_1404_MonthCardEvent : AUIEvent,IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Shop_1404_MonthCard;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

     

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_1404_MonthCard>();
        }
    }

    public partial class UISubPanel_Shop_1404_MonthCard : UI, IAwake
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