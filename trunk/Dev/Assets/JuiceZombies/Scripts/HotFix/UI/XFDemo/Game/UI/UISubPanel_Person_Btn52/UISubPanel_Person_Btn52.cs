//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Person_Btn52)]
    internal sealed class UISubPanel_Person_Btn52Event : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Person_Btn52;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_Btn52>();
        }
    }

    public partial class UISubPanel_Person_Btn52 : UI, IAwake
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