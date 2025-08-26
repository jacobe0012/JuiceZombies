//---------------------------------------------------------------------
// UnicornStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_Challenge_Container_AreaDetails)]
    internal sealed class UISubPanel_Challenge_Container_AreaDetailsEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_Challenge_Container_AreaDetails;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_Challenge_Container_AreaDetails>();
        }
    }

    public partial class UISubPanel_Challenge_Container_AreaDetails : UI, IAwake<int>
    {
        public int index;

        protected override void OnClose()
        {
            base.OnClose();
        }


        public void Initialize(int args)
        {
        }
    }
}