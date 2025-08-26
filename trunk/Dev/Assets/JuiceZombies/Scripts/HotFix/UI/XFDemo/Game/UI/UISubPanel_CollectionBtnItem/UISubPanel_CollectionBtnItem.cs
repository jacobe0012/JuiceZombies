//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_CollectionBtnItem)]
    internal sealed class UISubPanel_CollectionBtnItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_CollectionBtnItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_CollectionBtnItem>();
        }
    }

    public partial class UISubPanel_CollectionBtnItem : UI, IAwake<int>
    {
        public int powerId;

        public void Initialize(int args)
        {
            powerId = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}