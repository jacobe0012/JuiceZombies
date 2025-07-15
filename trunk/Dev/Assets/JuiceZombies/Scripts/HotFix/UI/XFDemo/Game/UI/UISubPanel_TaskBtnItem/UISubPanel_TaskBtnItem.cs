//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_TaskBtnItem)]
    internal sealed class UISubPanel_TaskBtnItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_TaskBtnItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_TaskBtnItem>();
        }
    }

    public partial class UISubPanel_TaskBtnItem : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            id = args;
            InitNode();
        }

        void InitNode()
        {
            var KImg_IconBtn = GetFromReference(UISubPanel_TaskBtnItem.KImg_IconBtn);
            var KText_IconBtn = GetFromReference(UISubPanel_TaskBtnItem.KText_IconBtn);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}