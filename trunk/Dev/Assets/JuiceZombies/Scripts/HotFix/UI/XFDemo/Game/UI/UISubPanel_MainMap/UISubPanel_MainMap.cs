//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

namespace XFramework
{
    [UIEvent(UIType.UISubPanel_MainMap)]
    internal sealed class UISubPanel_MainMapEvent : AUIEvent
    {
        public override string Key => UIPathSet.UISubPanel_MainMap;

        public override bool IsFromPool => true;

        public override bool AllowManagement => false;

        // 此UI是不受UIManager管理的

        public override UI OnCreate()
        {
            return UI.Create<UISubPanel_MainMap>();
        }
    }

    public partial class UISubPanel_MainMap : UI, IAwake<int>
    {
        public int blockId;

        public void Initialize(int args)
        {
            blockId = args;
            InitNode();
        }

        void InitNode()
        {
            // var KSpline = GetFromReference(UISubPanel_MainMap.KSpline);
            // var KBtn_CloudMask = GetFromReference(UISubPanel_MainMap.KBtn_CloudMask);
            // var KSplineAnimate_MoveEntity = GetFromReference(UISubPanel_MainMap.KSplineAnimate_MoveEntity);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}