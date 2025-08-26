//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Fund_Item)]
    internal sealed class UISubPanel_Shop_Fund_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Fund_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Fund_Item>();
        }
    }

    public partial class UISubPanel_Shop_Fund_Item : UI, IAwake
    {
        public void Initialize()
        {
            this.GetFromReference(KImg_Received_Left).SetActive(false);
            this.GetFromReference(KImg_Received_Mid).SetActive(false);
            this.GetFromReference(KImg_Received_Right).SetActive(false);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}