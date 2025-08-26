//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Person_Btn51)]
    internal sealed class UISubPanel_Person_Btn51Event : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Person_Btn51;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Person_Btn51>();
        }
    }

    public partial class UISubPanel_Person_Btn51 : UI, IAwake<int>
    {
        public int tag_fun_id;

        public void Initialize(int args)
        {
            tag_fun_id = args;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}