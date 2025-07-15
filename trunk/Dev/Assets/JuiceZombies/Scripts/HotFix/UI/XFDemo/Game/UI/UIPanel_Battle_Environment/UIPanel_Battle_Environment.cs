//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UIPanel_Battle_Environment)]
    internal sealed class UIPanel_Battle_EnvironmentEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_Battle_Environment;

        public override bool IsFromPool => true;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_Battle_Environment>();
        }
    }

    public partial class UIPanel_Battle_Environment : UI, IAwake
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