//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Common_Events)]
    internal sealed class UIPanel_Common_EventsEvent : AUIEvent
    {
        public override string Key => UIPathSet.UIPanel_Common_Events;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Common_Events>();
        }
    }

    public partial class UIPanel_Common_Events : UI, IAwake<int>
    {
        public int sortIndex;

        public void Initialize(int id)
        {
        }


        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}