//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Pass_Item)]
    internal sealed class UISubPanel_Pass_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Pass_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Pass_Item>();
        }
    }

    public partial class UISubPanel_Pass_Item : UI, IAwake<int>
    {
        public int level;

        public void Initialize(int args)
        {
            level = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}