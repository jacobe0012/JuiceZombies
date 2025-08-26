//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Task_DAndW)]
    internal sealed class UISubPanel_Task_DAndWEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Task_DAndW;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Task_DAndW>();
        }
    }

    public partial class UISubPanel_Task_DAndW : UI, IAwake<int, int>
    {
        public int taskId;
        public int state;

        public void Initialize(int args, int state)
        {
            taskId = args;
            this.state = state;
            GetFromReference(KPos).SetActive(true);
            GetFromReference(KPos2).SetActive(false);

            float alpha = 1.0f;
            GetFromReference(UISubPanel_Task_DAndW.KImg_Task).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KProgressBar).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KProgressBar).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KProgressBar2).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KProgressBar_Bg2).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar).GetTextMeshPro().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KText_ProgressBar2).GetTextMeshPro().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name).GetTextMeshPro().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KText_Task_Name2).GetTextMeshPro().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KImg_Dui).GetXImage().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KText_ScoreNum2).GetTextMeshPro().SetAlpha(alpha);
            GetFromReference(UISubPanel_Task_DAndW.KImg_Score2).GetXImage().SetAlpha(alpha);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}