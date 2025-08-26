//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Task_Score_Box)]
    internal sealed class UISubPanel_Task_Score_BoxEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Task_Score_Box;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Task_Score_Box>();
        }
    }

    public partial class UISubPanel_Task_Score_Box : UI, IAwake<int>
    {
        public int id;

        public void Initialize(int args)
        {
            id = args;
            //this.GetRectTransform().SetScale2(1.0f);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}