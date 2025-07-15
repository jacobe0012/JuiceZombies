//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Notice_Item)]
    internal sealed class UISubPanel_Notice_ItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Notice_Item;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Notice_Item>();
        }
    }

    public partial class UISubPanel_Notice_Item : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            id = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}