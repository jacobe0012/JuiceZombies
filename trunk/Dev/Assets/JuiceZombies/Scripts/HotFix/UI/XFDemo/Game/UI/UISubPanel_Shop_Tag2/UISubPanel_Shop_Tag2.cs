//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Tag2)]
    internal sealed class UISubPanel_Shop_Tag2Event : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Tag2;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Tag2>();
        }
    }

    public partial class UISubPanel_Shop_Tag2 : UI, IAwake
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