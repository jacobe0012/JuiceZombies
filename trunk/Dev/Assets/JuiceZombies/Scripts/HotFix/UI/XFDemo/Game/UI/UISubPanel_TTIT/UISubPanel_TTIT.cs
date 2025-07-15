//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_TTIT)]
    internal sealed class UISubPanel_TTITEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_TTIT;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_TTIT>();
        }
    }

    public partial class UISubPanel_TTIT : UI, IAwake<int>
    {
        public int index;
        public void Initialize(int args)
        {
            index = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}