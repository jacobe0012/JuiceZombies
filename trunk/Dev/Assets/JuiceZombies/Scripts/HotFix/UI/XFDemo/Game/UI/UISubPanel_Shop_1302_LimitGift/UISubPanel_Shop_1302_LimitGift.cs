//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_1302_LimitGift)]
    internal sealed class UISubPanel_Shop_1302_LimitGiftEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_1302_LimitGift;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_1302_LimitGift>();
        }
    }

    public partial class UISubPanel_Shop_1302_LimitGift : UI, IAwake
    {
        public void Initialize()
        {
        }

        protected override void OnClose()
        {
            //ClearList();

            base.OnClose();
        }

        private void ClearList()
        {
            //var list = this.GetFromReference(UISubPanel_Shop_1302_LimitGift.KPos_Item)?.GetList();
            
        }
    }
}