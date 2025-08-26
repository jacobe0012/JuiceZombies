//---------------------------------------------------------------------
// UnicornStudio
// Author: huangjinguo
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_ChapterGift)]
    internal sealed class UISubPanel_Shop_ChapterGiftEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Shop_ChapterGift;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_ChapterGift>();
        }
    }

    public partial class UISubPanel_Shop_ChapterGift : UI, IAwake
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