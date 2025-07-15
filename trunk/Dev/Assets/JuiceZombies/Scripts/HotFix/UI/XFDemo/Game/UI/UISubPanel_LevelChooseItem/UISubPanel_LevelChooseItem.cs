//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_LevelChooseItem)]
    internal sealed class UISubPanel_LevelChooseItemEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_LevelChooseItem;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_LevelChooseItem>();
        }
    }

    public partial class UISubPanel_LevelChooseItem : UI, IAwake<int>
    {
        public int chapterId;

        public void Initialize(int args)
        {
            chapterId = args;
            InitNode();
        }

        void InitNode()
        {
            
            var KBg_LevelItemDown = GetFromReference(UISubPanel_LevelChooseItem.KBg_LevelItemDown);
            var KText_LevelItemTitle = GetFromReference(UISubPanel_LevelChooseItem.KText_LevelItemTitle);
            var KText_LevelItemContent = GetFromReference(UISubPanel_LevelChooseItem.KText_LevelItemContent);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}