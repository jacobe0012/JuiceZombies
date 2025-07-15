//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Shop_Tag1)]
    internal sealed class UISubPanel_Shop_Tag1Event : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UISubPanel_Shop_Tag1;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Shop_Tag1>();
        }
    }

    public partial class UISubPanel_Shop_Tag1 : UI, IAwake
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