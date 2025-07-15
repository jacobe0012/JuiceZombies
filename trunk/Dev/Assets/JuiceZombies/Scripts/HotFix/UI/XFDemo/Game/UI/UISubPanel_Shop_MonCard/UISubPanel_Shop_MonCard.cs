//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_MonCard)]
    internal sealed class UISubPanel_Shop_MonCardEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_MonCard;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_MonCard>();
        }
    }

    public partial class UISubPanel_Shop_MonCard : UI, IAwake<int>
    {
        public int id;
        public void Initialize(int args)
        {
            id = args;
            this.GetFromReference(KImg_RedPoint).SetActive(false);

        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}